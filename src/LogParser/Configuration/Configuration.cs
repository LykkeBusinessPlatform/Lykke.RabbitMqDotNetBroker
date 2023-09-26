using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace LogParser.Configuration
{
    public class Configuration
    {
        private readonly IOptions<ParsingOptions> _parsingOptions;
        private readonly FilterOptions _filterOptions;

        public Configuration(IOptions<FilterOptions> filterOptions,
            IOptions<ParsingOptions> parsingOptions)
        {
            _parsingOptions = parsingOptions;
            _filterOptions = filterOptions.Value;
        }

        public DateTime? From => _filterOptions.From?.ToUniversalTime();

        public DateTime? To => _filterOptions.To?.ToUniversalTime();

        public List<string> ExcludedMessageTypes => _filterOptions.ExcludedMessageTypes;
        
        /// <summary>
        /// If set, ExcludedMessageTypes is ignored. Only types specified here will be sent
        /// </summary>
        public List<string> IncludedMessageTypes => _filterOptions.IncludedMessageTypes;

        public string LogDirectory => _parsingOptions.Value.LogDirectory;
    }
}
