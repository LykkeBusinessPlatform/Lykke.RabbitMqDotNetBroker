// Copyright (c) 2024 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;

using NUnit.Framework;

namespace Lykke.RabbitMqBroker.Tests;

[TestFixture]
public class ConnectionStringHashTests
{
    [Test]
    public void Constructor_NullConnectionString_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ConnectionStringHash(null));
    }

    [Test]
    public void Constructor_EmptyConnectionString_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ConnectionStringHash(""));
    }

    [Test]
    public void Equals_NullObject_ReturnsFalse()
    {
        var hash = new ConnectionStringHash("connectionString");
        Assert.That(hash, Is.Not.EqualTo(null));
    }

    [Test]
    public void Equals_SameObject_ReturnsTrue()
    {
        var hash = new ConnectionStringHash("connectionString");
        Assert.That(hash, Is.EqualTo(hash));
    }

    [Test]
    public void Equals_DifferentTypeObject_ReturnsFalse()
    {
        var hash = new ConnectionStringHash("connectionString");
        Assert.That(hash, Is.Not.EqualTo("not a ConnectionStringHash"));
    }

    [Test]
    public void Equals_EqualHash_ReturnsTrue()
    {
        var hash1 = new ConnectionStringHash("connectionString");
        var hash2 = new ConnectionStringHash("connectionString");
        Assert.That(hash1, Is.EqualTo(hash2));
    }

    [Test]
    public void Equals_DifferentHash_ReturnsFalse()
    {
        var hash1 = new ConnectionStringHash("connectionString1");
        var hash2 = new ConnectionStringHash("connectionString2");
        Assert.That(hash1, Is.Not.EqualTo(hash2));
    }
}
