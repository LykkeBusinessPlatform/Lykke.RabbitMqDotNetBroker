using System;
using System.Collections.Generic;
using System.Globalization;
using JetBrains.Annotations;
using Lykke.RabbitMqBroker.Logging;
using MessagePack;
using Newtonsoft.Json.Linq;

namespace LogParser.LogParsers
{
    public class JsonRecordParser
    {
        private readonly Configuration.Configuration _configuration;
        private readonly OutgoingMessageBuilder _builder;

        public JsonRecordParser(Configuration.Configuration configuration)
        {
            _configuration = configuration;
        }

        public List<OutgoingMessage> ParseAll(string json)
        {
            JArray jsonArray = JArray.Parse(json);

            var list = new List<OutgoingMessage>();

            foreach (JToken item in jsonArray)
            {
                var dealId = (string)item["DealId"];
                var accountId = (string)item["AccountId"];
                var assetPairId = (string)item["AssetPairId"];
                var pnlOfTheLastDay = (decimal)item["PnlOfTheLastDay"];
                var created = (string)item["Created"];
                
                string format = "MM/dd/yyyy HH:mm:ss";
                var timestamp = DateTime.ParseExact(created, format, CultureInfo.InvariantCulture);

                var opid = $"{dealId}-update-balance";

                var command = new UpdateBalanceInternalCommand()
                {
                    OperationId = opid,
                    AccountId = accountId,
                    AmountDelta = pnlOfTheLastDay,
                    Comment = $"Balance changed on position close (id = {dealId})",
                    AuditLog = string.Empty,
                    Source = "ClosePositionSaga",
                    ChangeReasonType = AccountBalanceChangeReasonType.RealizedPnL,
                    EventSourceId = dealId,
                    AssetPairId = assetPairId,
                    TradingDay = timestamp
                };

                var bytes = MessagePackSerializer.Serialize(command);

                var outgoingMessage = new OutgoingMessage();
                outgoingMessage.MessageTypeFullName = "MarginTrading.AccountsManagement.Workflow.UpdateBalance.Commands.UpdateBalanceInternalCommand";
                outgoingMessage.MessageTypeName = "UpdateBalanceInternalCommand";
                outgoingMessage.Format = SerializationFormat.Messagepack;
                outgoingMessage.Message = Convert.ToBase64String(bytes);
                outgoingMessage.Headers = new Dictionary<string, object>()
                {
                    { "Lykke-Correlation-ID", opid }
                };
                outgoingMessage.Exchange = _configuration.RabbitConfig.ExchangeName;
                outgoingMessage.RoutingKey = "UpdateBalanceInternalCommand";
                outgoingMessage.Timestamp = DateTime.UtcNow;

                list.Add(outgoingMessage);
            }

            return list;
        }
    }
}