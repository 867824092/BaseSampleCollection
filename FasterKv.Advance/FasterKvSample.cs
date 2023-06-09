using FASTER.core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FasterKv.Advance {
    public class FasterKvSample {
        public static void Run() {
            Console.WriteLine("\nDisk Sample:\n");

            long key = 1, value = 1, output = 0;

            // Create FasterKV config based on specified base directory path.
            using var config = new FasterKVSettings<long, long>("./database") { TryRecoverLatest = true };
            Console.WriteLine($"FasterKV config:\n{config}\n");

            // Create store using specified config
            using var store = new FasterKV<long, long>(config);

            // Create functions for callbacks; we use a standard in-built function in this sample.
            // You can write your own by extending this or FunctionsBase.
            // In this in-built function, read-modify-writes will perform value merges via summation.
            var funcs = new SimpleFunctions<long, long>((a, b) => a + b);

            // Each logical sequence of calls to FASTER is associated with a FASTER session.
            // No concurrency allowed within a single session
            using var session = store.NewSession(funcs);

            if (store.RecoveredVersion == 1) // did not recover
            {
                Console.WriteLine("Clean start; upserting key-value pair");

                // (1) Upsert and read back upserted value
                session.Upsert(ref key, ref value);

                // Take checkpoint so data is persisted for recovery
                Console.WriteLine("Taking full checkpoint");
                store.TryInitiateFullCheckpoint(out _, CheckpointType.Snapshot);
                store.CompleteCheckpointAsync().AsTask().GetAwaiter().GetResult();
            }
            else {
                Console.WriteLine($"Recovered store to version {store.RecoveredVersion}");
            }

            // Reads are served back from memory and return synchronously
            var status = session.Read(ref key, ref output);
            if (status.Found && output == value)
                Console.WriteLine("(1) Success!");
            else
                Console.WriteLine("(1) Error!");

            // (2) Force flush record to disk and evict from memory, so that next read is served from disk
            store.Log.FlushAndEvict(true);

            // Reads from disk will return PENDING status, result available via either asynchronous IFunctions callback
            // or on this thread via CompletePendingWithOutputs, shown below
            status = session.Read(ref key, ref output);
            if (status.IsPending) {
                session.CompletePendingWithOutputs(out var iter, true);
                while (iter.Next()) {
                    if (iter.Current.Status.Found && iter.Current.Output == value)
                        Console.WriteLine("(2) Success!");
                    else
                        Console.WriteLine("(2) Error!");
                }
                iter.Dispose();
            }
            else
                Console.WriteLine("(2) Error!");

            /// (3) Delete key, read to verify deletion
            session.Delete(ref key);

            status = session.Read(ref key, ref output);
            if (status.Found)
                Console.WriteLine("(3) Error!");
            else
                Console.WriteLine("(3) Success!");

            // (4) Perform two read-modify-writes (summation), verify result
            key = 2;
            long input1 = 25, input2 = 27;

            session.RMW(ref key, ref input1);
            session.RMW(ref key, ref input2);

            status = session.Read(ref key, ref output);

            if (status.Found && output == input1 + input2)
                Console.WriteLine("(4) Success!");
            else
                Console.WriteLine("(4) Error!");


            // (5) Perform TryAdd using RMW and custom IFunctions
            using var tryAddSession = store.NewSession(new TryAddFunctions<long, long>());
            key = 3;
            input1 = 30;
            input2 = 31;

            // First TryAdd - success; status should be NOTFOUND (does not already exist)
            status = tryAddSession.RMW(ref key, ref input1);

            // Second TryAdd - failure; status should be OK (already exists)
            var status2 = tryAddSession.RMW(ref key, ref input2);

            // Read, result should be input1 (first TryAdd)
            var status3 = session.Read(ref key, ref output);

            if (!status.Found && status2.Found && status3.Found && output == input1)
                Console.WriteLine("(5) Success!");
            else
                Console.WriteLine("(5) Error!");

            Console.WriteLine("Success!");
            Console.ReadLine();
        }
        static readonly bool useReadCache = false;
        public static void RunLogStorage() {
            Console.WriteLine("This sample runs forever and takes up around 200MB of storage space while running");

            var context = default(CacheContext);

            var log = Devices.CreateLogDevice("hlog.log", deleteOnClose: true);
            var objlog = Devices.CreateLogDevice("hlog.obj.log", deleteOnClose: true);

            var logSettings = new LogSettings {
                LogDevice = log,
                ObjectLogDevice = objlog,
                MutableFraction = 0.9,
                MemorySizeBits = 20,
                PageSizeBits = 12,
                SegmentSizeBits = 20
            };

            if (useReadCache) {
                logSettings.ReadCacheSettings = new ReadCacheSettings();
            }

            using var h = new FasterKV<CacheKey, CacheValue>(
                1L << 20,
                logSettings,
                serializerSettings: new SerializerSettings<CacheKey, CacheValue> { keySerializer = () => new CacheKeySerializer(), valueSerializer = () => new CacheValueSerializer() }
                );

            using var s = h.For(new CacheFunctions()).NewSession<CacheFunctions>();

            const int max = 1000000;

            Console.WriteLine("Writing keys from 0 to {0} to FASTER", max);

            Stopwatch sw = new();
            sw.Start();
            for (int i = 0; i < max; i++) {
                if (i % 256 == 0) {
                    s.Refresh();
                    if (i % (1 << 19) == 0) {
                        long workingSet = Process.GetCurrentProcess().WorkingSet64;
                        Console.WriteLine($"{i}: {workingSet / 1048576}M");
                    }
                }
                var key = new CacheKey(i);
                var value = new CacheValue(i);
                s.Upsert(ref key, ref value, context, 0);
            }
            sw.Stop();
            Console.WriteLine("Total time to upsert {0} elements: {1:0.000} secs ({2:0.00} inserts/sec)", max, sw.ElapsedMilliseconds / 1000.0, max / (sw.ElapsedMilliseconds / 1000.0));
            Console.WriteLine("Log tail address: {0}", h.Log.TailAddress);

            // Issue mix of deletes and upserts
            Random r = new(3);

            while (true) {
                for (int iter = 0; iter < 3; iter++) {
                    sw.Restart();
                    for (int i = 0; i < max; i++) {
                        if (i % (1 << 19) == 0) {
                            long workingSet = Process.GetCurrentProcess().WorkingSet64;
                            Console.WriteLine($"{i}: {workingSet / 1048576}M");
                        }

                        var key = new CacheKey(iter == 2 ? i : r.Next(max));
                        if (iter < 2 && r.Next(2) == 0) {
                            s.Delete(ref key, context, 0);
                        }
                        else {
                            var value = new CacheValue(key.key);
                            s.Upsert(ref key, ref value, context, 0);
                        }
                    }
                    sw.Stop();
                    Console.WriteLine("Time to delete/upsert {0} elements: {1:0.000} secs ({2:0.00} inserts/sec)", max, sw.ElapsedMilliseconds / 1000.0, max / (sw.ElapsedMilliseconds / 1000.0));
                    Console.WriteLine("Log begin address: {0}", h.Log.BeginAddress);
                    Console.WriteLine("Log tail address: {0}", h.Log.TailAddress);
                }

                s.CompletePending(true);

                sw.Restart();
                s.Compact(h.Log.HeadAddress, CompactionType.Scan);
                sw.Stop();
                Console.WriteLine("Time to compact: {0:0.000} secs", sw.ElapsedMilliseconds / 1000.0);
                h.Log.Truncate();

                Console.WriteLine("Log begin address: {0}", h.Log.BeginAddress);
                Console.WriteLine("Log tail address: {0}", h.Log.TailAddress);
            }
        }
    }
    public class CacheKey : IFasterEqualityComparer<CacheKey> {
        public long key;

        public CacheKey() { }

        public CacheKey(long first) {
            key = first;
        }

        public long GetHashCode64(ref CacheKey key) {
            return Utility.GetHashCode(key.key);
        }
        public bool Equals(ref CacheKey k1, ref CacheKey k2) {
            return k1.key == k2.key;
        }
    }

    public class CacheKeySerializer : BinaryObjectSerializer<CacheKey> {
        public override void Deserialize(out CacheKey obj) {
            obj = new CacheKey(reader.ReadInt64());
        }

        public override void Serialize(ref CacheKey obj) {
            writer.Write(obj.key);
        }
    }

    public class CacheValue {
        public long value;

        public CacheValue() { }

        public CacheValue(long first) {
            value = first;
        }
    }

    public class CacheValueSerializer : BinaryObjectSerializer<CacheValue> {
        public override void Deserialize(out CacheValue obj) {
            obj = new CacheValue(reader.ReadInt64());
        }

        public override void Serialize(ref CacheValue obj) {
            writer.Write(obj.value);
        }
    }

    public struct CacheInput {
    }

    public struct CacheOutput {
        public CacheValue value;
    }

    public struct CacheContext {
        public int type;
        public long ticks;
    }

    public sealed class CacheFunctions : FunctionsBase<CacheKey, CacheValue, CacheInput, CacheOutput, CacheContext> {
        public override bool SingleReader(ref CacheKey key, ref CacheInput input, ref CacheValue value, ref CacheOutput dst, ref ReadInfo readInfo) {
            dst.value = value;
            return true;
        }

        public override bool ConcurrentReader(ref CacheKey key, ref CacheInput input, ref CacheValue value, ref CacheOutput dst, ref ReadInfo readInfo) {
            dst.value = value;
            return true;
        }

        public override void ReadCompletionCallback(ref CacheKey key, ref CacheInput input, ref CacheOutput output, CacheContext ctx, Status status, RecordMetadata recordMetadata) {
            if (ctx.type == 0) {
                if (output.value.value != key.key)
                    throw new Exception("Read error!");
            }
            else {
                long ticks = DateTime.Now.Ticks - ctx.ticks;

                if (!status.Found)
                    Console.WriteLine("Async: Value not found, latency = {0}ms", new TimeSpan(ticks).TotalMilliseconds);

                if (output.value.value != key.key)
                    Console.WriteLine("Async: Incorrect value {0} found, latency = {1}ms", output.value.value, new TimeSpan(ticks).TotalMilliseconds);
                else
                    Console.WriteLine("Async: Correct value {0} found, latency = {1}ms", output.value.value, new TimeSpan(ticks).TotalMilliseconds);
            }
        }
    }
}
