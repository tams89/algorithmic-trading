using System.Windows;
using Algorithm.GUI.Config;

namespace Algorithm.GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            AutoMapperConfig.Configure();
        }
    }
}
