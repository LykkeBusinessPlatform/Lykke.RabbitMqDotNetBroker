using System.Reflection;

namespace Lykke.RabbitMqBroker;

internal class HostAssemblyDisplayName : AssemblyDisplayName
{
    public HostAssemblyDisplayName() : base(Assembly.GetEntryAssembly())
    {
    }

    public override string ToString()
    {
        return $"{Title} {Version}";
    }

    public static implicit operator string(HostAssemblyDisplayName displayName)
    {
        return displayName.ToString();
    }
}