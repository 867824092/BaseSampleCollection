using System.Reflection;

namespace SourceGenConsole.DynamicProxy; 

/// <summary>
/// 拦截器
/// </summary>
public interface IInterceptor {
    void Intercept(IInvocation invocation);
}

public interface IInvocation
	{
		/// <summary>
		///   Gets the arguments that the <see cref = "Method" /> has been invoked with.
		/// </summary>
		/// <value>The arguments the method was invoked with.</value>
		object[] Arguments { get; }
		/// <summary>
		///   Gets the <see cref = "MethodInfo" /> representing the method being invoked on the proxy.
		/// </summary>
		/// <value>The <see cref = "MethodInfo" /> representing the method being invoked.</value>
		MethodInfo Method { get; }
		
		/// <summary>
		///   Gets the proxy object on which the intercepted method is invoked.
		/// </summary>
		/// <value>Proxy object on which the intercepted method is invoked.</value>
		object Proxy { get; }
		/// <summary>
		///   Gets or sets the return value of the method.
		/// </summary>
		/// <value>The return value of the method.</value>
		object ReturnValue { get; set; }

		/// <summary>
		///   Gets the type of the target object for the intercepted method.
		/// </summary>
		/// <value>The type of the target object.</value>
		Type TargetType { get; }

		/// <summary>
		///   Gets the value of the argument at the specified <paramref name = "index" />.
		/// </summary>
		/// <param name = "index">The index.</param>
		/// <returns>The value of the argument at the specified <paramref name = "index" />.</returns>
		object GetArgumentValue(int index);
		/// <summary>
		///   Proceeds the call to the next interceptor in line, and ultimately to the target method.
		/// </summary>
		/// <remarks>
		///   Since interface proxies without a target don't have the target implementation to proceed to,
		///   it is important, that the last interceptor does not call this method, otherwise a
		///   <see cref = "NotImplementedException" /> will be thrown.
		/// </remarks>
		void Proceed();
	}