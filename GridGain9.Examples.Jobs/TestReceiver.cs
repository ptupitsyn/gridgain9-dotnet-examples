using Apache.Ignite.Table;

namespace GridGain9.ComputeTester.Jobs;

public class TestReceiver : IDataStreamerReceiver<int, string, IIgniteTuple>
{
    public async ValueTask<IList<IIgniteTuple>?> ReceiveAsync(
        IList<int> page,
        string arg,
        IDataStreamerReceiverContext context,
        CancellationToken cancellationToken)
    {
        var table = await context.Ignite.Tables.GetTableAsync(arg);
        var view = table!.RecordBinaryView;
        var results = new List<IIgniteTuple>(page.Count);

        foreach (var id in page)
        {
            var tuple = new IgniteTuple
            {
                ["ID"] = id,
                ["NAME"] = $".NET receiver {id}"
            };

            await view.UpsertAsync(transaction: null, tuple);

            results.Add(tuple);
        }

        return results;
    }
}
