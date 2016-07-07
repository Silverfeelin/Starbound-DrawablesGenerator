using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DrawablesGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly int
            PREVIEW_MARGIN_LEFT = 153,
            PREVIEW_MARGIN_TOP = 306;

        private DrawableData data;

        public MainWindow()
        {
            InitializeComponent();
            data = new DrawableData();

            this.UpdatePreviewImage();
        }

        #region Select Image

        /// <summary>
        /// Prompt an OpenFileDialog for the user to select an image file, sets this.imagePath if it's valid and the dimensions are within this.pixelLimit.
        /// Calls UpdatePreviewImage() to update the preview.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image files|*.png;*.jpg;*.bmp;*.gif";
            ofd.Title = "Select an image.";

            bool? ofdResult = ofd.ShowDialog();
            if (!ofdResult.HasValue || !ofdResult.Value)
                return;

            SelectImage(ofd.FileName);
        }

        public void SelectImage(string path)
        {
            try
            {
                data.LoadImage(path);
            }
            catch (ArgumentException)
            {
                MessageBox.Show("The image could not be loaded. Please try another image.");
                return;
            }
            catch (DrawableException exc)
            {
                MessageBox.Show(exc.Message + Environment.NewLine + "The selection has been cleared.");
                return;
            }
            finally
            {
                UpdatePreviewImage();
            }
        }

        /// <summary>
        /// Updates the preview image, and sets the image to the the '0,0' hand position if this.imagePath points to a valid image.
        /// Resets the hand and fire position textboxes.
        /// </summary>
        private void UpdatePreviewImage()
        {
            tbxHandX.Text = "0";
            tbxHandY.Text = "0";

            if (!data.ValidImage)
            {
                // Remove image.
                imgPreview.Source = null;

                tbxImage.Text = string.Empty;
            }
            else
            {
                // Display image.
                BitmapImage bi = new BitmapImage(new Uri(data.SelectedImagePath));
                imgPreview.Source = bi;
                imgPreview.Width = bi.PixelWidth * 2;
                imgPreview.Height = bi.PixelHeight * 2;

                Thickness margin = imgPreview.Margin;
                margin.Left = PREVIEW_MARGIN_LEFT;
                margin.Top = PREVIEW_MARGIN_TOP - imgPreview.Height;
                imgPreview.Margin = margin;

                tbxImage.Text = data.SelectedImagePath;
            }
        }

        #endregion

        #region Drag on Preview

        /// <summary>
        /// Starts capturing the mouse for the preview window, to update the position of the image in the Preview_MouseMove event.
        /// Also calls Preview_MouseMove, to update the preview even when the mouse isn't moved.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Preview_MouseDown(object sender, MouseButtonEventArgs e)
        {
            brdPreview.CaptureMouse();
            this.Preview_MouseMove(sender, e);
        }

        /// <summary>
        /// Adjusts the hand position textboxes by clicking (and dragging the mouse) on the preview window. 
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Preview_MouseMove(object sender, MouseEventArgs e)
        {
            if (data.ValidImage && brdPreview.IsMouseCaptured)
            {
                var pos = e.GetPosition(brdPreview);
                Thickness margin = new Thickness(pos.X - (imgPreview.Width / 2), pos.Y - (imgPreview.Height / 2), 0, 0);

                // Prevent displaying the image 'between' game pixels.
                if (margin.Top % 2 == 0)
                    margin.Top++;

                if (margin.Left % 2 == 0)
                    margin.Left++;

                int originalWidth = PREVIEW_MARGIN_LEFT,
                    originalHeight = PREVIEW_MARGIN_TOP - Convert.ToInt32(imgPreview.Height);

                // This also fires HandX_TextChanged and HandY_TextChanged.
                tbxHandX.Text = Math.Ceiling((margin.Left - originalWidth) / 2).ToString();
                tbxHandY.Text = (-Math.Ceiling((margin.Top - originalHeight) / 2)).ToString();
            }
        }

        /// <summary>
        /// Stops capturing the mouse for the preview window.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Preview_MouseUp(object sender, MouseButtonEventArgs e)
        {
            brdPreview.ReleaseMouseCapture();
        }

        #endregion

        #region Themes

        /// <summary>
        /// Changes the background of the preview to a dark image.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ThemeBlack_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ChangeTheme("DarkSmall.png");
        }

        /// <summary>
        /// Changes the background of the preview to a light image.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ThemeWhite_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ChangeTheme("LightSmall.png");
        }

        /// <summary>
        /// Changes the background of the preview to a natural image.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ThemeGreen_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ChangeTheme("NaturalSmall.png");
        }

        /// <summary>
        /// Changes the background of the preview.
        /// </summary>
        /// <param name="resourcePath">Path to an image, relative to Project/Resources/. Do not start with a slash.</param>
        private void ChangeTheme(string resourcePath)
        {
            if (resourcePath.IndexOf("/") == 0)
                resourcePath = resourcePath.Substring(1);

            imgPreviewBackground.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/" + resourcePath));
        }

        #endregion

        #region Positioning

        /// <summary>
        /// Variables to store the last valid value for the position textboxes; used to restore the value if the user enters an invalid character.
        /// </summary>
        private string oldTbxHandX = "0", oldTbxHandY = "0";

        /// <summary>
        /// Use the value of the hand position textboxes to render the preview image in the right location.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void HandX_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!this.IsInitialized)
            {
                return;
            }

            TextBox tbx = sender as TextBox;

            if (tbx.Text == string.Empty)
            {
                this.oldTbxHandX = "0";
                tbx.CaretIndex = 1;
                return;
            }

            e.Handled = !DrawableData.IsNumber(tbx.Text);

            if (e.Handled)
            {
                int index = DrawableData.Clamp(tbx.CaretIndex - 1, 0, tbx.Text.Length - 1);
                tbx.Text = this.oldTbxHandX;
                tbx.CaretIndex = index;
            }
            else
            {
                this.oldTbxHandX = tbx.Text;

                Thickness t = imgPreview.Margin;
                t.Left = PREVIEW_MARGIN_LEFT + (Convert.ToInt32(tbx.Text) * 2);
                imgPreview.Margin = t;
            }
        }

        /// <summary>
        /// Confirms if the new text value is a valid positive or negative integer, and updates the preview image position if it is.
        /// If the value is invalid, restores it to the preview value.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void HandY_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!this.IsInitialized)
                return;

            TextBox tbx = sender as TextBox;

            if (tbx.Text == string.Empty)
            {
                this.oldTbxHandY = "0";
                tbx.CaretIndex = 1;
                return;
            }

            e.Handled = !DrawableData.IsNumber(tbx.Text);

            if (e.Handled)
            {
                int index = DrawableData.Clamp(tbx.CaretIndex - 1, 0, tbx.Text.Length - 1);
                tbx.Text = this.oldTbxHandY;
                tbx.CaretIndex = index;
            }
            else
            {
                this.oldTbxHandY = tbx.Text;

                Thickness t = imgPreview.Margin;
                t.Top = PREVIEW_MARGIN_TOP - Convert.ToInt32(imgPreview.Height) - (Convert.ToInt32(tbx.Text) * 2);
                imgPreview.Margin = t;
            }
        }

        /// <summary>
        /// Event to increase or decrease the numbers in a textbox by using the up/down arrow keys for convenience (as there's only one line) 
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Hand_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox tbx = sender as TextBox;
            if (tbx.Text == string.Empty)
            {
                tbx.Text = "0";
            }

            string text = tbx.Text;
            switch (e.Key)
            {
                case Key.Up:
                    tbx.Text = string.Empty;
                    tbx.AppendText((Convert.ToInt32(text) + 1).ToString());
                    e.Handled = true;
                    break;
                case Key.Down:
                    tbx.Text = string.Empty;
                    tbx.AppendText((Convert.ToInt32(text) - 1).ToString());
                    e.Handled = true;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Resets Textbox.Text back to 0, if it's left empty when leaving the control
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Hand_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tbx = sender as TextBox;

            if (tbx.Text == string.Empty)
            {
                tbx.Text = "0";
            }
        }

        #endregion

        #region Export Options

        private void PlainText_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                DrawableOutput dt = data.GenerateDrawables(Convert.ToDouble(tbxHandX.Text) / 8d, Convert.ToDouble(tbxHandY.Text) / 8d);
                (new OutputWindow("Item Configuration:", dt.GenerateText())).Show();
            }
            catch (DrawableException exc)
            {
                MessageBox.Show(exc.Message);
                return;
            }
        }

        private void SingleTextureDirectives_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                DrawableOutput dt = data.GenerateDrawables(Convert.ToDouble(tbxHandX.Text) / 8d, Convert.ToDouble(tbxHandY.Text) / 8d, true);
                (new OutputWindow("Texture and Directives:", dt.GenerateSingleTextureDirectives())).Show();
            }
            catch (DrawableException exc)
            {
                MessageBox.Show(exc.Message);
                return;
            }
        }

        private void SelectImage_Drop(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    if (files.Length == 1)
                    {
                        SelectImage(files[0]);
                    }
                }
            }
            catch (ArgumentException)
            {
                MessageBox.Show("The image could not be loaded. Please try another image.");
                return;
            }
            catch (DrawableException exc)
            {
                MessageBox.Show(exc.Message + Environment.NewLine + "The selection has been cleared.");
                return;
            }
            finally
            {
                UpdatePreviewImage();
            }
        }

        private void StarCheatExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "JSON File|*.json|Any File|*.*";
                bool? res = sfd.ShowDialog();
                if (!res.HasValue || !res.Value)
                    return;

                DrawableOutput dt = data.GenerateDrawables(Convert.ToDouble(tbxHandX.Text) / 8d, Convert.ToDouble(tbxHandY.Text) / 8d);
                File.WriteAllText(sfd.FileName, dt.GenerateText());
                Process.Start("explorer.exe", @"/select, " + @"""" + sfd.FileName + @"""");
            }
            catch (DrawableException exc)
            {
                MessageBox.Show(exc.Message);
                return;
            }
        }

        private void Command_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DrawableOutput dt = data.GenerateDrawables(Convert.ToDouble(tbxHandX.Text) / 8d, Convert.ToDouble(tbxHandY.Text) / 8d);
                (new OutputWindow("Command:", dt.GenerateCommand())).Show();
            }
            catch (DrawableException exc)
            {
                MessageBox.Show(exc.Message);
                return;
            }
        }

        #endregion
    }
}
