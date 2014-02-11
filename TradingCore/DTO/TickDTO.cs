using System;

namespace TradingCore.DTO
{
    public class TickDTO : IDTO
    {
        public Guid TickId { get; set; }
        public string Symbol { get; set; }
        public DateTime Date { get; set; }
        public DateTime Time { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public int Volume { get; set; }
    }
}
