using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

[InvocationCount(1)]
[IterationCount(10)]
public class TaskVsValueTaskBench
{
    public Task<int> GetTaskAsync()
    {
        return Task.FromResult(20); // allways allocates Task<int> and it is heap allocation
    }

    public ValueTask<int> GetValueTaskAsync()
    {
        return new ValueTask<int>(1); // no heap allocation
    }

    [Benchmark]
    public async Task TaskBenchmark()
    {
        for (int i = 0; i < 1000_000_000; i++)
        {
            int result = await GetTaskAsync();
        }
    }


    [Benchmark]
    public async Task ValueTaskBenchmark()
    {
        for (int i = 0; i < 1000_000_000; i++)
        {
            int result = await GetValueTaskAsync();
        }
    }
}

public class Program
{
    public static void Main()
    {
        BenchmarkRunner.Run<TaskVsValueTaskBench>();
    }

}