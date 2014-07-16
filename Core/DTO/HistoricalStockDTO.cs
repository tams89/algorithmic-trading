using System;

namespace Core.DTO
{
    public class HistoricalStockDto : IDto
    {
        public Guid HistoricalStockId { get; set; }
        public string Symbol { get; set; }
        public string Exchange { get; set; }
        public DateTime Date { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public int Volume { get; set; }
    }
}
