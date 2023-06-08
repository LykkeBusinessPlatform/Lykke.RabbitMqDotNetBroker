using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Lykke.RabbitMqBroker.Logging;
using Newtonsoft.Json;

namespace LogParser.LogParsers
{
    public class OutgoingMessageLogParser
    {
        public IEnumerable<OutgoingMessage> Parse(string log)
        {
            var regex = Create(OutgoingMessage.MessageStart, OutgoingMessage.MessageEnd);
            return regex.Matches(log)
                .Select(x => ExtractJson(x.Value))
                .Select(JsonConvert.DeserializeObject<OutgoingMessage>)
                .Where(x => x != null)!;
        }

        private string ExtractJson(string val)
        {
            var start = val.IndexOf('{');
            var end = val.LastIndexOf('}');
            var json = val.Substring(start, end - start + 1);

            return json;
        }

        private Regex Create(string start, string end)
        {
            var str = $"({start})(.*?)({end})";
            return new Regex(str, RegexOptions.Singleline);
        }
    }
}
