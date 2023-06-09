using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
    // .LoadFromMemory(new List<RouteConfig>()
    // {
    //  new RouteConfig()
    //  {
    //  RouteId = "github",
    //   ClusterId = "github",
    //   Match = new RouteMatch()
    //   {
    //    Path = "/github-proxy/{**catch-all}"
    //   }
    //  }
    // },new List<ClusterConfig>()
    // {
    //   new ClusterConfig()
    //   {
    //    ClusterId = "github",
    //    Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
    //    {
    //     { "github", new DestinationConfig() { Address = "https://20.205.243.166" } }
    //    },
    //    HttpClient = new HttpClientConfig()
    //    {
    //     DangerousAcceptAnyServerCertificate = true,
    //     SslProtocols = System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls13
    //    }
    //   }
    // });
var app = builder.Build();
app.MapReverseProxy();
app.Run();