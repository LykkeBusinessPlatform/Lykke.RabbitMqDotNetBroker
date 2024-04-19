using JetBrains.Annotations;

namespace TestDIApp.Messages;

[UsedImplicitly]
public class JupiterMessage
{
    public string Sender { get; set; } = "Someone on Jupiter";
    public string Text { get; set; } = string.Empty;
}
