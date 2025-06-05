using Apache.Ignite.Compute;

namespace GridGain9.ComputeTester;

public sealed record DisposableDeploymentUnit(DeploymentUnit Unit) : IAsyncDisposable
{
    public async ValueTask DisposeAsync() => await ManagementApi.UndeployUnit(Unit);
}
