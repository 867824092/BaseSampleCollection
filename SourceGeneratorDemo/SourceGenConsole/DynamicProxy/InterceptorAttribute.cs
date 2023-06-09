namespace SourceGenConsole.DynamicProxy; 

[AttributeUsage(AttributeTargets.Class|AttributeTargets.Method, AllowMultiple = true)]
public abstract class  InterceptorAttribute : Attribute , IInterceptor
{
    public virtual void Intercept(IInvocation invocation) {
        invocation.Proceed();
        if (invocation.Method.ReturnType == typeof(Task) ||
            (invocation.Method.ReturnType.IsGenericType &&
             invocation.Method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))) {
            if (invocation.ReturnValue is Task)
            {
                Task.WaitAll(invocation.ReturnValue as Task);
            }
        }
    }
}

public class LogInterceptor : IInterceptor {
    public void Intercept(IInvocation invocation) {
        Console.WriteLine("LogInterceptor  执行前....");
        invocation.Proceed();
        if (invocation.Method.ReturnType == typeof(Task) ||
            (invocation.Method.ReturnType.IsGenericType &&
             invocation.Method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))) {
            if (invocation.ReturnValue is Task)
            {
                Task.WaitAll(invocation.ReturnValue as Task);
            }
        }
        Console.WriteLine("LogInterceptor  执行后....");
    }
}
public class TraceInterceptor : IInterceptor {
    public void Intercept(IInvocation invocation) {
        Console.WriteLine("TraceInterceptor   执行前....");
        invocation.Proceed();
        if (invocation.Method.ReturnType == typeof(Task) ||
            (invocation.Method.ReturnType.IsGenericType &&
             invocation.Method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))) {
            if (invocation.ReturnValue is Task)
            {
                Task.WaitAll(invocation.ReturnValue as Task);
            }
        }
        Console.WriteLine("TraceInterceptor   执行后....");
    }
}