namespace TaskAdvanced; 

public class AsyncLocalDemo {
    
        #region 基础方式

        // private static AsyncLocal<string> _asyncLocal = new AsyncLocal<string>();
        //
        // public static async Task Run() {
        //     _asyncLocal.Value = "A";
        //     Task.Run(() => Console.WriteLine($"AsyncLocal in task: {_asyncLocal.Value}"));
        //     await FooAsync();
        //     Console.WriteLine($"AsyncLocal after await FooAsync: {_asyncLocal.Value}");
        // }
        // private static async Task FooAsync()
        // {
        //     _asyncLocal.Value = "B";
        //     Console.WriteLine($"AsyncLocal before await in FooAsync: {_asyncLocal.Value}");
        //     await Task.Delay(100);
        //     Console.WriteLine($"AsyncLocal after await in FooAsync: {_asyncLocal.Value}");
        // }
#endregion

#region 改造
private static IValueAccessor<string> _valueAccessor = new ValueAccessor<string>();

public static async Task Run()
{
    _valueAccessor.Value = "A";
    Console.WriteLine($"ValueAccessor before await FooAsync in Main: {_valueAccessor.Value}");
    await FooAsync();
    Console.WriteLine($"ValueAccessor after await FooAsync in Main: {_valueAccessor.Value}");
}

private static async Task FooAsync()
{
    _valueAccessor.Value = "B";
    Console.WriteLine($"ValueAccessor before await in FooAsync: {_valueAccessor.Value}");
    await Task.Delay(100);
    Console.WriteLine($"ValueAccessor after await in FooAsync: {_valueAccessor.Value}");
}
#endregion

  
}

interface IValueAccessor<T> {
    T Value { get; set; }
}
class ValueAccessor<T> : IValueAccessor<T> {
    private static AsyncLocal<ValueHolder<T>> _asyncLocal = new AsyncLocal<ValueHolder<T>>();

    public T Value
    {
        get => _asyncLocal.Value != null ? _asyncLocal.Value.Value : default;
        set {
            var holder = _asyncLocal.Value;
            if (holder != null) {
                holder.Value = default;
            }

            if (value != null) {
                _asyncLocal.Value = new ValueHolder<T>
                { Value = value };
            }
            // _asyncLocal.Value ??= new ValueHolder<T>();
            //
            // _asyncLocal.Value.Value = value;
        }
    }
}
class ValueHolder<T>
{
    public T Value { get; set; }
}