namespace Lykke.RabbitMqBroker;

/// <summary>
/// Display name for shared connections based on the assembly metadata.
/// </summary>
internal sealed class SharedConnectionAssemblyBasedDisplayName : ConnectionAssemblyBasedDisplayName
{
    private readonly string _value;
        
    public SharedConnectionAssemblyBasedDisplayName()
    {
        _value = BuildDisplayName("Shared connection");
    }
        
    public static implicit operator string(SharedConnectionAssemblyBasedDisplayName displayName)
    {
        return displayName._value;
    }
}
