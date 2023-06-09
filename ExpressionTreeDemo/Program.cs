// 定义参数 a 和 b

using System.Linq.Expressions;
using ExpressionTreeToString;
using Microsoft.CodeAnalysis.CSharp.Scripting;


{
var a = Expression.Parameter(typeof(int), "a");
var b = Expression.Parameter(typeof(int), "b");

// 创建三元运算符表达式
var div = Expression.Divide(a, b);
var constant = Expression.Constant(-1, typeof(int));
var condition = Expression.GreaterThan(b, Expression.Constant(0, typeof(int)));
var ternary = Expression.Condition(condition, div, constant);

// 创建一个 Lambda 表达式
var lambda = Expression.Lambda<Func<int, int, int>>(ternary, a, b);


// 打印表达式树
Console.WriteLine(lambda.ToString("C#"));

// 编译表达式并调用
var func = lambda.Compile();
Console.WriteLine(func(10, 5));  // 2
Console.WriteLine(func(10, 0));  // -1
}

{
    var result = ConvertToArrayMethod()("1,2,3,4");
    Console.WriteLine(string.Join(" ",result));
    Func<string, int[]> ConvertToArrayMethod() {
        // 定义参数
        ParameterExpression fromParameter = Expression.Parameter(typeof(string), "from");

// 定义局部变量
        ParameterExpression partsVariable = Expression.Variable(typeof(string[]), "parts");
        ParameterExpression iVariable = Expression.Variable(typeof(int), "i");
        ParameterExpression lengthVariable = Expression.Variable(typeof(int), "length");
        ParameterExpression arrayVariable = Expression.Variable(typeof(int[]), "array");
        var label = Expression.Label();
// 创建表达式树
        BlockExpression block = Expression.Block(
            // 声明局部变量
            new[] { partsVariable, iVariable, lengthVariable, arrayVariable },
            // 赋值 partsVariable
            Expression.Assign(partsVariable, 
                Expression.Call(fromParameter, typeof(string).GetMethod("Split", new[] { typeof(char[]),typeof(StringSplitOptions) }), 
                    Expression.NewArrayInit(typeof(char), Expression.Constant(','), Expression.Constant('，')),
                    Expression.Constant(StringSplitOptions.RemoveEmptyEntries))),
            // 赋值 iVariable
            Expression.Assign(iVariable, Expression.Constant(0)),
            // 赋值 lengthVariable
            Expression.Assign(lengthVariable, Expression.Property(partsVariable, "Length")),
            // 赋值 arrayVariable
            Expression.Assign(arrayVariable, Expression.NewArrayBounds(typeof(int), lengthVariable)),
            // 循环
            Expression.Loop(
                Expression.IfThenElse(
                    Expression.LessThan(iVariable, lengthVariable),
                    Expression.Block(
                        // 赋值 array[i]
                        Expression.Assign(
                            Expression.ArrayAccess(arrayVariable, iVariable),
                            Expression.Call(typeof(int).GetMethod("Parse", new[] { typeof(string) })!, 
                                Expression.ArrayIndex(partsVariable, iVariable))
                        ),
                        // i ++
                        Expression.PostIncrementAssign(iVariable)
                    ),
                    Expression.Break(label)
                ),
                label
            ),
            // 返回 array
            arrayVariable
        );
        var lambda = Expression.Lambda<Func<string, int[]>>(block, fromParameter);
        Console.WriteLine(lambda.ToString("C#"));
        return  lambda.Compile();
    }
}
{
// Add the following directive to the file:
// using System.Linq.Expressions;

// Creating a parameter expression.
    ParameterExpression value = Expression.Parameter(typeof(int), "value");

// Creating an expression to hold a local variable.
    ParameterExpression result = Expression.Parameter(typeof(int), "result");

// Creating a label to jump to from a loop.
    LabelTarget label = Expression.Label(typeof(int));

// Creating a method body.
    BlockExpression block = Expression.Block(
        new[] { result },
        Expression.Assign(result, Expression.Constant(1)),
        Expression.Loop(
            Expression.IfThenElse(
                Expression.GreaterThan(value, Expression.Constant(1)),
                Expression.MultiplyAssign(result,
                    Expression.PostDecrementAssign(value)),
                Expression.Break(label, result)
            ),
            label
        )
    );
    var lambda = Expression.Lambda<Func<int, int>>(block, value);
    Console.WriteLine( lambda.ToString("C#"));

// Compile and run an expression tree.
    int factorial = lambda.Compile()(5);

    Console.WriteLine(factorial);

// This code example produces the following output:
//
// 120
}
{
    // 创建一个lambda表达式：x => x + 1
    var code = "int AddOne(int x) { return x + 1; }";
    var lambda = CSharpScript.EvaluateAsync<Func<int, int>>(code + " AddOne").Result;

    // 动态执行lambda表达式
    int result = lambda(2);
    Console.WriteLine(result); // 输出 3
}