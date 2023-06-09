{ // 补位运算
    uint a = 0b_0000_1111_0000_1111_0000_1111_0000_1100;
    uint b = ~a;
    Console.WriteLine(Convert.ToString(b, toBase: 2));
    // Output:
    // 11110000111100001111000011110011
}

{ //左移
    uint x = 0b_1100_1001_0000_0000_0000_0000_0001_0001;
    Console.WriteLine($"Before: {Convert.ToString(x, toBase: 2)}");

    uint y = x << 4;
    Console.WriteLine($"After:  {Convert.ToString(y, toBase: 2)}");
}

// Output:
// Before: 11001001000000000000000000010001
// After:  10010000000000000000000100010000
{ //右移
    uint x = 0b_1001;
    Console.WriteLine($"Before: {Convert.ToString(x, toBase: 2),4}");

    uint y = x >> 2;
    Console.WriteLine($"After:  {Convert.ToString(y, toBase: 2).PadLeft(4, '0'),4}");
    // Output:
    // Before: 1001
    // After:  0010

    {
        int a = int.MinValue;
        Console.WriteLine($"Before: {Convert.ToString(a, toBase: 2)}");

        int b = a >> 3;
        Console.WriteLine($"After:  {Convert.ToString(b, toBase: 2)}");
        // Output:
        // Before: 10000000000000000000000000000000
        // After:  11110000000000000000000000000000
    }
    {
        uint c = 0b_1000_0000_0000_0000_0000_0000_0000_0000;
        Console.WriteLine($"Before: {Convert.ToString(c, toBase: 2),32}");

        uint d = c >> 3;
        Console.WriteLine($"After:  {Convert.ToString(d, toBase: 2).PadLeft(32, '0'),32}");
        // Output:
        // Before: 10000000000000000000000000000000
        // After:  00010000000000000000000000000000
    }
} { // 逻辑与
    uint a = 0b_1111_1000;
    uint b = 0b_1001_1101;
    uint c = a & b;
    Console.WriteLine(Convert.ToString(c, toBase: 2));
    // Output:
    // 10011000
} { //逻辑异或
    uint a = 0b_1111_1000;
    uint b = 0b_0001_1100;
    uint c = a ^ b;
    Console.WriteLine(Convert.ToString(c, toBase: 2));
    // Output:
    // 11100100
} { //逻辑或
    uint a = 0b_1010_0000;
    uint b = 0b_1001_0001;
    uint c = a | b;
    Console.WriteLine(Convert.ToString(c, toBase: 2));
    // Output:
    // 10110001
}

