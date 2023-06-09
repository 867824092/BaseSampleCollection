namespace PluginOne {
    class Program {
        static void Main(params string[] args) {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            app.MapGet("/", () => "Hello World!");

            app.Run();
        }
    }
}


