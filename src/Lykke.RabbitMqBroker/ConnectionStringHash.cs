// Copyright (c) 2024 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Security.Cryptography;
using System.Text;

namespace Lykke.RabbitMqBroker
{
    internal sealed class ConnectionStringHash : IEquatable<ConnectionStringHash>
    {
        private readonly string _originalConnectionString;
        private readonly string _hash;
        
        public ConnectionStringHash(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            _originalConnectionString = connectionString;
            _hash = CalculateHash();
        }
        
        private string CalculateHash()
        {
            var bytesToHash = Encoding.UTF8.GetBytes(_originalConnectionString);
            var hash = MD5.HashData(bytesToHash);
            var stringHash = Convert.ToBase64String(hash);
            return stringHash;
        }

        public bool Equals(ConnectionStringHash other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _hash == other._hash;
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is ConnectionStringHash other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (_hash != null ? _hash.GetHashCode() : 0);
        }

        public static bool operator ==(ConnectionStringHash left, ConnectionStringHash right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ConnectionStringHash left, ConnectionStringHash right)
        {
            return !Equals(left, right);
        }
    }
}
