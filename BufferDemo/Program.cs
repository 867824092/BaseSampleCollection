using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;

{
    // source data:  00 01 02 03 04
    // binary data:  00000000 00000001 00000010 00000011 000001000
    byte[] bytes = new byte[] { 0,1, 2, 3, 4 };
    int value = BinaryPrimitives.ReadInt32LittleEndian(bytes);
    Console.WriteLine(value);  // 16909060
}
{
    byte[] data = { 1, 2, 3, 4, 5 };
    ReadOnlyMemory<byte> memory =  data;
    foreach (var b in memory.Span) {
        Console.WriteLine(b);
    }
    ReadOnlySequence<byte> sequence = new ReadOnlySequence<byte>(memory);
    ReadOnlyMemory<byte> memory1 = sequence.First;
    SequenceReader<byte> reader = new SequenceReader<byte>(sequence);
}

{
    ReadOnlySpan<char> readOnlySpan = "This is a sample data for testing purposes.";
    int index = readOnlySpan.IndexOf(' ');
    var data = ((index < 0) ?
        readOnlySpan : readOnlySpan.Slice(0, index)).ToArray();
    Console.WriteLine(data);
}
{
// 创建一个数组并使用 Memory<T> 包装它
    var array = new int[] { 1, 2, 3, 4, 5 };
    Memory<int> memory = array;

    // 读取数组中的第三个元素
    int third = memory.Span[2];
    Console.WriteLine($"Third element is {third}.");

    // 修改数组中的第三个元素
    memory.Span[2] = 10;
    Console.WriteLine($"Third element is now {array[2]}.");
}

{
    IntPtr ptr = Marshal.AllocHGlobal(1);
    Marshal.WriteInt16(ptr,45);

    Console.WriteLine(Marshal.ReadInt32(ptr).ToString());
    
}
