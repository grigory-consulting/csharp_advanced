using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct SequentialStruct
{
    public byte A;
    public int B;
    public short C;
}
[StructLayout(LayoutKind.Explicit)]
public struct ExplicitStruct
{
    [FieldOffset(0)] public byte A;
    [FieldOffset(1)] public int B;
    [FieldOffset(5)] public short C;
}


class Program
{
    static void Main()
    { 
        Console.WriteLine($"SequentialStruct size {Marshal.SizeOf< SequentialStruct>()}");
        Console.WriteLine($"ExplicitStruct size {Marshal.SizeOf<ExplicitStruct>()}");
        var s = new SequentialStruct { A = 0x11, B = 0x22334455, C = 6677 };
        var e = new ExplicitStruct { A = 0x11, B = 0x22334455, C = 6677 };
        DumpBytes(s);
        DumpBytes(e);

    }

    static void DumpBytes<T>(T obj) where T : struct
    {
        int size = Marshal.SizeOf<T>();
        byte[] buffer = new byte[size];
        IntPtr ptr = Marshal.AllocHGlobal(size);
        try
        {
            Marshal.StructureToPtr(obj, ptr, false); // false is destuctor
            Marshal.Copy(ptr, buffer, 0, size); // 
            //Console.WriteLine(Convert.ToHexString(buffer));

            for (int i = 0; i < size; i++)
            {
                Console.Write($"{buffer[i]:X2} ");
            }
            Console.WriteLine();
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }

    }
}
