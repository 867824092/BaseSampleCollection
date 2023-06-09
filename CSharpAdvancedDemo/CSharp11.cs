using System.Text;

namespace CSharpAdvancedDemo; 

public class CSharp11 {
    public static void Run() {
        short us = 100;
        Console.WriteLine(Convert.ToString(us,2));
        Console.WriteLine(Convert.ToString(us >> 10,2));
        string longMessage = """
    This is a long message.
    It has several lines.
        Some are indented
                more than others.
    Some should start at the first column.
    Some have "quoted text" in them.
    """;
        Console.WriteLine(longMessage);
        ReadOnlySpan<byte> bytes = "this utf-8 string"u8;
        Console.WriteLine(Encoding.UTF8.GetString(bytes));

        Console.WriteLine(Db.ToLower());
        Console.WriteLine(Db.ToLower());
    }
    public static string Db {
        get {
            return "123";
        }
    }
    internal class GenericAttribute<T> : Attribute { }
    public class Student {
        [Generic<string>()]
        public string Method() => default;
    }
}