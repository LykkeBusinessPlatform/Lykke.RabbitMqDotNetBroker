using System;
using System.Collections.Generic;

namespace LogParser.Configuration
{
    public class FilterOptions
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public List<string> ExcludedMessageTypes { get; set; } = new List<string>();
        public List<string> IncludedMessageTypes { get; set; } = new List<string>();
    }
}
