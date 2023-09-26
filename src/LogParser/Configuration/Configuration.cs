using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace LogParser.Configuration
{
    public class Configuration
    {
        private readonly IOptions<ParsingOptions> _parsingOptions;
        private readonly FilterOptions _filterOptions;
        private readonly RabbitConfig _rabbitConfig;

        private readonly InputConfig _inputConfig;

        public Configuration(IOptions<FilterOptions> filterOptions,
            IOptions<ParsingOptions> parsingOptions,
            IOptions<RabbitConfig> rabbitConfig,
            IOptions<InputConfig> inputConfig)
        {
            _parsingOptions = parsingOptions;
            _filterOptions = filterOptions.Value;
            _rabbitConfig = rabbitConfig.Value;
            _inputConfig = inputConfig.Value;
        }

        public DateTime? From => _filterOptions.From?.ToUniversalTime();

        public DateTime? To => _filterOptions.To?.ToUniversalTime();

        public List<string> ExcludedMessageTypes => _filterOptions.ExcludedMessageTypes;
        
        public RabbitConfig RabbitConfig => _rabbitConfig;
        public InputConfig InputConfig => _inputConfig;

        public string LogDirectory => _parsingOptions.Value.LogDirectory;
    }
}
