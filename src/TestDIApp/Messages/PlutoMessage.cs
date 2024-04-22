using JetBrains.Annotations;

namespace TestDIApp.Messages;

[UsedImplicitly]
public class PlutoMessage
{
    public string Sender { get; set; } = "Someone on Pluto";
    public string Text { get; set; } = string.Empty;
}
