# GridGain 9 .NET Client Examples 

# Start a GridGain node in Docker

Expose ports - 10300 for REST, 10800 for client:

```bash
docker run -p 10300:10300 -p 10800:10800 gridgain/gridgain9:9.1.2
```

# Activate the GridGain Cluster

* `ManagementApi.ActivateCluster` call in `Program.cs` will activate the cluster automatically.
* Cluster activation may take time - re-run the program if it fails with client connection error.

Alternatively, use the [CLI tool](https://www.gridgain.com/docs/gridgain9/latest/ignite-cli-tool).

# Run the API tester

```bash
dotnet run GridGain9.Examples.ApiTester
```
