using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceGenConsole.DynamicProxy;

namespace SourceGenConsole {
    partial class Program {
        static async Task Main(string[] args) {
            // HelloFrom("Generated Code");
            // Console.WriteLine("********************");
            // RoslynSample.Run();
            // Console.WriteLine("********************");
            // RoslynSample.CSharpSyntaxRewriter();
            // Console.WriteLine("********************");
            // RoslynSample.CompilationRun();
            // Console.WriteLine("********************");
            // RoslynSample.Symbol();
            // Console.WriteLine("********************");
            {
                IInterceptor[] interceptors = {new LogInterceptor(),new TraceInterceptor()};
                {
                    // var test = DynamicProxy.DynamicProxy.CreateWithAssignInterceptors<Test>(
                    //     new object[]{"123456"},interceptors);
                    // var result = test.Run("Hello",20);
                    // Console.WriteLine(result);
                }
                {
                     // var test = DynamicProxy.DynamicProxy.CreateGenericMethodWithAssignInterceptors<Test>(new object[]{"123456"},interceptors);
                     // Console.WriteLine(test.Run2("Hello", 1));
                }
                {
                    var test = DynamicProxy.DynamicProxy.CreateWithAssignInterceptors<Test<string>>(
                        new object[]{"123456"},interceptors);
                    Console.WriteLine(test.Run("Hello", "泛型类型"));
                }
            }
        }
        static partial void HelloFrom(string name);
    }

    public class Test {
        public Test(string name) {
            Console.WriteLine("Test Constructor." + name);
        }
        public virtual  string Run(string name,int age) {
            //await Task.Delay(1000 * 1);
            //Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
            return "Test." + name + ".Age" + age;
        }
        
        public virtual string Run2<T1,T2>(T1 name,T2 t) {
            return "GenericMethod~Run2." + t + "."+ name;
        }
    }

    public class Test<T> {
        public Test(string name) {
            Console.WriteLine($"Test<{nameof(T)}> Constructor." + name);
        }
        public virtual string Run(string name,T t) {
            //await Task.Delay(1000 * 1);
            //Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
            return "Test<T>." + name + ".arguments" + t;
        }
    }
}