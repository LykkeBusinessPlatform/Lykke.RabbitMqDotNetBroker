using System.Reflection;

namespace Lykke.RabbitMqBroker;

internal class LibraryAssemblyDisplayName : AssemblyDisplayName
{
    public LibraryAssemblyDisplayName() : base(Assembly.GetAssembly(typeof(ConnectionAssemblyBasedDisplayName)))
    {
    }

    public override string ToString()
    {
        return $"{Title} {Version}";
    }
}