using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lykke.RabbitMqBroker.Subscriber;

public abstract record ResourceName(string Value)
{
    private static readonly List<string> ProhibitedPrefixes = ["amq.", "amqp."];
    private const int MaxLengthInBytes = 255;
    private const string ForbiddenSymbols = @"!@#$%^&*()+=[]{}|\;'"",<>/?`~";
    protected static string Validate(string value)
    {
        return value switch
        {
            null => throw new ArgumentNullException(nameof(value), "Value cannot be null."),
            "" => throw new ArgumentException("Value cannot be empty.", nameof(value)),
            _ when value.Any(ForbiddenSymbols.Contains) => throw new ArgumentException($"Value cannot contain any of the following symbols: {ForbiddenSymbols}", nameof(value)),
            _ when ProhibitedPrefixes.Exists(p => value.StartsWith(p, StringComparison.InvariantCultureIgnoreCase)) => throw new ArgumentException($"Value cannot start with {string.Join(", ", ProhibitedPrefixes)}", nameof(value)),
            _ when Encoding.UTF8.GetByteCount(value) > MaxLengthInBytes => throw new ArgumentException($"Value length in bytes cannot exceed {MaxLengthInBytes}", nameof(value)),
            _ => value.Trim()
        };
    }

    public override string ToString() => Value;
}
