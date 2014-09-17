using System.Windows;
using Algorithm.GUI.ViewModels;

namespace Algorithm.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new TickChartViewModel();
        }
    }
}