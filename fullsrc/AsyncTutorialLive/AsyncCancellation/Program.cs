


using System.Runtime.CompilerServices;

class Program
{

    static async Task Main()
    {
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(4));
        var ctsEnter = new CancellationTokenSource();
        var linkedcts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, ctsEnter.Token);
        // Cancel after 6 sec 
        //await Task.Delay(6000);
        //cts.Cancel();

        using (linkedcts)
        {
            var CountingTask = AsyncMethod(linkedcts.Token);

            Task.Run(() =>
            {
                Console.WriteLine("Press Enter to exit:...");
                Console.ReadLine();// Check whether I pressed enter
                ctsEnter.Cancel();
                Console.WriteLine("User cancelled");
            }
            );

            await CountingTask;
        }

    }


    static async Task AsyncMethod(CancellationToken token)
    {
        try
        {
            for (int i = 0; i < 10; i++)
            {
                // do something 

                token.ThrowIfCancellationRequested();
                await Task.Delay(1000, token);
                Console.WriteLine($"Step {i}");

            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine($"Task was cancelled.");
        }

    }


}