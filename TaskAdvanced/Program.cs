using System.Runtime.CompilerServices;
using TaskAdvanced;

{
    // AsyncLocal<string> AsyncLocal = new AsyncLocal<string>();
    // AsyncLocal.Value = "Hello World";
    //
    // Task.Run(() => {
    //         Thread.Sleep(1000);
    //         Console.WriteLine(
    //             $"Task1 Run, ThreadId: {Environment.CurrentManagedThreadId}, AsyncLocal: {AsyncLocal.Value}");
    //     })
    //     .GetAwaiter()
    //     .OnCompleted(() =>
    //         Console.WriteLine(
    //             $"Task1 OnCompleted, ThreadId: {Environment.CurrentManagedThreadId}, AsyncLocal: {AsyncLocal.Value}"));
    //
    // Task.Run(() => {
    //         Thread.Sleep(1000);
    //         Console.WriteLine(
    //             $"Task2 Run, ThreadId: {Environment.CurrentManagedThreadId}, AsyncLocal: {AsyncLocal.Value}");
    //     })
    //     .GetAwaiter()
    //     .UnsafeOnCompleted(() =>
    //         Console.WriteLine(
    //             $"Task2 UnsafeOnCompleted, ThreadId: {Environment.CurrentManagedThreadId}, AsyncLocal: {AsyncLocal.Value}"));
}

{
    // Task task1 = Task.Run(() => Console.WriteLine("hello"));
    // Task task2 = task1.ContinueWith(t => Console.WriteLine("world"));
}
{
    Task task = Task.Run(() => {
        Thread.Sleep(1000 * 100);
        Console.WriteLine("Hello");
    });
    TaskAwaiter awaiter1 = task.GetAwaiter();
    awaiter1.OnCompleted(() => Console.WriteLine("World"));

    // TaskAwaiter<string> awaiter2 = Task.Run(() => "Hello").GetAwaiter();
    // awaiter2.OnCompleted(() => Console.WriteLine($"{awaiter2.GetResult()} World"));
}
{
    Test test = new Test();
    test.Get().GetAwaiter().GetResult();
}
{
   await AsyncLocalDemo.Run();
}
public class Test {
    public async Task Get() {
        await Task.Delay(100);
    }
}
