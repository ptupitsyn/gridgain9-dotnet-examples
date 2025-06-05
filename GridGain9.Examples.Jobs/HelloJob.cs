using Apache.Ignite.Compute;

namespace GridGain9.ComputeTester.Jobs;

public class HelloJob : IComputeJob<string, string>
{
    public async ValueTask<string> ExecuteAsync(IJobExecutionContext context, string arg, CancellationToken cancellationToken)
    {
        await Task.Delay(1);

        return $".NET job: Hello, {arg}!";
    }
}
