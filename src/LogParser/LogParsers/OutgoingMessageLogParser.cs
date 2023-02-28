using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LogParser.Utils;
using Lykke.RabbitMqBroker.Logging;
using Newtonsoft.Json;

namespace LogParser.LogParsers
{
    public class OutgoingMessageLogParser
    {
        private readonly Regex _exchangeUrlRegex = new BetweenRegex("://", "/");

        public IEnumerable<OutgoingMessage> Parse(string log)
        {
            var regex = new BetweenRegex(OutgoingMessage.MessageStart, OutgoingMessage.MessageEnd);

            return regex.Matches(log)
                .Select(x => ExtractJson(x.Value))
                .Select(x =>
                {
                    var result = JsonConvert.DeserializeObject<OutgoingMessage>(x);
                    if (result == null) return null;
                    result.Exchange = ParseExchange(result.Exchange);
                    return result;
                })
                .Where(x => x != null)!;
        }

        private string ExtractJson(string val)
        {
            var start = val.IndexOf('{');
            var end = val.LastIndexOf('}');
            var json = val.Substring(start, end - start + 1);

            return json;
        }

        private string ParseExchange(in string str)
        {
            if (!str.Contains("://")) return str;

            var match = _exchangeUrlRegex.Match(str);
            var exchange = match.Value.Remove(0, 3);
            exchange = exchange.Remove(exchange.Length - 1, 1);
            return exchange;
        }
    }
}
