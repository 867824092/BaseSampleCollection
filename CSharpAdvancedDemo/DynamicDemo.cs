using System.Dynamic;

namespace CSharpAdvancedDemo; 

public class DynamicDemo {
    public static void Run() {
        dynamic d = new Duck();
        d.Quack();
        d.Waddle();
    }
    
    public class Duck : DynamicObject {
        public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result) {
            Console.WriteLine(binder.Name + " method was called");
            result = null;
            return true;
        }
    }
}