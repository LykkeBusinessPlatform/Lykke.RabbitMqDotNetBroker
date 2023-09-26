using System;
using MessagePack;

namespace LogParser
{
    [MessagePackObject]
    public class UpdateBalanceInternalCommand
    {
        [Key(0)]
        public string OperationId { get; set; }

        [Key(1)]
        public string AccountId { get; set; }

        [Key(2)]
        public decimal AmountDelta { get; set; }

        [Key(3)]
        public string Comment { get; set; }

        [Key(4)]
        public string AuditLog { get; set; }

        [Key(5)]
        public string Source { get; set; }

        [Key(6)]
        public AccountBalanceChangeReasonType ChangeReasonType { get; set; }

        /// <summary>
        /// Event source ID (order, position, trade, etc).
        /// </summary>
        [Key(7)]
        public string EventSourceId { get; set; }

        /// <summary>
        /// Asset Pair ID (if can be found any)
        /// </summary>
        [Key(8)]
        public string AssetPairId { get; set; }

        [Key(9)]
        public DateTime TradingDay { get; set; }
    }
}