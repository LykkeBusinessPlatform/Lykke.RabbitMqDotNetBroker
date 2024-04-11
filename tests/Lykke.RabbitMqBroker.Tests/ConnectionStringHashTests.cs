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
        Assert.IsFalse(hash.Equals(null));
    }

    [Test]
    public void Equals_SameObject_ReturnsTrue()
    {
        var hash = new ConnectionStringHash("connectionString");
        Assert.IsTrue(hash.Equals(hash));
    }

    [Test]
    public void Equals_DifferentTypeObject_ReturnsFalse()
    {
        var hash = new ConnectionStringHash("connectionString");
        Assert.IsFalse(hash.Equals("not a ConnectionStringHash"));
    }

    [Test]
    public void Equals_EqualHash_ReturnsTrue()
    {
        var hash1 = new ConnectionStringHash("connectionString");
        var hash2 = new ConnectionStringHash("connectionString");
        Assert.IsTrue(hash1.Equals(hash2));
    }

    [Test]
    public void Equals_DifferentHash_ReturnsFalse()
    {
        var hash1 = new ConnectionStringHash("connectionString1");
        var hash2 = new ConnectionStringHash("connectionString2");
        Assert.IsFalse(hash1.Equals(hash2));
    }
}
