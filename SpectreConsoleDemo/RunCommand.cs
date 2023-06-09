using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace SpectreConsoleDemo {
    public class RunCommand : Command<RunCommand.Settings> {

        public sealed class Settings : CommandSettings {

            [Description("监听的ip地址")]
            [CommandOption("-i|--ip")]
            public string? Http { get; set; }

            [Description("监听的端口")]
            [CommandOption("-p|--port")]
            public int Port { get; init; }
        }

        public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings) {
            var builder = WebApplication.CreateBuilder();

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
            return 1;
        }
    }
}
