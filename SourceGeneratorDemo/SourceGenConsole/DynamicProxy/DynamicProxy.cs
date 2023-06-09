using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using SourceGenConsole.DynamicProxy;

namespace SourceGenConsole.DynamicProxy; 

public class DynamicProxy {
    static AssemblyName assemblyName = new AssemblyName("DynamicProxy");
    static AssemblyBuilder builder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
    static ModuleBuilder moduleBuilder = builder.DefineDynamicModule("DynamicProxy");
    public static T CreateWithAssignInterceptors<T>(object[] ctorArguments,IInterceptor[] interceptors)
        where T : class {
        Type t = typeof(T);
        TypeBuilder typeBuilder = moduleBuilder.DefineType("DynamicProxy."+ t.Name , TypeAttributes.Public, t);
        // 生成拦截器字段
        FieldBuilder interceptorsField = typeBuilder.DefineField("_interceptors", typeof(IInterceptor[]), FieldAttributes.Private);
        // 生成targetType字段
        FieldBuilder targetTypeField = typeBuilder.DefineField("_targetType", typeof(Type), FieldAttributes.Private);
        ConstructorInfo[] ctor = t.GetConstructors();
        ILGenerator il;
        for (int i = 0; i < ctor.Length; i++) {
            ParameterInfo[] ctorParams = ctor[i].GetParameters();
            Type[] parameterTypes = new Type[ctorParams.Length + 2];
            parameterTypes[0] = typeof(IInterceptor[]);
            parameterTypes[1] = typeof(Type);
            for (int j = 0; j < ctorParams.Length; j++) {
                parameterTypes[j + 2] = ctorParams[j].ParameterType;
            }

            // 生成构造函数，添加额外的参数一个是拦截器，一个是targetType
            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                parameterTypes);

            il = constructorBuilder.GetILGenerator();
            // 调用父类的构造函数
            il.Emit(OpCodes.Ldarg_0);
            for (int j = 0; j < ctorParams.Length; j++) {
                il.Emit(OpCodes.Ldarg, j + 3);
            }
            il.Emit(OpCodes.Call, ctor[i]);
            // 赋值拦截器
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, interceptorsField);
            // 赋值targetType
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Stfld, targetTypeField);
            il.Emit(OpCodes.Ret);
        }

        // 生成方法
        foreach (MethodInfo method in t.GetMethods(BindingFlags.Public|BindingFlags.Instance | BindingFlags.DeclaredOnly)) {
            // 虚方法且公共 且特性
            if (method.IsVirtual && method.IsPublic && !method.IsGenericMethod) {
                var parameters = method.GetParameters();
                //生成另一个方法，用于调用父类的方法
                var targetMethod = typeBuilder.DefineMethod(typeBuilder.Name + "_" + method.Name, 
                    MethodAttributes.Public | MethodAttributes.HideBySig, 
                    method.ReturnType, 
                    parameters.Select(p => p.ParameterType).ToArray());
                il = targetMethod.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                for (int i = 1; i <= parameters.Length; i++) {
                    il.Emit(OpCodes.Ldarg,i);
                }
                il.Emit(OpCodes.Call,method);
                il.Emit(OpCodes.Ret);
                //重写父类方法
                var methodBuilder = typeBuilder.DefineMethod(method.Name, 
                    MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                    method.ReturnType, method.GetParameters().Select(p => p.ParameterType).ToArray());
                il = methodBuilder.GetILGenerator();
                
                //创建参数数组
                LocalBuilder arguments = il.DeclareLocal(typeof(object[]));
                il.Emit(OpCodes.Ldc_I4, parameters.Length);
                il.Emit(OpCodes.Newarr,typeof(object));
                il.Emit(OpCodes.Stloc,arguments);
                for (int i = 0; i < parameters.Length; i++) {
                    //读取数组对象
                    il.Emit(OpCodes.Ldloc, arguments);
                    //索引
                    il.Emit(OpCodes.Ldc_I4, i);
                    //读取方法上的参数
                    il.Emit(OpCodes.Ldarg, i + 1);
                    //值对象装箱
                    if (parameters[i].ParameterType.IsAssignableTo(typeof(ValueType))) {
                        il.Emit(OpCodes.Box, parameters[i].ParameterType);
                    }
                    il.Emit(OpCodes.Stelem_Ref);
                }
                //方法对象
                LocalBuilder methodInfoLocal = il.DeclareLocal(typeof(MethodInfo));
                il.Emit(OpCodes.Ldtoken, method);
                il.Emit(OpCodes.Call, typeof(MethodBase).GetMethod("GetMethodFromHandle", new[] { typeof(RuntimeMethodHandle) }));
                il.Emit(OpCodes.Castclass, typeof(MethodInfo));
                il.Emit(OpCodes.Stloc, methodInfoLocal);
                //真实方法Method对象
                LocalBuilder targetMethodInfoLocal = il.DeclareLocal(typeof(MethodInfo)); 
                il.Emit(OpCodes.Ldtoken, targetMethod);
                il.Emit(OpCodes.Call, typeof(MethodBase).GetMethod("GetMethodFromHandle", new[] { typeof(RuntimeMethodHandle) }));
                il.Emit(OpCodes.Castclass, typeof(MethodInfo));
                il.Emit(OpCodes.Stloc, targetMethodInfoLocal);
                
                //生成ProxyInvocation对象
                LocalBuilder proxyInvocationLocal = il.DeclareLocal(typeof(ProxyInvocation));
                var ci = typeof(ProxyInvocation).GetConstructor(
                    new[]
                    { typeof(object[]),
                      typeof(MethodInfo),
                      typeof(MethodInfo),
                      typeof(object),
                      typeof(Type),
                      typeof(IInterceptor[]) });
                il.Emit(OpCodes.Ldloc, arguments);
                il.Emit(OpCodes.Ldloc, methodInfoLocal);
                il.Emit(OpCodes.Ldloc, targetMethodInfoLocal);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, targetTypeField);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, interceptorsField);
                il.Emit(OpCodes.Newobj, ci);
                il.Emit(OpCodes.Stloc,proxyInvocationLocal);
                
                //执行ProxyInvocation对象的Proceed方法
                il.Emit(OpCodes.Ldloc, proxyInvocationLocal);
                il.Emit(OpCodes.Callvirt, typeof(IInvocation).GetMethod("Proceed"));
                //返回值
                if (method.ReturnType != typeof(void)) {
                    il.Emit(OpCodes.Ldloc, proxyInvocationLocal);
                    il.Emit(OpCodes.Callvirt, typeof(IInvocation).GetMethod("get_ReturnValue"));
                    if (method.ReturnType.IsValueType) {
                        il.Emit(OpCodes.Unbox_Any, method.ReturnType);
                    }
                }
                il.Emit(OpCodes.Ret);
            }
        }

        var type = typeBuilder.CreateType();
        var args = new object[2 + ctorArguments.Length];
        args[0] = interceptors; 
        args[1] = t;
        Array.Copy(ctorArguments, 0, args, 2, ctorArguments.Length);
        return (T)Activator.CreateInstance(type, args);
    }
    /// <summary>
    /// 泛型方法代理
    /// </summary>
    public static T CreateGenericMethodWithAssignInterceptors<T>(object[] ctorArguments,IInterceptor[] interceptors)
        where T : class {
       Type t = typeof(T);
        TypeBuilder typeBuilder = moduleBuilder.DefineType("DynamicProxy."+ t.Name , TypeAttributes.Public, t);
        // 生成拦截器字段
        FieldBuilder interceptorsField = typeBuilder.DefineField("_interceptors", typeof(IInterceptor[]), FieldAttributes.Private);
        // 生成targetType字段
        FieldBuilder targetTypeField = typeBuilder.DefineField("_targetType", typeof(Type), FieldAttributes.Private);
        ConstructorInfo[] ctor = t.GetConstructors();
        ILGenerator il;
        for (int i = 0; i < ctor.Length; i++) {
            ParameterInfo[] ctorParams = ctor[i].GetParameters();
            Type[] parameterTypes = new Type[ctorParams.Length + 2];
            parameterTypes[0] = typeof(IInterceptor[]);
            parameterTypes[1] = typeof(Type);
            for (int j = 0; j < ctorParams.Length; j++) {
                parameterTypes[j + 2] = ctorParams[j].ParameterType;
            }
            // 生成构造函数，添加额外的参数一个是拦截器，一个是targetType
            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                parameterTypes);

            il = constructorBuilder.GetILGenerator();
            // 调用父类的构造函数
            il.Emit(OpCodes.Ldarg_0);
            for (int j = 0; j < ctorParams.Length; j++) {
                il.Emit(OpCodes.Ldarg, j + 3);
            }
            il.Emit(OpCodes.Call, ctor[i]);
            // 赋值拦截器
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, interceptorsField);
            // 赋值targetType
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Stfld, targetTypeField);
            il.Emit(OpCodes.Ret);
        }
        // 生成方法
        foreach (MethodInfo methodInfo in t.GetMethods(BindingFlags.Public | BindingFlags.Instance |
                                                   BindingFlags.DeclaredOnly)) {
            // 虚方法且公共 且特性 //&& methodInfo.IsGenericMethod
            if (methodInfo.IsVirtual && methodInfo.IsPublic ) {
                Type[] genericArguments = methodInfo.GetGenericArguments();
                string[] argumentNames = new string[genericArguments.Length];
                for (int i = 0; i < genericArguments.Length; i++)
                {
                    argumentNames[i] = genericArguments[i].Name;
                }
                ParameterInfo[] parameters = methodInfo.GetParameters();
                Type[] parameterTypes = new Type[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    parameterTypes[i] = parameters[i].ParameterType;
                }
                //生成另一个方法，用于调用父类的方法
                var targetMethod = typeBuilder.DefineMethod(typeBuilder.Name + "_" + methodInfo.Name, 
                    MethodAttributes.Public | MethodAttributes.HideBySig, 
                    methodInfo.ReturnType, 
                    parameters.Select(p => p.ParameterType).ToArray());
                if (argumentNames.Length > 0) {
                    targetMethod.DefineGenericParameters(argumentNames);
                }
                
                il = targetMethod.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                for (int i = 1; i <= parameters.Length; i++) {
                    il.Emit(OpCodes.Ldarg,i);
                }
                il.Emit(OpCodes.Call,methodInfo);
                il.Emit(OpCodes.Ret);
                
                MethodBuilder methodBuilder = typeBuilder.DefineMethod(methodInfo.Name,
                    MethodAttributes.Public | MethodAttributes.Virtual,
                    methodInfo.ReturnType,
                    parameterTypes);
                GenericTypeParameterBuilder[] genericTypeParameterBuilders = methodBuilder.DefineGenericParameters(argumentNames);
                //定义泛型参数
                il = methodBuilder.GetILGenerator();
                //泛型参数类型数组
                LocalBuilder typeArray = il.DeclareLocal(typeof(Type[]));
                il.Emit(OpCodes.Ldc_I4, genericArguments.Length);
                il.Emit(OpCodes.Newarr, typeof(Type));
                il.Emit(OpCodes.Stloc, typeArray);
                for (int i = 0; i < genericArguments.Length; i++)
                {
                    il.Emit(OpCodes.Ldloc, typeArray);
                    il.Emit(OpCodes.Ldc_I4, i);
                    il.Emit(OpCodes.Ldtoken, genericTypeParameterBuilders[i]);
                    il.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));
                    il.Emit(OpCodes.Stelem_Ref);
                }
                //创建参数数组
                LocalBuilder objectArray = il.DeclareLocal(typeof(object[]));
                il.Emit(OpCodes.Ldc_I4, parameters.Length);
                il.Emit(OpCodes.Newarr,typeof(object));
                il.Emit(OpCodes.Stloc,objectArray);
                for (int i = 0; i < parameters.Length; i++) {
                    //读取数组对象
                    il.Emit(OpCodes.Ldloc, objectArray);
                    //索引
                    il.Emit(OpCodes.Ldc_I4, i);
                    //读取方法上的参数
                    il.Emit(OpCodes.Ldarg_S, i + 1);
                    //值对象装箱
                    if (parameterTypes[i].IsValueType || parameterTypes[i].IsGenericParameter) {
                        il.Emit(OpCodes.Box, parameterTypes[i]);
                    }
                    il.Emit(OpCodes.Stelem_Ref);
                    il.Emit(OpCodes.Ldarg, i + 1);
                    il.Emit(OpCodes.Box, parameterTypes[i]);
                    il.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] {typeof(object)}));
                }
                //方法对象
                LocalBuilder methodInfoLocal = il.DeclareLocal(typeof(MethodInfo));
                il.Emit(OpCodes.Ldtoken, methodInfo);
                il.Emit(OpCodes.Call, typeof(MethodBase).GetMethod("GetMethodFromHandle", new[]
                { typeof(RuntimeMethodHandle) }));
                il.Emit(OpCodes.Castclass, typeof(MethodInfo));
                il.Emit(OpCodes.Stloc, methodInfoLocal);
                //真实方法Method对象
                LocalBuilder targetMethodInfoLocal = il.DeclareLocal(typeof(MethodInfo)); 
                il.Emit(OpCodes.Ldtoken, targetMethod);
                il.Emit(OpCodes.Call, typeof(MethodBase).GetMethod("GetMethodFromHandle", new[] { typeof(RuntimeMethodHandle) }));
                il.Emit(OpCodes.Castclass, typeof(MethodInfo));
                il.Emit(OpCodes.Stloc, targetMethodInfoLocal);
                // //生成ProxyInvocation对象
                LocalBuilder proxyInvocationLocal = il.DeclareLocal(typeof(ProxyGenericInvocation));
                var ci = typeof(ProxyGenericInvocation).GetConstructor(
                    new[]
                    { typeof(object[]),
                      typeof(MethodInfo),
                      typeof(MethodInfo),
                      typeof(Type[]),
                      typeof(object),
                      typeof(Type),
                      typeof(IInterceptor[]) });
                il.Emit(OpCodes.Ldloc, objectArray);
                il.Emit(OpCodes.Ldloc, methodInfoLocal);
                il.Emit(OpCodes.Ldloc, targetMethodInfoLocal);
                il.Emit(OpCodes.Ldloc, typeArray);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, targetTypeField);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, interceptorsField);
                il.Emit(OpCodes.Newobj, ci);
                il.Emit(OpCodes.Stloc, proxyInvocationLocal);
                
                //执行ProxyInvocation对象的Proceed方法
                il.Emit(OpCodes.Ldloc, proxyInvocationLocal);
                il.Emit(OpCodes.Callvirt, typeof(IInvocation).GetMethod("Proceed"));
                //返回值
                if (methodInfo.ReturnType != typeof(void)) {
                    il.Emit(OpCodes.Ldloc, proxyInvocationLocal);
                    il.Emit(OpCodes.Callvirt, typeof(IInvocation).GetMethod("get_ReturnValue"));
                    if (methodInfo.ReturnType.IsValueType) {
                        il.Emit(OpCodes.Unbox_Any, methodInfo.ReturnType);
                    }
                }
                il.Emit(OpCodes.Ret);
            }
        }

        var type = typeBuilder.CreateType();
        var args = new object[2 + ctorArguments.Length];
        args[0] = interceptors; 
        args[1] = t;
        Array.Copy(ctorArguments, 0, args, 2, ctorArguments.Length);
        return (T)Activator.CreateInstance(type, args);
    }
}

