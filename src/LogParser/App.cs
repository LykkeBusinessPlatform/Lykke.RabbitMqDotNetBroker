using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common;
using LogParser.Filters;
using LogParser.LogParsers;
using LogParser.Services;
using Lykke.RabbitMqBroker.Logging;
using Microsoft.Extensions.Logging;

namespace LogParser
{
    public class App
    {
        private readonly Configuration.Configuration _configuration;
        private readonly Publisher _publisher;
        private readonly ILogger<App> _logger;

        private readonly OutgoingMessageLogParser _logParser = new OutgoingMessageLogParser();

        public App(Configuration.Configuration configuration,
            Publisher publisher,
            ILogger<App> logger)
        {
            _configuration = configuration;
            _publisher = publisher;
            _logger = logger;
        }

        public void Execute()
        {
            _logger.LogInformation("Starting to import messages from logs");

            _logger.LogInformation("Current configuration: {Configuration}", _configuration.ToJson());

            var logsPath = _configuration.LogDirectory;
            var files = GetFiles(logsPath);

            var outgoingMessages = Parse(files);

            var sortedAndFiltered = outgoingMessages
                .SelectMany(x => x)
                .OrderBy(x => x.Timestamp)
                .FromUtcDate(_configuration.From)
                .ToUtcDate(_configuration.To)
                .ExcludeMessageTypes(_configuration.ExcludedMessageTypes)
                .ToList();

            LogStats(sortedAndFiltered);

            Publish(sortedAndFiltered);

            _logger.LogInformation("Messages published");
        }

        private void Publish(List<OutgoingMessage> messages)
        {
            var i = 1;
            foreach (var message in messages)
            {
                _logger.LogInformation("Publishing event #{I} out of {N}, message type: {Type}, exchange: {Exchange}",
                    i,
                    messages.Count,
                    message.MessageTypeName,
                    message.Exchange);
                _publisher.Publish(message);

                i++;
            }
        }

        private IEnumerable<IEnumerable<OutgoingMessage>> Parse(List<string> files)
        {
            foreach (var file in files)
            {
                _logger.LogInformation("Starting to parse {File}", file);

                yield return _logParser.Parse(File.ReadAllText(file));

                _logger.LogInformation("File {File} processed", file);
            }
        }

        private List<string> GetFiles(string path)
        {
            if (!Directory.Exists(path))
            {
                _logger.LogError("Directory {Path} not found", path);
                throw new Exception("Check directory configuration: directory not found");
            }

            var files = Directory.EnumerateFiles(path).ToList();
            if (files.Count == 0)
            {
                _logger.LogError("Logfiles not found in directory {Path}", path);
                throw new Exception("Check directory configuration: logfiles not found");
            }

            _logger.LogInformation("{N} files found", files.Count);

            return files;
        }

        private void LogStats(IEnumerable<OutgoingMessage> outgoingMessages)
        {
            var groups = outgoingMessages.GroupBy(x => x.MessageTypeName);
            foreach (var group in groups)
            {
                _logger.LogInformation("Found {Count} outgoing messages of category {Category}",
                    group.Count(),
                    group.Key);
            }
        }
    }
}
