using System.Diagnostics;
using System.IO.Ports;
using System.Text;


class Program
{
    const string S_PORT = "COM1";
    const string R_PORT = "COM2";
    const int BAUD = 921600;
    const int BYTES =   1024;
    const int CHUNK = 16 * 1024;

    static async Task Main()
    {
        string payload = new string('A', BYTES);
        byte[] data = Encoding.ASCII.GetBytes(payload);

        using var sender = new SerialPort(S_PORT, BAUD);
        using var receiver = new SerialPort(R_PORT, BAUD);

        sender.Open();
        receiver.Open();
        long received = 0;
        var receiverTask = Task.Run(async () =>
        {
            var buf = new byte[2 * CHUNK]; // your convention 
            while (received < BYTES)
            {
                int n = await receiver.BaseStream.ReadAsync(buf);
                if (n > 0) received += n;
            }
        });

        var sw = Stopwatch.StartNew();

        int offset = 0;

        while (offset < data.Length)
        {
            int chuncktoWrite = Math.Min(data.Length - offset, CHUNK);
            await sender.BaseStream.WriteAsync(data, offset, chuncktoWrite);
            offset += chuncktoWrite;
        }

        await receiverTask;
        sw.Stop();
        double seconds = sw.Elapsed.TotalSeconds;
        double bytesSec = BYTES / seconds;

        Console.WriteLine($"{bytesSec}");

    }


}