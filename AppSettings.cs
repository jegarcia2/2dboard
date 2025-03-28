using System.IO;
using System.Text.Json;
class AppSettings
{
    public string Language { get; set; } = "en"; // Default language
    private static readonly string ConfigFile = "./settings.config";

    public static AppSettings LoadSettings()
    {
        if (!File.Exists(ConfigFile))
            return new AppSettings(); // Return default settings if file doesn't exist

        string json = File.ReadAllText(ConfigFile);
        
        return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
    }

    public void SaveSettings()
    {
        string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(ConfigFile, json);
    }
}
