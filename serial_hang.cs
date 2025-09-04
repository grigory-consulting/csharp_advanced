using System.Diagnostics;
using System.IO.Ports;
using System.Text;

internal class Program
{
    const string SEND_PORT = "COM1";
    const string RECEIVE_PORT = "COM2";
    const int BAUD_RATE = 921_600;
    const int BYTES = 1_048_576;
    const int CHUNK_SIZE = 16_384;


    static async Task Main(string[] args)
    {
        string payload = new string('F', BYTES);
        byte[] data = Encoding.ASCII.GetBytes(payload);

        using SerialPort sender = new SerialPort(SEND_PORT, BAUD_RATE);
        using SerialPort receiver = new SerialPort(RECEIVE_PORT, BAUD_RATE);

        long received = 0;

        sender.Open();
        receiver.Open();

        Task receiverTask = Task.Run(async () => 
        {
            var buf = new byte[2 * CHUNK_SIZE];
            while (received < BYTES)
            {
                int n = await receiver.BaseStream.ReadAsync(buf);
                if (n > 0)
                {
                    received += n;
                }
            }
        });
        Console.WriteLine("1");
        Stopwatch sw = Stopwatch.StartNew();

        int offset = 0;

        while (offset < data.Length)
        {
            int chunkToWrite = Math.Min(data.Length - offset, CHUNK_SIZE);
            await sender.BaseStream.WriteAsync(data, offset, chunkToWrite);
            offset += chunkToWrite;
            Console.WriteLine($"wrote chunk {chunkToWrite:N0} with offset {offset:N0}");
        }

        Console.WriteLine("2");

        await receiverTask;

        Console.WriteLine("3");

        sw.Stop();
        Console.WriteLine($"took: {sw.ElapsedMilliseconds} ms");
        Console.WriteLine($"bytes/s: {data.Length/sw.Elapsed.TotalSeconds}");
    }
}
