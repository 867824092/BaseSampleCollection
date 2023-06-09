using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using FASTER.core;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FasterKv.Advance {
    //[RPlotExporter]
    [MemoryDiagnoser]
    public class Test {

        //public List<GpsPointerClass> _GpsPointerClasses = new List<GpsPointerClass>();
        //public List<GpsPointeStruct> _GpsPointeStructs = new List<GpsPointeStruct>();

        [Params(1000,2000,5000)]
        public int Count;

        //// [Benchmark]
        //// public void BaseOnClassCache() {
        ////     for (int i = 0; i < Count; i++) {
        ////         _GpsPointerClasses.Add(new GpsPointerClass()
        ////         { Altitude = 1,
        ////           AlarmState = 0,
        ////           Signal = 0,
        ////           Latitude = 33.5555,
        ////           Location = "河南省郑州市金水区建业凯旋广场",
        ////           Longitude = 114.556666,
        ////           Mileage = 1234,
        ////           SendTime = DateTime.Now,
        ////           Status = 1,
        ////           DataState = 1,
        ////           GpsId = Guid.NewGuid().ToString("N"),
        ////           InsertionDate = DateTime.Now,
        ////           IsSuppleTrans = false });
        ////     }
        //// }
        //[Benchmark]
        //public void BaseOnStructCache() {
        //    for (int i = 0; i < Count; i++) {
        //        _GpsPointeStructs.Add(new GpsPointeStruct() {
        //            Platecolor = 1,
        //            PlateNo = "豫A0001",
        //            Altitude = 1,
        //            AlarmState = 0,
        //            Signal = 0,
        //            Latitude = 335555,
        //            //Location = "河南省郑州市金水区建业凯旋广场",
        //            Longitude = 114556666,
        //            Mileage = 1234,
        //            SendTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
        //            Status = 1,
        //            DataState = 1,
        //            GpsId = Guid.NewGuid().ToString("N"),
        //            InsertionDate = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
        //            IsSuppleTrans = 0
        //        });
        //    }
        //}

        [Benchmark]
        public void ValueWapperArrayCopy() {
            var vw = new ValueWapper(Count);
            var pool = ArrayPool<GpsPointStruct>.Shared.Rent(Count);
            for (int i = 0; i < Count; i++) {
                pool[i] = new GpsPointStruct() {
                    Platecolor = 1,
                    PlateNo = "豫A0001",
                    Altitude = 1,
                    AlarmState = 0,
                    Signal = 0,
                    Latitude = 335555,
                    //Location = "河南省郑州市金水区建业凯旋广场",
                    Longitude = 114556666,
                    Mileage = 1234,
                    SendTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                    Status = 1,
                    DataState = 1,
                    GpsId = Guid.NewGuid().ToString("N"),
                    InsertionDate = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                    IsSuppleTrans = 0
                };
            }
            vw.InsertByArray(pool[..Count]);
            ArrayPool<GpsPointStruct>.Shared.Return(pool, false);
        }
        [Benchmark]
        public void ValueWapperSigleInsert() {
            var vw = new ValueWapper(Count);
            for (int i = 0; i < Count; i++) {
                vw.Insert(new GpsPointStruct() {
                    Platecolor = 1,
                    PlateNo = "豫A0001",
                    Altitude = 1,
                    AlarmState = 0,
                    Signal = 0,
                    Latitude = 335555,
                    //Location = "河南省郑州市金水区建业凯旋广场",
                    Longitude = 114556666,
                    Mileage = 1234,
                    SendTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                    Status = 1,
                    DataState = 1,
                    GpsId = Guid.NewGuid().ToString("N"),
                    InsertionDate = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                    IsSuppleTrans = 0
                });
            }
        }

        [Benchmark]
        public void ValueWapperSpanCopy() {
            var vw = new ValueWapper(Count);
            var pool = ArrayPool<GpsPointStruct>.Shared.Rent(Count);
            for (int i = 0; i < Count; i++) {
                pool[i] = new GpsPointStruct() {
                    Platecolor = 1,
                    PlateNo = "豫A0001",
                    Altitude = 1,
                    AlarmState = 0,
                    Signal = 0,
                    Latitude = 335555,
                    //Location = "河南省郑州市金水区建业凯旋广场",
                    Longitude = 114556666,
                    Mileage = 1234,
                    SendTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                    Status = 1,
                    DataState = 1,
                    GpsId = Guid.NewGuid().ToString("N"),
                    InsertionDate = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                    IsSuppleTrans = 0
                };
            }
            vw.InsertBySpan(pool.AsSpan(0,Count));
            ArrayPool<GpsPointStruct>.Shared.Return(pool, false);
        }

    }
    public struct ValueWapper {
        private GpsPointStruct[] _gpsPoints;
        public ValueWapper() : this(100) {

        }
        public ValueWapper(int capacity) {
            _gpsPoints = new GpsPointStruct[capacity];
            _capacity = capacity;
        }
        public ReadOnlySpan<GpsPointStruct> ReadOnlyWriteGpsPoint => _gpsPoints.AsSpan(0, _write);
        //总容量
        private int _capacity;
        public int Capacity => _capacity;
        //已写入量
        private int _write = 0;
        //已写入
        public int Write => _write;
        //剩余容量
        public int Free => _capacity - _write;
        public void Insert(GpsPointStruct gpsPointStruct) {
            _gpsPoints[_write] = gpsPointStruct;
            _write++;
        }
        public void InsertBySpan(ReadOnlySpan<GpsPointStruct> gpsPointStructs) {
            gpsPointStructs.CopyTo(_gpsPoints.AsSpan(_write, gpsPointStructs.Length));
            _write += gpsPointStructs.Length;
        }
        public void InsertByArray(GpsPointStruct[] gpsPointStructs) {
             Array.Copy(gpsPointStructs,0, _gpsPoints, _write, gpsPointStructs.Length);
             _write += gpsPointStructs.Length;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CheckAndResize(int length) {
            if (Free < length) {
                // 容量扩容
                Array.Resize(ref _gpsPoints, Capacity >= length ? Capacity * 2 : length * 2);
            }
        }
    }
    public class GpsPointerClass {
        /// <summary>
        /// gps信号Id
        /// </summary>
        public string GpsId { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string PlateNo { get; set; }
        /// <summary>
        /// 车辆Id
        /// </summary>
        public string VehicleId { get; set; }
        /// <summary>
        /// gps信号时间
        /// </summary>
        public DateTime SendTime { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public double Longitude { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public double Latitude { get; set; }
        /// <summary>
        /// 速度
        /// </summary>
        public int Velocity { get; set; }
        /// <summary>
        /// 方向
        /// </summary>
        public ushort Direction { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public uint Status { get; set; }
        /// <summary>
        /// 里程
        /// </summary>
        public long Mileage { get; set; }
        /// <summary>
        /// 行车记录仪速度
        /// </summary>
        public int RecordVelocity { get; set; }
        /// <summary>
        /// 位置（暂不用解析）
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// 程序接收时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 报警状态
        /// </summary>
        public uint AlarmState { get; set; }
        /// <summary>
        /// 海拔高度
        /// </summary>
        public int Altitude { get; set; }
        /// <summary>
        /// 是否定位
        /// </summary>
        public bool Valid { get; set; }
        /// <summary>
        /// 车辆信号状态位
        /// </summary>
        public uint Signal { get; set; }
        /// <summary>
        /// 数据状态
        /// </summary>
        public int DataState { get; set; }
        /// <summary>
        /// 车牌颜色
        /// </summary>
        public int Platecolor { get; set; }
        /// <summary>
        /// 是否补发信号
        /// </summary>
        public bool? IsSuppleTrans { get; set; } = false;
        /// <summary>
        /// 数据入库时间
        /// </summary>
        public DateTime? InsertionDate { get; set; }
    }

    public struct GpsPointStruct {
        /// <summary>
        /// gps信号Id
        /// </summary>
        public string GpsId { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string PlateNo { get; set; }
        /// <summary>
        /// 车辆Id
        /// </summary>
        public string VehicleId { get; set; }
        /// <summary>
        /// gps信号时间
        /// </summary>
        public long SendTime { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public long Longitude { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public long Latitude { get; set; }
        /// <summary>
        /// 速度
        /// </summary>
        public byte Velocity { get; set; }
        /// <summary>
        /// 方向
        /// </summary>
        public byte Direction { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public uint Status { get; set; }
        /// <summary>
        /// 里程
        /// </summary>
        public uint Mileage { get; set; }
        /// <summary>
        /// 行车记录仪速度
        /// </summary>
        public uint RecordVelocity { get; set; }
        /// <summary>
        /// 位置（暂不用解析）
        /// </summary>
        ///public string Location { get; set; }
        /// <summary>
        /// 程序接收时间
        /// </summary>
        public long CreateDate { get; set; }
        /// <summary>
        /// 报警状态
        /// </summary>
        public uint AlarmState { get; set; }
        /// <summary>
        /// 海拔高度
        /// </summary>
        public int Altitude { get; set; }
        /// <summary>
        /// 是否定位
        /// </summary>
        public byte Valid { get; set; }
        /// <summary>
        /// 车辆信号状态位
        /// </summary>
        public uint Signal { get; set; }
        /// <summary>
        /// 数据状态
        /// </summary>
        public byte DataState { get; set; }
        /// <summary>
        /// 车牌颜色
        /// </summary>
        public byte Platecolor { get; set; }
        /// <summary>
        /// 是否补发信号
        /// </summary>
        public byte IsSuppleTrans { get; set; }
        /// <summary>
        /// 数据入库时间
        /// </summary>
        public long InsertionDate { get; set; }
    }


    [GcForce]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [MemoryDiagnoser]
    [HtmlExporter]
    public class AddBench {
        private const int ThreadCount = 6;
        private const int NumCount = 200_0000;

        private ConcurrentDictionary<long, Data> _concurrent;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static async Task FasterInternal(double percent, bool inMemory = false, bool multi = false) {
            FasterKVSettings<long, Data> kvSetting;

            if (inMemory) {
                kvSetting = new FasterKVSettings<long, Data>(null);
            }
            else {
                // 总计内存大小 总数 * (key + 每个Data需要占用的内存)
                var dataByte = NumCount * (Unsafe.SizeOf<Data>() + 8 + 8);

                // 计算memorySize 计划只使用{percent * 100}%的内存 需要是2的次幂
                var memorySizeBits = (int)Math.Ceiling(Math.Log2(dataByte * percent));

                // 根据数量计算IndexSize 需要是2的次幂
                var numBucketBits = (int)Math.Ceiling(Math.Log2(NumCount));
                kvSetting = new FasterKVSettings<long, Data>("./faster-add", deleteDirOnDispose: true) {
                    IndexSize = 1L << numBucketBits,
                    MemorySize = 1L << memorySizeBits
                };

                // 不分页
                kvSetting.PageSize = kvSetting.MemorySize;
            }


            using var fkv = new FasterKV<long, Data>(kvSetting);
            if (multi) {
                await FasterMultiThread(fkv);
            }
            else {
                FasterSingleThread(fkv);
            }

            kvSetting.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void FasterSingleThread(FasterKV<long, Data> fkv) {
            using var session = fkv.For(new SimpleFunctions<long, Data>()).NewSession<SimpleFunctions<long, Data>>();
            for (int i = 0; i < NumCount; i++) {
                session.Upsert(i, new Data());
            }

            session.CompletePending(true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static async Task FasterMultiThread(FasterKV<long, Data> fkv) {
            const int perCount = NumCount / ThreadCount;
            var tasks = new Task[ThreadCount];
            for (var i = 0; i < ThreadCount; i++) {
                var i1 = i;
                var session = fkv.For(new SimpleFunctions<long, Data>())
                    .NewSession<SimpleFunctions<long, Data>>();
                tasks[i] = Task.Run(() =>
                {
                    var j = i1 * perCount;
                    var length = j + perCount;
                    for (; j < length; j++) {
                        session.Upsert(j, new Data());
                    }
                    session.CompletePending(true);
                });
            }

            await Task.WhenAll(tasks);
        }

        [Benchmark]
        public async Task Faster_Hybrid_10per_Memory_Add() {
            await FasterInternal(0.10);
        }

        [Benchmark]
        public async Task Faster_Hybrid_25per_Memory_Add() {
            await FasterInternal(0.25);
        }

        [Benchmark]
        public async Task Faster_Hybrid_50per_Memory_Add() {
            await FasterInternal(0.50);
        }

        [Benchmark]
        public async Task Faster_Hybrid_90per_Memory_Add() {
            await FasterInternal(0.90);
        }

        [Benchmark]
        public async Task Faster_Hybrid_100per_Memory_Add() {
            await FasterInternal(1.0);
        }

        [Benchmark]
        public async Task Faster_Default_InMemory_Add() {
            await FasterInternal(0, true);
        }

        [Benchmark]
        public async Task Faster_Hybrid_10per_Memory_Multi_Add() {
            await FasterInternal(0.10, multi: true);
        }

        [Benchmark]
        public async Task Faster_Hybrid_25per_Memory_Multi_Add() {
            await FasterInternal(0.25, multi: true);
        }

        [Benchmark]
        public async Task Faster_Hybrid_90per_Memory_Multi_Add() {
            await FasterInternal(0.90, multi: true);
        }

        [Benchmark]
        public async Task Faster_Hybrid_100per_Memory_Multi_Add() {
            await FasterInternal(1.0, multi: true);
        }

        [Benchmark]
        public async Task Faster_Hybrid_50per_Memory_Multi_Add() {
            await FasterInternal(0.50, multi: true);
        }

        [Benchmark]
        public async Task Faster_Default_InMemory_Multi_Add() {
            await FasterInternal(0, true, true);
        }

        [Benchmark]
        public void Concurrent_Add() {
            _concurrent = new ConcurrentDictionary<long, Data>(1, NumCount);
            for (long i = 0; i < NumCount; i++) {
                _concurrent.TryAdd(i, new Data());
            }
        }

        [Benchmark]
        public async Task Concurrent_Multi_Add() {
            const int perCount = NumCount / ThreadCount;
            var tasks = new Task[ThreadCount];
            _concurrent = new ConcurrentDictionary<long, Data>(1, NumCount);
            for (var i = 0; i < ThreadCount; i++) {
                var i1 = i;
                tasks[i] = Task.Run(() =>
                {
                    var j = i1 * perCount;
                    var length = j + perCount;
                    for (; j < length; j++) {
                        _concurrent.TryAdd(j, new Data());
                    }
                });
            }

            await Task.WhenAll(tasks);
        }
    }

    [StructLayout(LayoutKind.Auto)]
    public struct Data {
        public long One { get; set; }
        public long Two { get; set; }
        public long Three { get; set; }
        public long Four { get; set; }
    }
}
