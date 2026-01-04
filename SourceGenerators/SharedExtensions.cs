using Microsoft.CodeAnalysis.CSharp;

namespace PCL.Core.SourceGenerators;

public static class SharedExtensions
{
    public static string ToLiteral(this string str)
    {
        // SymbolDisplay.FormatLiteral 是 Roslyn (Microsoft.CodeAnalysis) 提供的工具
        return SymbolDisplay.FormatLiteral(str, true);
    }
}
