using System;
using TradingCore.Model.Interface;

namespace TradingCore.Model
{
    public class Tick : EntityBase
    {
        public Tick() { }

        public Tick(Guid id, string symbol, DateTime date, TimeSpan time, decimal open,
            decimal high, decimal low, decimal close, int volume)
        {
            TickId = id;
            Symbol = symbol;
            Date = date;
            Time = time;
            Open = open;
            High = high;
            Low = low;
            Close = close;
            Volume = volume;
        }

        public Guid TickId { get; set; }
        public string Symbol { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public int Volume { get; set; }
    }
}