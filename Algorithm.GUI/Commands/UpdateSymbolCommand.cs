using System;
using System.Windows.Input;
using Algorithm.GUI.ViewModels;

namespace Algorithm.GUI.Commands
{
    public class UpdateSymbolCommand : ICommand
    {
        private readonly TickChartViewModel _tickChartViewModel;

        public UpdateSymbolCommand(TickChartViewModel tickChartViewModel)
        {
            _tickChartViewModel = tickChartViewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _tickChartViewModel.GetLatestTicks();
        }

        public event EventHandler CanExecuteChanged;
    }
}