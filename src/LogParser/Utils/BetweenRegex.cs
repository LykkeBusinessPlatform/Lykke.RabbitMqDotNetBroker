using System.Text.RegularExpressions;

namespace LogParser.Utils
{
    public class BetweenRegex : Regex
    {
        public BetweenRegex(in string start, in string end)
            : base(Create(start, end), RegexOptions.Singleline)
        {
        }

        private static string Create(string start, string end)
        {
            var escapedStart = Regex.Escape(start);
            var escapedEnd = Regex.Escape(end);

            var str = $"({escapedStart})(.*?)({escapedEnd})";
            return str;
        }
    }
}