public class ProxyInvocation : IInvocation {
    /// <summary>
    /// 方法中的参数
    /// </summary>
    public object[] Arguments { get; }

    /// <summary>
    /// 重写的方法类型
    /// </summary>
    public MethodInfo Method { get; }
    /// <summary>
    /// 真实调用的方法
    /// </summary>
    public MethodInfo MethodInvocationTarget { get; }
    /// <summary>
    /// 生成的代理对象
    /// </summary>
    public object Proxy { get; }
    /// <summary>
    /// 返回值
    /// </summary>
    public object ReturnValue { get; set; }

    public Type TargetType { get; }
    private int _interceptorIndex = -1;
    public IInterceptor[] Interceptors { get; }

    public ProxyInvocation(object[] arguments,
        MethodInfo method,
        MethodInfo methodInvocationTarget,
        object proxy,
        Type targetType,
        IInterceptor[] interceptors) {
        Arguments = arguments;
        Method = method;
        MethodInvocationTarget = methodInvocationTarget;
        Proxy = proxy;
        TargetType = targetType;
        Interceptors = interceptors;
    }

    public object GetArgumentValue(int index) {
        return Arguments[index];
    }

    public void Proceed() {
        if (Interceptors == null || Interceptors.Length == 0) {
            ReturnValue = MethodInvocationTarget.Invoke(Proxy, Arguments);
            return;
        }
        _interceptorIndex++;
        if (_interceptorIndex == Interceptors.Length) {
            ReturnValue = MethodInvocationTarget.Invoke(Proxy, Arguments);
        } else
            if (_interceptorIndex > Interceptors.Length) {
                throw new InvalidOperationException("No more interceptors to proceed to.");
            } else {
                Interceptors[_interceptorIndex].Intercept(this);
            }
    }
}

