using System.Text;

namespace Lykke.RabbitMqBroker;

/// <summary>
/// Display name of the connection based on the assembly metadata (title and version)
/// and library assembly metadata (title and version).
/// </summary>
internal abstract class ConnectionAssemblyBasedDisplayName
{
    private readonly HostAssemblyDisplayName _hostAssemblyDisplayName = new ();
    private readonly LibraryAssemblyDisplayName _libraryAssemblyDisplayName = new ();

    protected string BuildDisplayName(string prefix)
    {
        var result = new StringBuilder(prefix);
        result.Append($" from [{_hostAssemblyDisplayName}]");
        result.Append($" using [{_libraryAssemblyDisplayName}]");

        return result.ToString();
    }
}
