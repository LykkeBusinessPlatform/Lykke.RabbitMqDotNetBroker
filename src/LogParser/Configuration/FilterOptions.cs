namespace LogParser.Configuration;

public class FilterOptions
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public List<string> ExcludedMessageTypes { get; set; } = new();
}
