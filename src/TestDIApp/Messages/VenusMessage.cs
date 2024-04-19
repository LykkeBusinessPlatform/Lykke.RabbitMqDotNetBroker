using JetBrains.Annotations;

namespace TestDIApp.Messages;

[UsedImplicitly]
public class VenusMessage
{
    public string Sender { get; set; } = "Someone on Venus";
    public string Text { get; set; } = string.Empty;
}
