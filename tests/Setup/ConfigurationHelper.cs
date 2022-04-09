using Microsoft.Extensions.Configuration;

namespace DocumentUploader.Setup;

public static class ConfigurationHelper
{
    private static Settings? settings;

    public static Settings Settings
    {
        get
        {
            if (settings != null)
                return settings;

            var root = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var buildSettings = new Settings();
            root.Bind(buildSettings);

            settings = buildSettings;

            return settings;
        }
    }
}