using System.Diagnostics.Metrics;
using System.Threading;

namespace ThreadingTutorialLive
{
    public class LockVsInterlocked
    {

        static void Main()
        {

            int n = 100000;
            object locker = new();

            int counter = 0;
            counter = 0;
            Parallel.For(0, n, i => { counter++; });
            Console.WriteLine($"Race: counter={counter} (expect {n})");

            counter = 0;
            Parallel.For(0, n, i => { lock (locker) { counter++; } });
            Console.WriteLine($"Lock: counter={counter} (expect {n})");

            counter = 0;
            Parallel.For(0, n, i => { Interlocked.Increment(ref counter); });
            Console.WriteLine($"Interlocked: counter={counter} (expect {n})");


        }


    }
}



