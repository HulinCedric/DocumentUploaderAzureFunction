namespace DocumentUploader.Setup;

public class Settings
{
    public string? DotNetExecutablePath { get; set; } = default;
    public string? FunctionApplicationPath { get; set; } = default;
    public string? FunctionHostPath { get; set; } = default;
    public string? StorageConnectionString { get; set; } = default;
}