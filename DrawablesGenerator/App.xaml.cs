using System.Windows;

namespace DrawablesGeneratorTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            (new MainWindow()).Show();
        }
    }
}
