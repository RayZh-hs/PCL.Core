using System;
using System.IO;
using System.Text.Json;

namespace PCL.Core.App;

// Simple test class to verify i18n functionality
public static class I18nServiceTest
{
    public static void RunTests()
    {
        System.Diagnostics.Debug.WriteLine("=== Running I18nService Tests ===");
        
        // Test 1: Verify language files exist
        string zhCnPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Languages", "zh-CN.json");
        string enUsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Languages", "en-US.json");
        
        System.Diagnostics.Debug.WriteLine($"zh-CN.json exists: {File.Exists(zhCnPath)}");
        System.Diagnostics.Debug.WriteLine($"en-US.json exists: {File.Exists(enUsPath)}");
        
        // Test 2: Verify language files can be loaded
        try
        {
            // Initialize service
            I18nService.Initialize();
            
            // Test 3: Test translation retrieval
            string langKey = I18nService.Get("Language");
            System.Diagnostics.Debug.WriteLine($"Current language for 'Language' key: {langKey}");
            
            // Test 4: Test fallback behavior
            string nonExistentKey = I18nService.Get("NonExistentKey");
            System.Diagnostics.Debug.WriteLine($"Non-existent key returns: {nonExistentKey}");
            
            // Test 5: Test language switching
            I18nService.LoadLanguage("en-US");
            string englishLang = I18nService.Get("Language");
            System.Diagnostics.Debug.WriteLine($"English 'Language' key: {englishLang}");
            
            I18nService.LoadLanguage("zh-CN");
            string chineseLang = I18nService.Get("Language");
            System.Diagnostics.Debug.WriteLine($"Chinese 'Language' key: {chineseLang}");
            
            // Test 6: Test missing language file behavior
            I18nService.LoadLanguage("fr-FR"); // This file doesn't exist
            string fallbackLang = I18nService.Get("Language");
            System.Diagnostics.Debug.WriteLine($"Fallback 'Language' key: {fallbackLang}");
            
            System.Diagnostics.Debug.WriteLine("=== I18nService Tests Completed ===");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"I18nService test failed: {ex.Message}");
        }
    }
}