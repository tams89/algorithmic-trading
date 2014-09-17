using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Algorithm.GUI.Annotations;

namespace Algorithm.GUI.Models
{
    public class Tick : INotifyPropertyChanged
    {
        private DateTime _date;
        private decimal _open;
        private decimal _high;
        private decimal _low;
        private decimal _close;
        private decimal _volume;
        private decimal _adjClose;
        private decimal? _ask;
        private decimal? _bid;

        public Tick(DateTime date, decimal open, decimal high, decimal low, decimal close,
            decimal volume, decimal adjClose, decimal? ask, decimal? bid)
        {
            Date = date;
            Open = open;
            High = high;
            Low = low;
            Close = close;
            Volume = volume;
            AdjClose = adjClose;
            Ask = ask;
            Bid = bid;
        }

        public DateTime Date
        {
            get { return _date; }
            set
            {
                if (value.Equals(_date)) return;
                _date = value;
                OnPropertyChanged();
            }
        }

        public decimal Open
        {
            get { return _open; }
            set
            {
                if (value == _open) return;
                _open = value;
                OnPropertyChanged();
            }
        }

        public decimal High
        {
            get { return _high; }
            set
            {
                if (value == _high) return;
                _high = value;
                OnPropertyChanged();
            }
        }

        public decimal Low
        {
            get { return _low; }
            set
            {
                if (value == _low) return;
                _low = value;
                OnPropertyChanged();
            }
        }

        public decimal Close
        {
            get { return _close; }
            set
            {
                if (value == _close) return;
                _close = value;
                OnPropertyChanged();
            }
        }

        public decimal Volume
        {
            get { return _volume; }
            set
            {
                if (value == _volume) return;
                _volume = value;
                OnPropertyChanged();
            }
        }

        public decimal AdjClose
        {
            get { return _adjClose; }
            set
            {
                if (value == _adjClose) return;
                _adjClose = value;
                OnPropertyChanged();
            }
        }

        public decimal? Ask
        {
            get { return _ask; }
            set
            {
                if (value == _ask) return;
                _ask = value;
                OnPropertyChanged();
            }
        }

        public decimal? Bid
        {
            get { return _bid; }
            set
            {
                if (value == _bid) return;
                _bid = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}