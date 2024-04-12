using System.Reflection;

namespace Lykke.RabbitMqBroker;

/// <summary>
/// Display name of the connection based on the assembly metadata (title and version)
/// </summary>
internal abstract class ConnectionAssemblyBasedDisplayName
{
    private string _assemblyTitle;
    private string _assemblyVersion;

    protected ConnectionAssemblyBasedDisplayName()
    {
        ExtractAssemblyMetadata();
    }
        
    private void ExtractAssemblyMetadata()
    {
        var assemblyName = Assembly.GetEntryAssembly()?.GetName();
            
        _assemblyTitle = assemblyName?.Name;
        _assemblyVersion = assemblyName?.Version?.ToString();
    }
        
    protected string BuildDisplayName(string prefix)
    {
        var result = prefix;
        if (!string.IsNullOrWhiteSpace(_assemblyTitle))
            result += $" from {_assemblyTitle}";
        if (!string.IsNullOrWhiteSpace(_assemblyVersion))
            result += $" of version {_assemblyVersion}";
            
        return result;
    }
}
