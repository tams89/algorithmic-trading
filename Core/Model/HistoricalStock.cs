using System;

namespace Core.Model
{
    public class HistoricalStock : EntityBase
    {
        public HistoricalStock()
        {

        }

        public HistoricalStock(Guid id, string symbol, string exchange, DateTime date, decimal open,
            decimal high, decimal low, decimal close, int volume)
        {
            HistoricalStockId = id;
            Symbol = symbol;
            Exchange = exchange;
            Date = date;
            Open = open;
            High = high;
            Low = low;
            Close = close;
            Volume = volume;
        }

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
