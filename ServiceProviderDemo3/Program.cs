using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Linq;

namespace ServiceProviderDemo3 {
    internal class Program {
        static void Main(string[] args) {
            var (engineType, engineScopeType) = ResolveTypes();
            var root = new ServiceCollection().BuildServiceProvider();
            var child1 = root.CreateScope().ServiceProvider;
            var child2 = root.CreateScope().ServiceProvider;
            var engine = GetEngine(root);
            var rootScope = GetRootScope(engine, engineType);
            Console.WriteLine("ServiceProviderEngine 唯一性");
            // ServiceProviderEngine 唯一性
            Console.WriteLine(ReferenceEquals(GetEngine(rootScope, engineScopeType), engine));
            Console.WriteLine(ReferenceEquals(GetEngine(child1, engineScopeType), engine));
            Console.WriteLine(ReferenceEquals(GetEngine(child2, engineScopeType), engine));
            Console.WriteLine("ServiceProviderEngine 和 IServiceScopeFactory 的同一性");
            // ServiceProviderEngine 和 IServiceScopeFactory 的同一性
            Console.WriteLine(ReferenceEquals(root.GetRequiredService<IServiceScopeFactory>(), engine));
            Console.WriteLine(ReferenceEquals(child1.GetRequiredService<IServiceScopeFactory>(), engine));
            Console.WriteLine(ReferenceEquals(child2.GetRequiredService<IServiceScopeFactory>(), engine));
            Console.WriteLine("ServiceProviderEngineScope 提供的IServiceProvider是它自己");
            Console.WriteLine("ServiceProvider 提供的IServiceProvider是rootScope");
            //ServiceProviderEngineScope 提供的IServiceProvider是它自己
            //ServiceProvider 提供的IServiceProvider是rootScope
            Console.WriteLine(ReferenceEquals(root.GetRequiredService<IServiceProvider>(), rootScope));
            Console.WriteLine(ReferenceEquals(child1.GetRequiredService<IServiceProvider>(), child1));
            Console.WriteLine(ReferenceEquals(child2.GetRequiredService<IServiceProvider>(), child2));

            Console.WriteLine("ServiceProviderEngineScope 和 IServiceProvider的同一性");
            //ServiceProviderEngineScope 和 IServiceProvider的同一性
            Console.WriteLine(ReferenceEquals(rootScope.ServiceProvider, rootScope));
            Console.WriteLine(ReferenceEquals(((IServiceScope)child1).ServiceProvider, child1));
            Console.WriteLine(ReferenceEquals(((IServiceScope)child2).ServiceProvider, child2));
        }

        static (Type Engine, Type EngineScope) ResolveTypes() {
            var assembly = typeof(ServiceProvider).Assembly;
            var engine = assembly.GetTypes().Single(it => it.Name == "ServiceProviderEngine");
            var engineScope = assembly.GetTypes().Single(it => it.Name == "ServiceProviderEngineScope");
            return (engine, engineScope);
        }

        static object GetEngine(ServiceProvider serviceProvider) {
            var field = typeof(ServiceProvider).GetField("_engine", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            return field.GetValue(serviceProvider);
        }

        static object GetEngine(object enginScope, Type enginScopeType) {
            var property = enginScopeType.GetProperty("Engine", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            return property.GetValue(enginScope);
        }
        static IServiceScope GetRootScope(object engine, Type engineType) {
            var property = engineType.GetProperty("RootScope", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            return (IServiceScope)property.GetValue(engine);
        }
    }
}
