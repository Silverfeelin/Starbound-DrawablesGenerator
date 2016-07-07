using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WinForms = System.Windows.Forms;

namespace DrawablesGenerator
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
                    DrawableData data = new DrawableData();
                    data.LoadImage(e.Args[0]);
                    Clipboard.SetText(data.GenerateDrawables(0, 0).GenerateCommand());
                    MessageBox.Show("The command has been copied to the clipboard.", "Drawables Generator");
                }
                catch (ArgumentException)
                {
                    MessageBox.Show("The image could not be loaded. Please try another image.");
                }
                catch (DrawableException exc)
                {
                    MessageBox.Show(exc.Message + Environment.NewLine + "The selection has been cleared.");
                }

                Application.Current.Shutdown();
                return;
            }

            MainWindow mw = new MainWindow();
            mw.Show();
        }
    }
}
