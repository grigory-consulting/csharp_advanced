using System.Buffers;
using System.Diagnostics;

class Program
{

    const int BufferSize = 200_000;
    const int Iterations = 200_000;


    static void Main()
    {

        //AllocateEachTime(100);
        //Pooling(100);

        //ForceGC();
        Run("AllocateEachTime", AllocateEachTime);

        //ForceGC();
        Run("Pooling", Pooling);
    }

    static void AllocateEachTime(int iterations)
    {
        for (int i = 0; i < iterations; i++)
        {

            byte[] buf = new byte[BufferSize];
            buf[0] = 1;

            unsafe
            {
                fixed (byte* ptr = buf)
                {

                    {
                        SimulateNativeUse(ptr, BufferSize);
                    }
                }

            }
        }
    }
    static void Pooling(int iterations)
    {
        var pool = ArrayPool<byte>.Shared;

        for (int i = 0; i < iterations; i++)
        {
            byte[] buf = pool.Rent(BufferSize);

            try
            {
                buf[0] = 1;
                unsafe
                {
                    fixed (byte* ptr = buf)
                    {

                        {
                            SimulateNativeUse(ptr, BufferSize);
                        }
                    }

                }
            }
            finally
            {
                pool.Return(buf, clearArray: false);
            }

        } 
    }
    private static unsafe void SimulateNativeUse(byte* ptr, int bufferSize)
    {
        Process(ptr[0]);
    }
    private static void Process(byte b) { }

    static void Run(string name, Action<int> action)
    {
        var sw = Stopwatch.StartNew();
        long allocBefore = GC.GetAllocatedBytesForCurrentThread();
        action(Iterations);
        sw.Stop();
        long allocAfter = GC.GetAllocatedBytesForCurrentThread();
        Console.WriteLine($"Name: {name} | {sw.ElapsedMilliseconds} ms | Allocated {allocAfter - allocBefore} bytes.");
    }

    private static void ForceGC()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }


}
