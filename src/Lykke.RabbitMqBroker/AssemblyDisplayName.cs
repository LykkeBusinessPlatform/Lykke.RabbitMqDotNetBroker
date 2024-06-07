using System.Reflection;

namespace Lykke.RabbitMqBroker;

internal abstract class AssemblyDisplayName
{
    protected AssemblyDisplayName(Assembly assembly)
    {
        var name = assembly.GetName();
        Title = name.Name;
        Version = name.Version?.ToString();
    }
    
    public string Title { get; }
    public string Version { get; }
}