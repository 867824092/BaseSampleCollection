using System.Reflection;
using Castle.DynamicProxy;
namespace AspNetCoreIntegration.DynamicProxy;

public interface ICalculate {
    Task Add(int a, int b);
}
public class Calculate : ICalculate {
    [NewLogInterceptor]
    public virtual async Task  Add(int a, int b) {
        await Task.Delay(1000 * 1);
        Console.WriteLine("等待10s打印结果：{0}",a+b);
    }
}

public class LogInterceptor : IInterceptor {
    public void Intercept(IInvocation invocation) {
        Console.WriteLine("LogInterceptor  执行前....");
        invocation.Proceed();
        Console.WriteLine("LogInterceptor  执行后....");
    }
}

public class NewLogInterceptor : Guard.DynamicProxy.Abstracts.Attributes.InterceptorAttribute {
    public override void Intercept(Guard.DynamicProxy.Abstracts.Interfaces.IInvocation invocation) {
        Console.WriteLine("LogInterceptor  执行前...."); 
        invocation.Proceed();
        var result = invocation.ReturnValue;
        if(result is Task task) {
            Task.WaitAll(result as Task);
        }
        Console.WriteLine("LogInterceptor  执行后....");
    }
}