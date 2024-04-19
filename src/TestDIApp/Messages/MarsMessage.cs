using JetBrains.Annotations;

namespace TestDIApp.Messages;

[UsedImplicitly]
public class MarsMessage
{
    public string Sender { get; set; } = "Someone on Mars";
    public string Text { get; set; } = string.Empty;
}
