using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Newtonsoft.Json.Serialization;
using PluginManagent;
using System.Reflection;
using System.Runtime.Loader;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//var context = new AssemblyLoadContext("plugin");
builder.Services.Configure<MvcOptions>(config => {
    config.EnableEndpointRouting = false;
});
builder.Services.AddControllers()
    .AddNewtonsoftJson(options => {
        options.SerializerSettings.ContractResolver = new DefaultContractResolver();
    })
    //.ConfigureApplicationPartManager(part => {    
    //   part.ApplicationParts.Add(new AssemblyPart(context.LoadFromAssemblyPath(Path.Combine(builder.Environment.ContentRootPath, "Plugin","One", "PluginOne.dll"))));
    //})
    ;
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IActionDescriptorChangeProvider>(MyActionDescriptorChangeProvider.Instance);
builder.Services.AddSingleton(MyActionDescriptorChangeProvider.Instance);
builder.Services.AddSingleton(new PluginContexts());
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();


app.MapGet("/load", (string name,string path,ApplicationPartManager partManager, MyActionDescriptorChangeProvider changeProvider, PluginContexts plugin) => {
 
    if (!plugin.IsExists(name)) {
        var pluginContext = new PluginAssemblyLoadContext(name);
        var assembly = pluginContext.LoadFromAssemblyPath(Path.Combine(path));

        plugin.Add(name, pluginContext);
        partManager.ApplicationParts.Add(new AssemblyPart(assembly));
        changeProvider.Change();
        return $"插件{name}加载成功..";
    }
    else {
        return "插件不可重复加载...";
    }
});
app.MapGet("/unLoad", (string name,ApplicationPartManager partManager, MyActionDescriptorChangeProvider changeProvider, PluginContexts plugin) => {
    if (plugin.IsExists(name)) {
        plugin.Remove(name);
        partManager.ApplicationParts.Remove(partManager.ApplicationParts.FirstOrDefault(u => u.Name == name)!);
        changeProvider.Change();
        return $"插件{name}卸载成功..";
    }
    else {
        return "插件不存在...";
    }
});
app.Run();

public class People {
    public string Name => "admin";
}
