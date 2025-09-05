
// Async Directory Hashing - Single Threaded

using System.Globalization;
using System.Security.Cryptography;

class AsyncDirHash
{

    static async Task Main(string[] args)
    {
        string folder = args.Length > 0 ? args[0] : Directory.GetCurrentDirectory(); ;// first argument, else Current Folder

        if (!Directory.Exists(folder))
        {
            Console.WriteLine($"Directory \"{folder}\" not found.");
            return;
        }

        const int maxTasks = 3;

        var files = Directory.GetFiles(folder);
        var active = new List<Task>();


        foreach (var file in files)
        {
            if (active.Count >= maxTasks)
            {
                var finished = await Task.WhenAny(active);
                active.Remove(finished);
            }
            active.Add(Task.Run(async () =>
            {
                try
                {
                    string hash = await HashAsync(file);
                    Console.WriteLine($"{file,-60} {hash}");
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
            }));

        }

        await Task.WhenAll(active);


    }

    private static async Task<string> HashAsync(string file)
    {
        using FileStream fs = File.OpenRead(file);
        using SHA256 sha = SHA256.Create();
        byte[] hashbytes = await sha.ComputeHashAsync(fs);
        return BitConverter.ToString(hashbytes).Replace("-", "").ToLowerInvariant();
    }
}

