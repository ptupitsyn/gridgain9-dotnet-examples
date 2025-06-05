# GridGain 9 .NET Client Examples 

# Start a GridGain Node in Docker

Expose ports - `10300` for REST, `10800` for client:

```bash
docker run -p 10300:10300 -p 10800:10800 gridgain/gridgain9:9.1.2
```

# Activate the GridGain Cluster

* `ManagementApi.ActivateCluster` call in `Program.cs` will activate the cluster automatically.
  * **Substitute your own license file path**
* Cluster activation may take time - re-run the program if it fails with client connection error.

Alternatively, use the [CLI tool](https://www.gridgain.com/docs/gridgain9/latest/ignite-cli-tool).

# Run the Examples

```bash
dotnet run GridGain9.Examples
```

The output should look like this:

```
>>> Starting .NET Compute Examples
Cluster activated

Client connected: IgniteClientInternal { Connections = [ ClusterNode { Id = 3281997b-e81e-4f95-adc1-51ac9ef084f1, Name = defaultNode, Address = 127.0.0.1:10800 } ] }

Jobs assembly deployed to the cluster: DisposableDeploymentUnit { Unit = DeploymentUnit { Name = temporary_unit, Version = 33.22.2 } }


>>> Starting .NET compute example

Job descriptor created: JobDescriptor { JobClassName = GridGain9.ComputeTester.Jobs.HelloJob, GridGain9.Examples.Jobs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null, DeploymentUnits = <>z__ReadOnlySingleElementList`1[Apache.Ignite.Compute.DeploymentUnit], Options = JobExecutionOptions { Priority = 0, MaxRetries = 0, ExecutorType = DotNetSidecar }, ArgMarshaller = , ResultMarshaller =  }

Job executed: '.NET job: Hello, World!'


>>> Starting .NET streamer receiver example

Table created: Table { Name = PUBLIC.TEST, Id = 20 }

Receiver descriptor created: ReceiverDescriptor { ReceiverClassName = GridGain9.ComputeTester.Jobs.TestReceiver, GridGain9.Examples.Jobs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null, DeploymentUnits = <>z__ReadOnlySingleElementList`1[Apache.Ignite.Compute.DeploymentUnit], Options = ReceiverExecutionOptions { Priority = 0, MaxRetries = 0, ExecutorType = DotNetSidecar } }

Results from receiver:
IgniteTuple { ID = 1, NAME = .NET receiver 1 }
IgniteTuple { ID = 2, NAME = .NET receiver 2 }
IgniteTuple { ID = 3, NAME = .NET receiver 3 }
IgniteTuple { ID = 4, NAME = .NET receiver 4 }
IgniteTuple { ID = 5, NAME = .NET receiver 5 }
```
