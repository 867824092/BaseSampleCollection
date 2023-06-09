
using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using FASTER.core;
using FasterKv.Advance;

public class Program {

    public static void Main(string[] args) {
        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);


        //FasterKvSample.Run();

        //FasterKvSample.RunLogStorage();
        //int count = 1000;
        //var vw = new ValueWapper(count);
        //var pool = ArrayPool<GpsPointStruct>.Shared.Rent(count);
        //for (int i = 0; i < count; i++) {
        //    pool[i] = new GpsPointStruct() {
        //        Platecolor = 1,
        //        PlateNo = "豫A0001",
        //        Altitude = 1,
        //        AlarmState = 0,
        //        Signal = 0,
        //        Latitude = 335555,
        //        //Location = "河南省郑州市金水区建业凯旋广场",
        //        Longitude = 114556666,
        //        Mileage = 1234,
        //        SendTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
        //        Status = 1,
        //        DataState = 1,
        //        GpsId = Guid.NewGuid().ToString("N"),
        //        InsertionDate = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
        //        IsSuppleTrans = 0
        //    };
        //}
        ////vw.InsertBySpan(pool.AsSpan(0,count));
        //vw.InsertByArray(pool[..count]);
        //ArrayPool<GpsPointStruct>.Shared.Return(pool, false);

        Console.ReadLine();
    }
}
