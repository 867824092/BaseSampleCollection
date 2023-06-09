using System.Diagnostics;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using AspNetCoreIntegration.Binders;
using AspNetCoreIntegration.Controllers;
using AspNetCoreIntegration.Data;
using AspNetCoreIntegration.DiagnosticCollectors;
using AspNetCoreIntegration.DynamicProxy;
using AspNetCoreIntegration.Models;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using Guard.DynamicProxy.Core;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace AspNetCoreIntegration {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);
            #region MVC

            // Add services to the container.
            builder.Services.AddDbContext<AuthorContext>(options => options.UseInMemoryDatabase("Authors"));
            builder.Services.AddControllers(options => {
                //options.ModelBinderProviders.Insert(0, new AuthorEntityBinderProvider());
            }).ConfigureApplicationPartManager(part => {
                part.FeatureProviders.Add(new ApplicationServiceControllerFeatureProvider());
            });
            builder.Services.Configure<MvcOptions>(options => {
                options.Conventions.Add(new ApplicationServiceConvention());
            });

            #endregion

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            #region Authentication Authorization

            // builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //     .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,options => {
            //         options.LoginPath = "/api/account";
            //         options.LogoutPath = "/api/account";
            //     });
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
                    options => { });

            builder.Services.AddAuthorization(options => {
                // options.AddPolicy("CookieDefault",
                //     new AuthorizationPolicyBuilder()
                //     .RequireRole("Admin").Build());
            });

            #endregion

            builder.Services.AddSingleton<MvcActionDiagnosticCollector>();

            //Autofac
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory(builder => {
                // builder.RegisterType<Calculate>().As<ICalculate>()
                //     .InterceptedBy(typeof(LogInterceptor))
                //     .EnableClassInterceptors();
                // builder.RegisterType<LogInterceptor>();

                builder.Register(t => {
                    var defaultProxyBuilder =
                        new Guard.DynamicProxy.Core.DefaultProxyBuilder(new Guard.DynamicProxy.Core.ModuleScope());
                    var generator = new Guard.DynamicProxy.Core.DefaultProxyGenerator(defaultProxyBuilder);
                    return generator.CreateInterfaceProxy<ICalculate, Calculate>();
                });
            }));
            var app = builder.Build();

            #region diagnostic

            var diagnostic = app.Services.GetRequiredService<DiagnosticListener>();
            diagnostic.SubscribeWithAdapter(app.Services.GetRequiredService<MvcActionDiagnosticCollector>());

            #endregion

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //StaticFileOptions fileOptions = new();
            // var provider = new PhysicalFileProvider(@"F:\Samples\Net\AspNetCoreSamples\AspNetCoreIntegration\bin\Debug\net6.0\wwwroot");
            // fileOptions.FileProvider = provider;
            // app.UseStaticFiles(fileOptions);
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();
            using (var scope = app.Services.CreateScope()) {
                var context = scope.ServiceProvider.GetRequiredService<AuthorContext>();

                context.Authors.Add(new Author
                { Name = "Steve Smith", Twitter = "ardalis", GitHub = "ardalis", BlogUrl = "ardalis.com" });
                context.SaveChanges();
            }
            app.Run();

            // Task.Factory.StartNew(async () => {
            //     while (true) {
            //         await Task.Delay(20 * 1000);
            //         fileOptions.FileProvider = new PhysicalFileProvider("F:\\test");
            //     }
            // },TaskCreationOptions.LongRunning);
        }
    }
}