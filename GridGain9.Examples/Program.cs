using Apache.Ignite;
using Apache.Ignite.Compute;
using Apache.Ignite.Table;
using GridGain9.ComputeTester;
using GridGain9.ComputeTester.Jobs;
Console.WriteLine(">>> Starting .NET Compute Examples");

await ManagementApi.ActivateCluster("/home/pavel/Downloads/gridgain-license.json");
Console.WriteLine("Cluster activated");

var client = await IgniteClient.StartAsync(new("localhost:10800"));
Console.WriteLine($"\nClient connected: {client}");

// Deploy the GridGain9.Examples.Jobs assembly to the cluster.
// It will be undeployed automatically on exit.
// => we can modify the jobs and simply re-run the program to test the changes.
await using var deploymentUnit = await ManagementApi.DeployAssembly(typeof(HelloJob).Assembly);
Console.WriteLine($"\nJobs assembly deployed to the cluster: {deploymentUnit}");

await ExecuteHelloJob(client, deploymentUnit.Unit);

await RunStreamerWithReceiver(client, deploymentUnit.Unit);

static async Task ExecuteHelloJob(IIgniteClient client, DeploymentUnit unit)
{
    Console.WriteLine("\n\n>>> Starting .NET compute example");

    // JobDescriptor.Of sets ExecutorType = JobExecutorType.DotNetSidecar
    JobDescriptor<string, string> helloJobDesc = JobDescriptor.Of(new HelloJob()) with
    {
        DeploymentUnits = [unit]
    };

    Console.WriteLine($"\nJob descriptor created: {helloJobDesc}");

    var nodes = await client.GetClusterNodesAsync();
    var jobTarget = JobTarget.AnyNode(nodes);

    var jobExec = await client.Compute.SubmitAsync(jobTarget, helloJobDesc, arg: "World");
    var jobResult = await jobExec.GetResultAsync();

    Console.WriteLine($"\nJob executed: '{jobResult}'");
}

static async Task RunStreamerWithReceiver(IIgniteClient client, DeploymentUnit unit)
{
    Console.WriteLine("\n\n>>> Starting .NET streamer receiver example");

    // Create a table to stream data.
    await client.Sql.ExecuteScriptAsync(
        "CREATE TABLE IF NOT EXISTS Test (Id INT PRIMARY KEY, Name VARCHAR); " +
        "DELETE FROM Test;");

    var table = await client.Tables.GetTableAsync("Test");
    IRecordView<IIgniteTuple> tableView = table!.RecordBinaryView;

    Console.WriteLine($"\nTable created: {table}");

    // Prepare streamer receiver.
    ReceiverDescriptor<int, string, IIgniteTuple> receiverDesc = ReceiverDescriptor.Of(new TestReceiver()) with
    {
        DeploymentUnits = [unit]
    };

    Console.WriteLine($"\nReceiver descriptor created: {receiverDesc}");

    // Stream data.
    int[] ids = [1, 2, 3, 4, 5];
    IAsyncEnumerable<int> data = ids.ToAsyncEnumerable();

    IAsyncEnumerable<IIgniteTuple> streamerResults = tableView.StreamDataAsync(
        data: data,
        receiver: receiverDesc,
        keySelector: id => new IgniteTuple { ["ID"] = id },
        payloadSelector: id => id,
        receiverArg: table.Name);

    Console.WriteLine($"\nResults from receiver:");
    await foreach (var item in streamerResults)
    {
        Console.WriteLine(item);
    }
}
