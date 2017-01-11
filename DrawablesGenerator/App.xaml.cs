using Silverfeelin.StarboundDrawables;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WinForms = System.Windows.Forms;

namespace DrawablesGeneratorTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length == 1)
            {
                try
                {
                    (new MainWindow(e.Args[0])).Show();
                    return;
                }
                catch (DrawableException exc)
                {
                    MessageBox.Show(exc.Message + Environment.NewLine + "The selection has been cleared.");
                }
            }

            (new MainWindow()).Show();
        }
    }
}
