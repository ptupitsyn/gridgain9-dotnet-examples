using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using Apache.Ignite.Compute;

namespace GridGain9.ComputeTester;

/// <summary>
/// REST API wrapper - see https://www.gridgain.com/docs/gridgain9/latest/developers-guide/rest/rest-api.
/// </summary>
public static class ManagementApi
{
    private const string BaseUri = "http://localhost:10300";

    public static async Task ActivateCluster(string licenseFilePath)
    {
        using var client = new HttpClient();

        var request = new
        {
            metaStorageNodes = new[] { "defaultNode" },
            clusterName = "myCluster",
            license = await File.ReadAllTextAsync(licenseFilePath)
        };

        using var response = await client.PostAsync(
            $"{BaseUri}/management/v1/cluster/init",
            JsonContent.Create(request));

        await EnsureSuccess(response);
    }

    public static async Task<DisposableDeploymentUnit> DeployUnit(string unitId, string unitVersion, IList<string> unitContent)
    {
        var url = GetUnitUrl(unitId, unitVersion);

        var content = new MultipartFormDataContent();
        foreach (var file in unitContent)
        {
            // HttpClient will close the file.
            var fileContent = new StreamContent(File.OpenRead(file));
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            content.Add(fileContent, "unitContent", fileName: Path.GetFileName(file));
        }

        var request = new HttpRequestMessage(HttpMethod.Post, url.ToString())
        {
            Content = content
        };

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var client = new HttpClient();
        HttpResponseMessage response = await client.SendAsync(request);

        await EnsureSuccess(response);

        return new DisposableDeploymentUnit(new DeploymentUnit(unitId, unitVersion));
    }

    public static async Task UndeployUnit(DeploymentUnit unit)
    {
        using var client = new HttpClient();
        await client.DeleteAsync(GetUnitUrl(unit.Name, unit.Version).Uri);
    }

    public static async Task<DisposableDeploymentUnit> DeployAssembly(
        Assembly asm,
        string? unitId = null,
        string? unitVersion = null)
    {
        var unitId0 = unitId ?? "temporary_unit";
        var unitVersion0 = unitVersion ?? DateTime.Now.TimeOfDay.ToString(@"m\.s\.f");

        return await DeployUnit(
            unitId: unitId0,
            unitVersion: unitVersion0,
            unitContent: [asm.Location]);
    }

    private static UriBuilder GetUnitUrl(string unitId, string unitVersion) =>
        new(BaseUri)
        {
            Path = $"/management/v1/deployment/units/{Uri.EscapeDataString(unitId)}/{Uri.EscapeDataString(unitVersion)}"
        };

    private static async Task EnsureSuccess(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        string resContent = await response.Content.ReadAsStringAsync();

        throw new Exception($"Request failed [status={response.StatusCode}, response={resContent}]");
    }
}
