using System.Runtime.InteropServices;

class Program
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int Callback(int val);

    [DllImport("NativeLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void RegisterCallback(Callback cb);

    [DllImport("NativeLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void TriggerLoop();

    private static Callback cb;

    static void Main()
    {
        
        cb = x =>
        {
            Console.WriteLine($"[C#] Got {x}, returning {x * x}");
            return x * x;
        };
        RegisterCallback(cb);
        TriggerLoop();
    }


}