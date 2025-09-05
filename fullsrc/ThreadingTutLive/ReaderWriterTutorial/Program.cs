

class Program
{
    static double value = 1.00;
    static ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

    static void Main()
    {

        Task.Run(() =>
        {
            for (int i = 0; i <= 3; i++)
            {
                
                _lock.EnterWriteLock();
                try
                {
                    value += 0.01 * i;
                    Console.WriteLine("Value updated " + value);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }

                Thread.Sleep(500); // delay
            }
        });

        Parallel.For(0, 30, readerId =>
        {
            _lock.EnterReadLock();

            try
            {
                Console.WriteLine($"Reader {readerId}, Value is {value}");
            }
            finally
            {
                _lock.ExitReadLock();
            }
            Thread.Sleep(300);
        });



    }
}