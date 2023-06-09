// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DiagnosticAdapter;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

Console.WriteLine("Hello, World!");

DiagnosticListener.AllListeners.Subscribe(new CustomerOberver<DiagnosticListener>(listen => {
    listen.Subscribe(new CustomerOberver<KeyValuePair<string, object>>(eventData => {
        Console.WriteLine("key: {0}", eventData.Key);
        Console.WriteLine("values:{0}", eventData.Value);
    }));
}));


var listener = new DiagnosticListener("test");

listener.SubscribeWithAdapter(new DiagnosticCollector());

listener.Write("test.log", new { msg = "this is a message" });
ActivitySource s_source = new ActivitySource("Sample.DistributedTracing");
{ //openTelemetry

    using var tracerProvider = Sdk.CreateTracerProviderBuilder()
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("MySample"))
        .AddSource("Sample.DistributedTracing")
        .AddConsoleExporter()
        .Build();

    await DoSomeWork();
    Console.WriteLine("Example work done");
    
}


 async Task DoSomeWork()
{
    using (Activity a = s_source.StartActivity("SomeWork"))
    {
        await StepOne();
        await StepTwo();
    }
}

 async Task StepOne()
{
    using (Activity a = s_source.StartActivity("StepOne"))
    {
        await Task.Delay(500);
    }
}

 async Task StepTwo()
{
    using (Activity a = s_source.StartActivity("StepTwo"))
    {
        await Task.Delay(1000);
    }
}

Console.Read();

public class CustomerOberver<T> : IObserver<T> {

    private Action<T> next;
    public CustomerOberver(Action<T> action) {
        next = action;
    }

    public void OnCompleted() {
        //throw new NotImplementedException();
    }

    public void OnError(Exception error) {
        //throw new NotImplementedException();
    }

    public void OnNext(T value) {
        next(value);
    }
}

public class DiagnosticCollector {
    [DiagnosticName("test.log")]
    public void OnReceiveRequest(string msg) {
        Task.Run(async () => {
            await Task.Delay(1000 * 10);
            Console.WriteLine($"异步等待10s...接收到信息:{msg}");
        });
    }
}
