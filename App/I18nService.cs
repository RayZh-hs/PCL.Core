using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace PCL.Core.App;

public static class I18nService
{
    private static Dictionary<string, string> _translations = new();
    public static string CurrentLanguage { get; private set; } = "zh-CN";

    public static void Initialize()
    {
        CurrentLanguage = Config.Language;
        LoadLanguage(CurrentLanguage);
    }

    public static void LoadLanguage(string languageCode)
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Languages", $"{languageCode}.json");
        if (File.Exists(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath);
                _translations = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new();
            }
            catch (Exception ex)
            {
                // Log error
                System.Diagnostics.Debug.WriteLine($"Failed to load language {languageCode}: {ex.Message}");
            }
        }
        else
        {
             System.Diagnostics.Debug.WriteLine($"Language file not found: {filePath}");
        }
    }

    public static string Get(string key)
    {
        return _translations.TryGetValue(key, out var value) ? value : key;
    }
}
