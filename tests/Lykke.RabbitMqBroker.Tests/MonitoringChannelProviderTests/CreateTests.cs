using NUnit.Framework;

namespace Lykke.RabbitMqBroker.Tests.MonitoringChannelProviderTests;

[TestFixture]
internal sealed class CreateTests
{
    private FakeConnectionProvider _connectionProvider;

    [SetUp]
    public void SetUp()
    {
        _connectionProvider = new FakeConnectionProvider();
    }

    [Test]
    public void Create_Gets_Exclusive_Connection()
    {
        var sut = CreateSut();

        var _ = sut.Create();

        Assert.That(
            _connectionProvider.ExclusiveConnectionsCount,
            Is.EqualTo(1));
    }

    [Test]
    public void Create_Publisher_Confirmation_Is_Enabled()
    {
        var sut = CreateSut();

        var channel = sut.Create();

        Assert.That(channel is FakeChannel fakeChannel && fakeChannel.PublisherConfirmationsEnabled);
    }

    private MonitoringMessageChannelProvider CreateSut()
    {
        return new MonitoringMessageChannelProvider(
            _connectionProvider,
            "connectionString");
    }
}