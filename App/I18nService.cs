using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.ComponentModel;
using PCL.Core.Logging;

namespace PCL.Core.App;

public static class I18nService
{
    private static Dictionary<string, string> _translations = new();
    private static Dictionary<string, string> _fallbackTranslations = new(); // Default to zh-CN as fallback
    public static string CurrentLanguage { get; private set; } = "zh-CN";
    
    public static event EventHandler LanguageChanged;

    public static void Initialize()
    {
        CurrentLanguage = Config.Language;
        if (string.IsNullOrWhiteSpace(CurrentLanguage))
        {
            CurrentLanguage = "zh-CN";
        }
        LogService.Logger?.Info($"I18nService initializing with language: {CurrentLanguage}");
        
        // Load fallback language (zh-CN) first
        LoadFallbackLanguage();
        
        // Load selected language
        LoadLanguage(CurrentLanguage);
    }

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true
    };

    private static void LoadFallbackLanguage()
    {
        string fallbackPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Languages", "zh-CN.json");
        LogService.Logger?.Info($"Loading fallback language from: {fallbackPath}");
        if (File.Exists(fallbackPath))
        {
            try
            {
                string json = File.ReadAllText(fallbackPath);
                _fallbackTranslations = JsonSerializer.Deserialize<Dictionary<string, string>>(json, _jsonOptions) ?? new();
                LogService.Logger?.Info($"Loaded {_fallbackTranslations.Count} keys for fallback language.");
            }
            catch (Exception ex)
            {
                LogService.Logger?.Error($"Failed to load fallback language: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Failed to load fallback language: {ex.Message}");
            }
        }
        else
        {
            LogService.Logger?.Warn($"Fallback language file not found: {fallbackPath}");
        }
    }

    public static void LoadLanguage(string languageCode)
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Languages", $"{languageCode}.json");
        LogService.Logger?.Info($"Loading language {languageCode} from: {filePath}");
        if (File.Exists(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath);
                _translations = JsonSerializer.Deserialize<Dictionary<string, string>>(json, _jsonOptions) ?? new();
                CurrentLanguage = languageCode;
                LogService.Logger?.Info($"Loaded {_translations.Count} keys for language {languageCode}.");
                
                // Notify all listeners that language has changed
                LanguageChanged?.Invoke(null, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                // Log error and fall back to default
                LogService.Logger?.Error($"Failed to load language {languageCode}: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Failed to load language {languageCode}: {ex.Message}");
                _translations = new Dictionary<string, string>(_fallbackTranslations);
            }
        }
        else
        {
            LogService.Logger?.Warn($"Language file not found: {filePath}");
            System.Diagnostics.Debug.WriteLine($"Language file not found: {filePath}");
            // Fall back to default language
            _translations = new Dictionary<string, string>(_fallbackTranslations);
        }
    }

    public static string Get(string key)
    {
        // Try to get from current language
        if (_translations.TryGetValue(key, out var value))
        {
            return value;
        }
        
        // Fall back to default language
        if (_fallbackTranslations.TryGetValue(key, out var fallbackValue))
        {
            return fallbackValue;
        }
        
        // If not found in either, return the key itself
        return key;
    }
}

public class TranslationNotifier : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public void Refresh()
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
    }
}
