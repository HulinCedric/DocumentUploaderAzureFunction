using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using Azure.Storage.Blobs;
using JetBrains.Annotations;

namespace DocumentUploader.Setup;

[UsedImplicitly]
public class AzureFunctionFixture : IDisposable
{
    private const int Port = 7071;
    private readonly Process funcHostProcess;

    public AzureFunctionFixture()
    {
        var dotnetExePath = Environment.ExpandEnvironmentVariables(ConfigurationHelper.Settings.DotNetExecutablePath!);
        var functionHostPath = Environment.ExpandEnvironmentVariables(ConfigurationHelper.Settings.FunctionHostPath!);
        var functionAppFolder = Path.GetRelativePath(
            Directory.GetCurrentDirectory(),
            ConfigurationHelper.Settings.FunctionApplicationPath!);

        funcHostProcess = new Process
        {
            StartInfo =
            {
                FileName = dotnetExePath,
                Arguments = $"\"{functionHostPath}\" start -p {Port}",
                WorkingDirectory = functionAppFolder
            }
        };

        var success = funcHostProcess.Start();
        if (!success)
            throw new InvalidOperationException("Could not start Azure Functions host.");

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ||
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            funcHostProcess.WaitForExit(10000);

        Client = new HttpClient
        {
            BaseAddress = new Uri($"http://localhost:{Port}")
        };

        var documentsContainer = new BlobServiceClient(ConfigurationHelper.Settings.StorageConnectionString)
            .GetBlobContainerClient(UploadDocumentFunction.DocumentContainerName);

        documentsContainer.CreateIfNotExists();
    }

    public HttpClient Client { get; }

    public virtual void Dispose()
    {
        if (!funcHostProcess.HasExited)
            funcHostProcess.Kill();

        funcHostProcess.Dispose();
        Client.Dispose();
    }
}