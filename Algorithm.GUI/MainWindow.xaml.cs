using System.Linq;
using System.Windows;
using Algorithm.Core.YahooFinanceService;

namespace Algorithm.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Query.QueryService _queryService = new Query.QueryService("APPL");

        public MainWindow()
        {
            InitializeComponent();
            var data = _queryService.GetStockPrice.ToList();
        }
    }
}