using JetBrains.Annotations;

namespace TestDIApp.Messages;

[UsedImplicitly]
public class MercuryMessage
{
    public string Sender { get; set; } = "Someone on Mercury";
    public string Text { get; set; } = string.Empty;
}
