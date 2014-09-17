
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Algorithm.Core.YahooFinanceService;
using Algorithm.GUI.Commands;
using Algorithm.GUI.Models;

namespace Algorithm.GUI.ViewModels
{
    public class TickChartViewModel
    {
        private readonly ISet<Tick> _ticks;
        private string _symbol;
        private readonly Query.QueryService _queryService;

        public TickChartViewModel()
        {
            _ticks = new HashSet<Tick>();
            UpdateCommand = new UpdateSymbolCommand(this);
            _queryService = new Query.QueryService(_symbol); // TODO refactor where the symbol is input from.
        }

        public string Symbol
        {
            get { return _symbol; }
            set { _symbol = value; }
        }

        public IEnumerable<Tick> Ticks
        {
            get { return _ticks; }
        }

        /// <summary>
        /// Gets the UpdateCommand for the ViewModel
        /// </summary>
        public ICommand UpdateCommand
        {
            get;
            private set;
        }

        public void GetLatestTicks()
        {
            var latestTicks = _queryService.GetStockPrice.Cast<Tick>();
            latestTicks.ToList().ForEach(t => _ticks.Add(t));
        }
    }
}
