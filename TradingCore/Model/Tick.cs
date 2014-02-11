using System;

namespace TradingCore.Model
{
    public class Tick : IEntity
    {
        public Tick(Guid id, string symbol, DateTime date, DateTime time, decimal high,
            decimal low, decimal close, int volume)
        {
            TickId = id;
            Symbol = symbol;
            Date = date;
            Time = time;
            High = high;
            Low = low;
            Close = close;
            Volume = volume;
        }

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
