using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Windows;

namespace DrawablesGeneratorTool
{
    /// <summary>
    /// Interaction logic for OutputWindow.xaml
    /// </summary>
    public partial class OutputWindow : Window
    {
        private string contentString = null;
        private JObject contentObject = null;
        private bool formatted = true;

        public OutputWindow(string title, JObject content)
        {
            InitializeComponent();

            contentObject = content;

            tbxCode.Text = content.ToString(Newtonsoft.Json.Formatting.Indented);
        }

        public OutputWindow(string title, string content)
        {
            InitializeComponent();

            contentString = content;
            tbxCode.Text = content;   
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetDataObject(tbxCode.Text);
        }

        private void ToggleFormat_Click(object sender, RoutedEventArgs e)
        {
            formatted = !formatted;
            if (contentObject != null)
                tbxCode.Text = contentObject.ToString(formatted ? Newtonsoft.Json.Formatting.Indented : Newtonsoft.Json.Formatting.None);
            else
            {
                // TODO: Remove tabs and spaces, but only outside of string.. probably parse to JObject then format using ToString
                tbxCode.Text = formatted ? contentString : contentString.Replace(Environment.NewLine, "");
            }
                
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Save file to...";
            sfd.Filter = "JSON file|*.json|All files|*.*";

            bool? result = sfd.ShowDialog();

            if (result.HasValue && result.Value)
            {
                File.WriteAllText(sfd.FileName, tbxCode.Text);
            }
        }
    }
}
