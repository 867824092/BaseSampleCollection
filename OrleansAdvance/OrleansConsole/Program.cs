// using Microsoft.Extensions.Logging;
// using Orleans.Configuration;
// using Orleans.Hosting;
//
// var builder = new SiloHostBuilder()
//     .UseLocalhostClustering()
//     .Configure<ClusterOptions>(options => {
//         options.ClusterId = "dev";
//         options.ServiceId = "OrleansBasics";
//     })
//     .ConfigureApplicationParts(parts=>
//         parts.AddApplicationPart(typeof(EchoTalker).Assembly).WithReferencs())
//     .AddMemoryGrainStorage("Profile",config=>config.NumStorageGrains = 255)
//     .AddMemoryGrainStorage("Cart", config=>config.NumStorageGrains = 1000)
//     .ConfigureLogging(logging=>logging.AddConsole());
//     
//     var host = builder.Build();
//     await host.StartAsync();

Console.Write(123);