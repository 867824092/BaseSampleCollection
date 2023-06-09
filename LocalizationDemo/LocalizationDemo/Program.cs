using LocalizationExtensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddViewLocalization(options => options.ResourcesPath = "Resources");
builder.Services//.AddLocalization(options=>options.ResourcesPath = "Resources")
    .AddJsonLocalization(options=>options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options => {
    var supportedCultures = new[] { "zh-CN", "en-US" };
    options.SetDefaultCulture(supportedCultures[0])
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

var supportedCultures = new[] { "zh-CN", "en-US" };
var localizationOptions =
    new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);
localizationOptions.ApplyCurrentCultureToResponseHeaders = true;
app.UseRequestLocalization(localizationOptions);
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
