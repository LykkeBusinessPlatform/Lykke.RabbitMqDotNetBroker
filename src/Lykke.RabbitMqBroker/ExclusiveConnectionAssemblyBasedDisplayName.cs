namespace Lykke.RabbitMqBroker;

/// <summary>
/// Display name for exclusive connection based on the assembly metadata.
/// Can be customized with a custom name.
/// </summary>
internal sealed class ExclusiveConnectionAssemblyBasedDisplayName : ConnectionAssemblyBasedDisplayName
{
    private readonly string _value;
        
    public ExclusiveConnectionAssemblyBasedDisplayName(string customName = "")
    {
        var prefix = "Exclusive connection";
        if (!string.IsNullOrWhiteSpace(customName))
            prefix += $" {customName}";
        _value = BuildDisplayName(prefix);
    }
        
    public static implicit operator string(ExclusiveConnectionAssemblyBasedDisplayName displayName)
    {
        return displayName._value;
    }
}
