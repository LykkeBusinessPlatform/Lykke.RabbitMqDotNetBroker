using NUnit.Framework;

namespace Lykke.RabbitMqBroker.Tests;

[TestFixture]
public class ConnectionDisplayNameTests
{
    [Test]
    public void SharedConnection_DisplayName_Should_State_It_Is_Shared()
    {
        var displayName = new SharedConnectionAssemblyBasedDisplayName();

        Assert.That((string)displayName, Does.Contain("Shared connection"));
    }
    
    [Test]
    public void ExclusiveConnection_DisplayName_Should_State_It_Is_Exclusive()
    {
        var displayName = new ExclusiveConnectionAssemblyBasedDisplayName();

        Assert.That((string)displayName, Does.Contain("Exclusive connection"));
    }
    
    [Test]
    public void ExclusiveConnection_DisplayName_Should_Contain_Name_If_Provided()
    {
        var displayName = new ExclusiveConnectionAssemblyBasedDisplayName("MyConnection");

        Assert.That((string)displayName, Does.Contain("MyConnection"));
    }
}
