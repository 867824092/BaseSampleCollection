// See https://aka.ms/new-console-template for more information
using System.Runtime.InteropServices;

Console.WriteLine("Hello, World!"); {
    var array = new byte[100];

    var span = new Span<byte>(array);

    byte data = 0;

    for (int index = 0; index < span.Length; index++)

        span[index] = data++;



    int sum = 0;

    foreach (int value in array)
        sum += value;

    Console.WriteLine(sum);
} {
    var nativeMemory = Marshal.AllocHGlobal(100);

    Span<byte> span;

    unsafe {

        span = new Span<byte>(nativeMemory.ToPointer(), 100);

    }
    byte data = 0;

    for (int index = 0; index < span.Length; index++)

        span[index] = data++;



    int sum = 0;

    foreach (int value in span)

        sum += value;

    Console.WriteLine($"The sum of the numbers in the array is {sum}");

    Marshal.FreeHGlobal(nativeMemory);
}

