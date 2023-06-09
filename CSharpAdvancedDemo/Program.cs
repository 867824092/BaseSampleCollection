// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");
using System.Runtime.InteropServices;
using CSharpAdvancedDemo;

class Program {
    static unsafe void Main(string[] args) {
        {
            // Demonstrate how to call GlobalAlloc and
            // GlobalFree using the Marshal class.
            IntPtr hglobal = Marshal.AllocHGlobal(100); 
            Marshal.WriteInt32(hglobal,20);
            Console.WriteLine(Marshal.ReadInt32(hglobal));
            Marshal.FreeHGlobal(hglobal);
        }
        {
            Span<int> numbers = stackalloc[]
            { 1, 2, 3, 4, 5, 6 };
            var ind = numbers.IndexOfAny(stackalloc[]
            { 2, 4, 6, 8 });
            Console.WriteLine(ind); // output: 1
        }

        {
            int length = 3;
            int[] numbers = new int[length];
            for (int i = 0; i < length; i++) {
                numbers[i] = i;
            }

            fixed (int* ptr = &numbers[1]) {
                Console.WriteLine(*ptr);
            }
        }
        {
            Coords coords;
            Coords* p = &coords;
            p->X = 3;
            p->Y = 4;
            Console.WriteLine(p->ToString()); // output: (3, 4)
        }
        {
            int num = 10;
            int* p = &num;
            int** p1 = &p;
            Console.WriteLine((long)p);
            Console.WriteLine((long)p1);
            int*[] prt = new int*[]
            { &num };
            Console.WriteLine(*prt[0]);
            
            object obj = new object();
            object* pObj = &obj;
            var foo = new Foo() {Bar = 1 };
            Foo* foo1 = &foo;
            fixed (int* pp = &foo1->Bar) {
                Console.WriteLine(*pp);
            }
           
            Console.WriteLine(foo1->Bar);
        }
        {
            int* numbers = stackalloc int[]
            { 10, 20, 30, 40, 50 };
            for (int i = 0; i < 5; i++) {
                int* p = &numbers[i];
                Console.WriteLine((int)p);
            }
        }
        {
            // Demonstrate the use of public static fields of the Marshal class.
            Console.WriteLine("SystemDefaultCharSize={0}, SystemMaxDBCSCharSize={1}",
                Marshal.SystemDefaultCharSize, Marshal.SystemMaxDBCSCharSize);

            // Demonstrate the use of the SizeOf method of the Marshal class.
            Console.WriteLine("Number of bytes needed by a Point object: {0}",
                Marshal.SizeOf(typeof(Point)));
            Point p = new Point();
            Console.WriteLine("Number of bytes needed by a Point object: {0}",
                Marshal.SizeOf(p));

            // Demonstrate how to call GlobalAlloc and
            // GlobalFree using the Marshal class.
            IntPtr hglobal = Marshal.AllocHGlobal(100);
            Marshal.FreeHGlobal(hglobal);

            // Demonstrate how to use the Marshal class to get the Win32 error
            // code when a Win32 method fails.
            Boolean f = CloseHandle(new IntPtr(-1));
            if (!f) {
                Console.WriteLine("CloseHandle call failed with an error code of: {0}",
                    Marshal.GetLastWin32Error());
            }
        }
        {

            {
                uint a = 0b_0000_1111_0000_1111_0000_1111_0000_1100;
                uint b = ~a;
                Console.WriteLine(Convert.ToString(b, toBase: 2));
            }
            {
                uint a = 0b_1111_1000;
                uint b = 0b_1001_1101;
                uint c = a & b;
                Console.WriteLine(Convert.ToString(c, toBase: 2));
            }
            {
                uint a = 0b_1111_1000;
                uint b = 0b_0001_1111;
                uint c = a ^ b;
                Console.WriteLine(Convert.ToString(c, toBase: 2));
            }
            {
                uint a = 0b_1010_0000;
                uint b = 0b_1001_0001;
                uint c = a | b;
                Console.WriteLine(Convert.ToString(c, toBase: 2));
            }
            {
                uint a = 0b_1101;
                uint b = 0b_1001;
                uint c = 0b_1010;

                uint d1 = a | b & c;
                Display(d1); // output: 1101
                // a | b 1101
                // 1101 & c  1000
                uint d2 = (a | b) & c;
                Display(d2); // output: 1000

                void Display(uint x) => Console.WriteLine($"{Convert.ToString(x, toBase: 2),4}");
            }
        }
        {
            DynamicDemo.Run();
        }

// This is a platform invoke prototype. SetLastError is true, which allows
// the GetLastWin32Error method of the Marshal class to work correctly.
        [DllImport("Kernel32", ExactSpelling = true, SetLastError = true)]
        static extern Boolean CloseHandle(IntPtr h);
    }
}


public struct Coords {
    public int X;
    public int Y;
    public override string ToString() => $"({X}, {Y})";
}
public struct Point {
    public Int32 x, y;
}

public  class Foo
{
    public int Bar;
}


