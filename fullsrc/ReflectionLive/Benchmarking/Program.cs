

using System.Reflection;
using System.Diagnostics;

public class Data
{
    public int Value { get; set; }
}


class Program
{
    static void Main()
    {
        var data = new Data { Value = 999 };
        int N = 10_000_000;

        // direct
        var sw = Stopwatch.StartNew();

        int sum1 = 0;

        for (int i = 0; i < N; i++)
        {
            sum1 += data.Value;
        }

        sw.Stop();
        Console.WriteLine($"Direct: {sw.ElapsedMilliseconds} ms");

        // With Reflection 

        var value = typeof( Data ).GetProperty( "Value" ); 
        
        var swRefl = Stopwatch.StartNew();

        int sum2 = 0;

        for (int i = 0; i < N; i++)
        {
            sum2 += (int)value.GetValue(data);
        }

        swRefl.Stop();
        Console.WriteLine($"With Reflection: {swRefl.ElapsedMilliseconds} ms");


    }
}