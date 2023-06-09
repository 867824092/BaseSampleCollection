using Spectre.Console.Cli;
using SpectreConsoleDemo;

var app = new CommandApp();
app.Configure(config => {
    config.AddCommand<FileSizeCommand>("file");
    config.AddCommand<RunCommand>("run");
});
await app.RunAsync(args);

