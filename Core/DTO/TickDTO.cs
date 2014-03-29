using System;
using TradingCore.DTO.Interface;

namespace TradingCore.DTO
{
    public class TickDTO : IDTO
    {
        public Guid TickId { get; set; }
        public string Symbol { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public int Volume { get; set; }
    }
}
