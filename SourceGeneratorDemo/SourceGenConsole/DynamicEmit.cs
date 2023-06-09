using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SourceGenConsole {
    public class DynamicEmit {
        public static void DynamicMethodAdvance() {
            {
                //Emit Stringbuilder
                // We will call: new StringBuilder ("Hello", 1000)
                DynamicMethod dy = new DynamicMethod("StringBuilder", null, null, typeof(DynamicEmit));
                ILGenerator il = dy.GetILGenerator();
                ConstructorInfo ci = typeof(StringBuilder).GetConstructor(new[]
                { typeof(string), typeof(int) });
                il.Emit(OpCodes.Ldstr, "Hello");
                il.Emit(OpCodes.Ldc_I4, 1000);
                il.Emit(OpCodes.Newobj, ci);
                Type[] strT =
                { typeof(string) };
                il.Emit(OpCodes.Ldstr, "World");
                il.Emit(OpCodes.Call, typeof(StringBuilder).GetMethod("Append", strT));
                il.Emit(OpCodes.Callvirt, typeof(object).GetMethod("ToString"));
                il.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", strT));
                il.Emit(OpCodes.Ret);

                dy.Invoke(null, null);
            }
            {
                // Exception Handling
                // try { throw new NotSupportedException(); }
                // catch (NotSupportedException ex) { Console.WriteLine (ex.Message); }
                // finally { Console.WriteLine ("Finally"); }
                DynamicMethod dy = new DynamicMethod("Exception", null, null, typeof(DynamicEmit));
                MethodInfo getMessageProp = typeof(NotSupportedException).GetProperty("Message").GetGetMethod();
                MethodInfo writeLine = typeof(Console).GetMethod("WriteLine", new[]
                { typeof(object) });
                ILGenerator il = dy.GetILGenerator();
                il.BeginExceptionBlock();
                ConstructorInfo ci = typeof(NotSupportedException).GetConstructor(new Type[0]);
                il.Emit(OpCodes.Newobj, ci);
                il.Emit(OpCodes.Throw);
                il.BeginCatchBlock(typeof(NotSupportedException));
                il.Emit(OpCodes.Callvirt, getMessageProp);
                il.Emit(OpCodes.Call, writeLine);
                il.BeginFinallyBlock();
                il.EmitWriteLine("Finally");
                il.EndExceptionBlock();
                il.Emit(OpCodes.Ret);
                dy.Invoke(null, null);
            }
            {
                // int x = 5;
                // while (x <= 10) Console.WriteLine (x++);
                // ILGenerator gen = ...
                // Label startLoop = gen.DefineLabel(); // Declare labels
                // Label endLoop = gen.DefineLabel();
                // LocalBuilder x = gen.DeclareLocal (typeof (int)); // int x
                // gen.Emit (OpCodes.Ldc_I4, 5); //
                // gen.Emit (OpCodes.Stloc, x); // x = 5
                // gen.MarkLabel (startLoop);
                // gen.Emit (OpCodes.Ldc_I4, 10); // Load 10 onto eval stack
                // gen.Emit (OpCodes.Ldloc, x); // Load x onto eval stack
                // 812 | Chapter 18: Reflection and Metadata
                // gen.Emit (OpCodes.Blt, endLoop); // if (x > 10) goto endLoop
                // gen.EmitWriteLine (x); // Console.WriteLine (x)
                // gen.Emit (OpCodes.Ldloc, x); // Load x onto eval stack
                // gen.Emit (OpCodes.Ldc_I4, 1); // Load 1 onto the stack
                // gen.Emit (OpCodes.Add); // Add them together
                // gen.Emit (OpCodes.Stloc, x); // Save result back to x
                // gen.Emit (OpCodes.Br, startLoop); // return to start of loop
                // gen.MarkLabel (endLoop);
                // gen.Emit (OpCodes.Ret);
            }
        }

        public static void DynamicAssemblyAdvance() {
            // AssemblyBuilder
            AssemblyName name = new AssemblyName("MyDynamicAssembly");
            AssemblyBuilder assemblyBuilder =
                AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MyDynamicModule");
            TypeBuilder typeBuilder = moduleBuilder.DefineType("Widget", TypeAttributes.Public);
            MethodBuilder methodBuilder =
                typeBuilder.DefineMethod("Hello", MethodAttributes.Public, null, null);
            ILGenerator il = methodBuilder.GetILGenerator();
            il.EmitWriteLine("Hello World");
            il.Emit(OpCodes.Ret);

            Type t = typeBuilder.CreateType();
            object o = Activator.CreateInstance(t);
            t.GetMethod("Hello").Invoke(o, null);
        }

        public static void EmittingTypeMembers() {
            AssemblyName name = new AssemblyName("MyEmissions");
            AssemblyBuilder assemblyBuilder =
                AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            TypeBuilder typeBuilder = moduleBuilder.DefineType("Widget", TypeAttributes.Public);
            //public static double SquareRoot (double value) => Math.Sqrt (value);
            MethodBuilder methodBuilder =
                typeBuilder.DefineMethod("SquareRoot", 
                    MethodAttributes.Public | MethodAttributes.Static, null, new[]
                { typeof(double).MakeByRefType(), typeof(int).MakeByRefType()});
            methodBuilder.DefineParameter(1, ParameterAttributes.None, "value");
            methodBuilder.DefineParameter(2, ParameterAttributes.Out, "result");
            ILGenerator ilGenerator = methodBuilder.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            //ref
            ilGenerator.Emit (OpCodes.Ldarg_0);
            ilGenerator.Emit (OpCodes.Ldind_R8);
            ilGenerator.Emit(OpCodes.Call, typeof(Math).GetMethod("Sqrt", new[]
            { typeof(double) }));
            //ref 
            ilGenerator.Emit (OpCodes.Stind_R8);
            
            
            ilGenerator.Emit(OpCodes.Ldarg_1);
            //out
            ilGenerator.Emit (OpCodes.Ldarg_1);
            ilGenerator.Emit (OpCodes.Ldind_I4);
            ilGenerator.Emit(OpCodes.Ldc_I4,100);
            ilGenerator.Emit(OpCodes.Add);
            ilGenerator.Emit (OpCodes.Stind_I4);
            
            ilGenerator.Emit(OpCodes.Ret);

            Type realType = typeBuilder.CreateType();
            object[] args = { 2.0,0 }; 
            typeBuilder.GetMethod("SquareRoot").Invoke(null, args);
            Console.WriteLine(args[0]);
            Console.WriteLine(args[1]);
        }

        public static void EmittingFieldAndProperty() {
            AssemblyName name = new AssemblyName("MyEmissions");
            AssemblyBuilder assemblyBuilder =
                AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            TypeBuilder typeBuilder = moduleBuilder.DefineType("Widget", TypeAttributes.Public);
        //     string _text;
        // public string Text
        // {
        //     get => _text;
        //     internal set => _text = value;
        // }
        FieldBuilder fieldBuilder = typeBuilder.DefineField("_text", typeof(string), FieldAttributes.Private);
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty("Text", PropertyAttributes.None, 
                typeof(string), new Type[0]);
            MethodBuilder getter = typeBuilder.DefineMethod("get_Text", MethodAttributes.Public, 
                typeof(string), Type.EmptyTypes);
            ILGenerator gen = getter.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, fieldBuilder);
            gen.Emit(OpCodes.Ret);
            
            MethodBuilder setter = typeBuilder.DefineMethod("set_Text", MethodAttributes.Public, 
                null, new[]
            { typeof(string) });
            gen = setter.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Stfld, fieldBuilder);
            gen.Emit(OpCodes.Ret);
            
            propertyBuilder.SetSetMethod(setter);
            propertyBuilder.SetGetMethod(getter);
            
            Type t = typeBuilder.CreateType();
            object o = Activator.CreateInstance (t);
            t.GetProperty ("Text").SetValue (o, "Good emissions!");
            string text = (string) t.GetProperty ("Text").GetValue (o, null);
            Console.WriteLine (text);
        }

        public static void EmittingConstructors() {
            AssemblyName name = new AssemblyName("MyDynamicAssembly");
            AssemblyBuilder assemblyBuilder =
                AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MyDynamicModule");
            TypeBuilder typeBuilder = moduleBuilder.DefineType("Widget", TypeAttributes.Public);
            FieldBuilder fieldBuilder = typeBuilder.DefineField("_name", typeof(string), FieldAttributes.Private);
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty("Name", PropertyAttributes.None, 
                typeof(string), new Type[0]);
            
            MethodBuilder setter = typeBuilder.DefineMethod("set_Name", MethodAttributes.Public, 
                null, new[]
            { typeof(string) });
            ILGenerator gen = setter.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Stfld,fieldBuilder);
            gen.Emit(OpCodes.Ret);
            
            MethodBuilder getter = typeBuilder.DefineMethod("get_Name", MethodAttributes.Public, 
                typeof(string), Type.EmptyTypes);
            gen = getter.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, fieldBuilder);
            gen.Emit(OpCodes.Ret);
            
            propertyBuilder.SetSetMethod(setter);
            propertyBuilder.SetGetMethod(getter);
            
            //声明构造函数
            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, 
                CallingConventions.Standard, new[]
            { typeof(string) });
            gen = constructorBuilder.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Call,typeof(object).GetConstructor(new Type[0]));
            
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Call, setter);
            gen.Emit(OpCodes.Ret);
            
            Type t = typeBuilder.CreateType();
            object o = Activator.CreateInstance(t, new object[] { "Hello" });
            string name1 = (string) t.GetProperty("Name").GetValue(o, null);
            Console.WriteLine(name1);
        }

        public static void EmittingGenericMethod() {
            AssemblyName name = new AssemblyName("MyEmissions");
            AssemblyBuilder assemblyBuilder =
                AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            // public static T Echo<T> (T value)
            // {
            //     return value;
            // }
            TypeBuilder typeBuilder = moduleBuilder.DefineType("Widget", TypeAttributes.Public);
            MethodBuilder methodBuilder = typeBuilder.DefineMethod("Echo", MethodAttributes.Public | MethodAttributes.Static, 
                CallingConventions.Standard);
            GenericTypeParameterBuilder[] genericParams = methodBuilder.DefineGenericParameters("T");
            methodBuilder.SetSignature(genericParams[0],null
                ,null,genericParams,null,null);
            methodBuilder.DefineParameter(1, ParameterAttributes.None, "value");
            ILGenerator gen = methodBuilder.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0); 
            gen.Emit(OpCodes.Ret);
            
            Type t = typeBuilder.CreateType();
            MethodInfo methodInfo = t.GetMethod("Echo");
            MethodInfo genericMethodInfo = methodInfo.MakeGenericMethod(typeof(string));
            object result = genericMethodInfo.Invoke(null, new object[] { "Hello" });
            Console.WriteLine(result);
        }

        public static void EmittingGenericType() {
            AssemblyName name = new AssemblyName("MyEmissions");
AssemblyBuilder assemblyBuilder =
                AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            TypeBuilder typeBuilder = moduleBuilder.DefineType("Widget", TypeAttributes.Public);
            GenericTypeParameterBuilder[] genericParams = typeBuilder.DefineGenericParameters("T");
            typeBuilder.DefineField("Value",genericParams[0], FieldAttributes.Public);
            Type t = typeBuilder.CreateType();
            Type genericType = t.MakeGenericType(typeof(string));
            object o = Activator.CreateInstance(genericType);
            genericType.GetField("Value").SetValue(o, "Hello");
            string value = (string) genericType.GetField("Value").GetValue(o);
            Console.WriteLine(value);
        }

       
        public static void EmittingUnCreatedClosedGeneric() {
            // public class Widget
            // {
            //     public static void Test() { var list = new List<int>(); }
            // }
            AssemblyName name = new AssemblyName("MyEmissions");
            AssemblyBuilder assemblyBuilder =
                AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            TypeBuilder typeBuilder = moduleBuilder.DefineType("Widget", TypeAttributes.Public);
            MethodBuilder methodBuilder = typeBuilder.DefineMethod("Test", MethodAttributes.Public | MethodAttributes.Static, 
                CallingConventions.Standard,null,null);
            ILGenerator gen = methodBuilder.GetILGenerator();
            
            Type variableType = typeof (List<int>);
            //ConstructorInfo ci = variableType.GetConstructor (new Type[]{typeof(int)});
            LocalBuilder listVar = gen.DeclareLocal (variableType);
            gen.Emit(OpCodes.Ldc_I4,10);
            // 创建一个新的 List<int> 对象，并将其引用推送到计算堆栈上
            gen.Emit(OpCodes.Newobj, typeof(List<int>).GetConstructor(new Type[] { typeof(int) }));
            //gen.Emit(OpCodes.Newobj, ci);
            gen.Emit(OpCodes.Stloc, listVar);
            gen.Emit(OpCodes.Ldloc,listVar);
            gen.Emit(OpCodes.Callvirt,typeof (List<int>).GetMethod("get_Capacity"));
            gen.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new[]
            { typeof(int) }));
            gen.Emit (OpCodes.Ret);

            Type t = typeBuilder.CreateType();
            t.GetMethod("Test").Invoke(null, null);
            
        }


        public static void AOPEmittingTest() {
            AssemblyName name = new AssemblyName("MyEmissions");
            AssemblyBuilder assemblyBuilder =
                AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            Type parent = typeof(Student);
            TypeBuilder typeBuilder = moduleBuilder.DefineType("ProxyStudent", TypeAttributes.Public,parent);
            MethodBuilder methodBuilder = typeBuilder.DefineMethod("PrintName", MethodAttributes.Public | MethodAttributes.Virtual, 
                CallingConventions.Standard,typeof(string),null);
            ILGenerator gen = methodBuilder.GetILGenerator();
            gen.Emit(OpCodes.Ldstr, "代码执行前....");
            gen.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }));
            LocalBuilder localBuilder = gen.DeclareLocal(typeof(string));
            //gen.Emit(OpCodes.Ldstr,"123");
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Call, parent.GetMethod("PrintName"));
            gen.Emit(OpCodes.Stloc,localBuilder);
            gen.Emit(OpCodes.Ldstr,"代码执行后....");
            gen.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }));
            gen.Emit(OpCodes.Ldloc,localBuilder);
            gen.Emit(OpCodes.Ret);
            Type t = typeBuilder.CreateType();
            //t.GetMethod("PrintName").Invoke(Activator.CreateInstance(t), null);
            Student o = (Student)Activator.CreateInstance(t);
            o.PrintName();
        }
    }

    public interface IPrint {
        string PrintName();
    }

    public class Student : IPrint {
        /// <summary>
        /// 年龄
        /// </summary>
        public int Aget;
        public string Name { get; set; }
        public virtual string PrintName()
        {
            Console.WriteLine("12");
            return Name;
        }
    }

    public class Proxy<T>  where T:IPrint {
	
        public T Value { get; set; }

        public string PrintName()
        {
            Console.WriteLine("代码执行前....");
            var result = Value.PrintName();
            Console.WriteLine("代码执行后....");
            return result;
        }
    }
}