public class ProxyGenericInvocation : IInvocation {
    /// <summary>
    /// 方法中的参数
    /// </summary>
    public object[] Arguments { get; }
    /// <summary>
    /// 泛型参数类型
    /// </summary>
    public Type[] GenericTypeArguments { get; }

    /// <summary>
    /// 方法类型
    /// </summary>
    public MethodInfo Method { get; }
    /// <summary>
    /// 调用方法
    /// </summary>
    public MethodInfo MethodInvocationTarget { get; }

    /// <summary>
    /// 生成的代理对象
    /// </summary>
    public object Proxy { get; }
    /// <summary>
    /// 返回值
    /// </summary>
    public object ReturnValue { get; set; }

    public Type TargetType { get; }
    private int _interceptorIndex = -1;
    public IInterceptor[] Interceptors { get; }

    public ProxyGenericInvocation(object[] arguments,
        MethodInfo method,
        MethodInfo methodInvocationTarget,
        Type[] genericTypeArguments,
        object proxy,
        Type targetType,
        IInterceptor[] interceptors) {
        Arguments = arguments;
        Method = method;
        MethodInvocationTarget = methodInvocationTarget;
        GenericTypeArguments = genericTypeArguments;
        Proxy = proxy;
        TargetType = targetType;
        Interceptors = interceptors;
    }

    public object GetArgumentValue(int index) {
        return Arguments[index];
    }

    public void Proceed() {
        if (Interceptors == null || Interceptors.Length == 0) {
            ReturnValue = MethodInvocationTarget.MakeGenericMethod(GenericTypeArguments)
                .Invoke(Proxy, Arguments);
            return;
        }
        _interceptorIndex++;
        if (_interceptorIndex == Interceptors.Length) {
            ReturnValue = MethodInvocationTarget.MakeGenericMethod(GenericTypeArguments).Invoke(Proxy, Arguments);
        } else
            if (_interceptorIndex > Interceptors.Length) {
                throw new InvalidOperationException("No more interceptors to proceed to.");
            } else {
                Interceptors[_interceptorIndex].Intercept(this);
            }
    }
}