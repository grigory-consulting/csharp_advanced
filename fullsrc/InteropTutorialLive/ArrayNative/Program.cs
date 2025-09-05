using System.Runtime.InteropServices;


class Program
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void ArrayCallback(int* data, int length);

    [DllImport("NativeLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void RegisterArrayCallback(ArrayCallback cb);
    [DllImport("NativeLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void TriggerArrayCallback();

    private static ArrayCallback cb;

    static unsafe void Main()
    {
        cb = (int* data, int length) =>
        {
            Console.Write("[C#] Received array:");
            for (int i = 0; i < length; i++)
            {
                Console.Write($"{data[i]} "); // pointer indexing
                data[i] *= 2;                 // inplace modification 
            }
            Console.WriteLine();
        };
        RegisterArrayCallback(cb);
        TriggerArrayCallback();

    }

}