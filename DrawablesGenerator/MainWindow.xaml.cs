using Microsoft.Win32;
using Silverfeelin.StarboundDrawables;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace DrawablesGeneratorTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly int
            PREVIEW_MARGIN_LEFT = 153,
            PREVIEW_MARGIN_TOP = 306;

        private readonly FileSystemWatcher watcher;
        private BitmapImage previewImage = null;
        private string imagePath = null;
        private bool warned = false;

        public MainWindow()
        {
            InitializeComponent();

            watcher = new FileSystemWatcher();
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
            watcher.Changed += Watcher_Changed;
            watcher.Deleted += Watcher_Deleted;
            watcher.Renamed += Watcher_Deleted;
        }

        #region File Watcher

        // Update selected image when modified externally.
        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => { SelectImage(null); }));
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => { UpdatePreviewImage(); }));
        }

        #endregion

        public MainWindow(string imagePath) : this()
        {
            this.SelectImage(imagePath);
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

        /// <summary>
        /// Event handler for dropping a file onto the application. Attempts to select it as an image.
        /// <see cref="SelectImage(string)"/>
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">The <see cref="DragEventArgs"/> instance containing event data, including the dropped item(s).</param>
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
        }

        public void SelectImage(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path) || !File.Exists(path))
                    throw new DrawableException("Invalid image selected, or the file no longer exists.");

                imagePath = path;
                watcher.Path = Path.GetDirectoryName(path);
                watcher.Filter = Path.GetFileName(path);
                watcher.EnableRaisingEvents = true;
            }
            catch (DrawableException exc)
            {
                imagePath = null;
                previewImage = null;
                MessageBox.Show(exc.Message + Environment.NewLine + "The selection has been cleared.");
                return;
            }
            finally
            {
                NewImageSelected();
            }
        }

        private void UpdatePreviewImage()
        {
            try
            {
                if (!string.IsNullOrEmpty(imagePath))
                {
                    tbxImage.Text = imagePath;

                    Uri imageUri = new Uri(imagePath);
                    BitmapImage bi = new BitmapImage();

                    bi.BeginInit();
                    bi.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.UriSource = imageUri;
                    bi.EndInit();
                    bi.Freeze();

                    previewImage = bi;
                    imgPreview.Source = bi;
                    imgPreview.Width = bi.PixelWidth * 2;
                    imgPreview.Height = bi.PixelHeight * 2;
                }
            }
            catch
            {
                previewImage = null;
                imagePath = null;
                imgPreview.Source = null;
            }

            bool clear = false;

            if (previewImage != null) // Check dimensions if image is loaded.
            {
                int pixels = previewImage.PixelWidth * previewImage.PixelHeight;
                if (pixels > 32768)
                {
                    if (!warned)
                    {
                        MessageBoxResult res = MessageBox.Show("The image (" + previewImage.PixelWidth + "x" + previewImage.PixelHeight + "=" + pixels + ") exceeds the limit of " + 32768 + " total pixels.\nAre you sure you would like to continue?", "Warning", MessageBoxButton.YesNo);
                        if (res == MessageBoxResult.Yes)
                            warned = true;
                        else
                            clear = true;
                    }
                    
                }
            }
            else
            {
                clear = true;
            }

            if (clear)
            {
                // Remove image.
                imgPreview.Source = null;
                previewImage = null;

                tbxImage.Text = string.Empty;
                MessageBox.Show("The selection has been cleared.");
            }
            else
            {
                // Set preview position to match the last known position.
                Thickness t = imgPreview.Margin;
                t.Left = PREVIEW_MARGIN_LEFT + (Convert.ToInt32(oldTbxHandX) * 2);
                t.Top = PREVIEW_MARGIN_TOP - Convert.ToInt32(imgPreview.Height) - (Convert.ToInt32(oldTbxHandY) * 2);
                imgPreview.Margin = t;
            }
        }

        /// <summary>
        /// Updates the preview image, and sets the image to the the '0,0' hand position if this.imagePath points to a valid image.
        /// Resets the hand and fire position textboxes.
        /// </summary>
        private void NewImageSelected()
        {
            tbxHandX.Text = "0";
            tbxHandY.Text = "0";

            // Display image.
            UpdatePreviewImage();
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
            // TODO: Change generator.Image to self contained value.
            if (previewImage != null && brdPreview.IsMouseCaptured)
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

            TextBox tbx = tbxHandX;

            if (tbx.Text == string.Empty)
            {
                this.oldTbxHandX = "0";
                tbx.CaretIndex = 1;
                return;
            }

            e.Handled = !DrawableUtilities.IsNumber(tbx.Text);

            if (e.Handled)
            {
                int index = DrawableUtilities.Clamp(tbx.CaretIndex - 1, 0, tbx.Text.Length - 1);
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

            TextBox tbx = tbxHandY;

            if (tbx.Text == string.Empty)
            {
                this.oldTbxHandY = "0";
                tbx.CaretIndex = 1;
                return;
            }

            e.Handled = !DrawableUtilities.IsNumber(tbx.Text);

            if (e.Handled)
            {
                int index = DrawableUtilities.Clamp(tbx.CaretIndex - 1, 0, tbx.Text.Length - 1);
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
                DrawablesGenerator generator = new DrawablesGenerator(imagePath);

                generator = DrawableUtilities.SetUpGenerator(generator, tbxHandX.Text, tbxHandY.Text, tbxIgnoreColor.Text);
                generator.ReplaceWhite = true;

                DrawablesOutput output = generator.Generate();
                (new OutputWindow("Item Descriptor (Export):", (GetExporter(output)).GetDescriptor(chkAddWeaponGroup.IsChecked.HasValue && chkAddWeaponGroup.IsChecked.Value ? "weapon" : null))).Show();
            }
            catch (NotImplementedException exc)
            {
                MessageBox.Show(exc.Message);
                return;
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("Argument may not be null. Did you select a valid image?");
            }
            catch (FormatException)
            {
                MessageBox.Show("Could not convert hand offsets to numbers.");
                return;
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
                DrawablesGenerator generator = new DrawablesGenerator(imagePath);

                generator = DrawableUtilities.SetUpGenerator(generator, tbxHandX.Text, tbxHandY.Text, tbxIgnoreColor.Text);
                generator.ReplaceBlank = true;
                generator.ReplaceWhite = true;

                DrawablesOutput output = generator.Generate();

                int j = 64;
                int.TryParse(tbxSourceImageSize.Text, out j);
                (new OutputWindow("Single Texture Directives:", DrawableUtilities.GenerateSingleTextureDirectives(output, j))).Show();
            }
            catch (FormatException)
            {
                MessageBox.Show("Invalid format. Did you provide a correct ignore color code? (hexadecimal RRGGBB or RRGGBBAA)");
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("Argument may not be null. Did you select a valid image?");
            }
            catch (DrawableException exc)
            {
                MessageBox.Show(exc.Message);
                return;
            }
        }

        private void InventoryIcon_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DrawablesGenerator generator = new DrawablesGenerator(imagePath);

                generator = DrawableUtilities.SetUpGenerator(generator, "0", "0", tbxIgnoreColor.Text);

                DrawablesOutput output = generator.Generate();
                
                (new OutputWindow("Inventory Icon:", DrawableUtilities.GenerateInventoryIcon(output))).Show();
            }
            catch (FormatException)
            {
                MessageBox.Show("Invalid format. Did you provide a correct ignore color code? (hexadecimal RRGGBB or RRGGBBAA)");
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("Argument may not be null. Did you select a valid image?");
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
                DrawablesGenerator generator = new DrawablesGenerator(imagePath);

                generator = DrawableUtilities.SetUpGenerator(generator, tbxHandX.Text, tbxHandY.Text, tbxIgnoreColor.Text);
                generator.ReplaceWhite = true;

                DrawablesOutput output = generator.Generate();

                (new OutputWindow("Item Command:", (GetExporter(output)).GetCommand(chkAddWeaponGroup.IsChecked.HasValue && chkAddWeaponGroup.IsChecked.Value ? "weapon" : null))).Show();
            }
            catch (NotImplementedException exc)
            {
                MessageBox.Show(exc.Message);
                return;
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("Argument may not be null. Did you select a valid image?");
            }
            catch (FormatException)
            {
                MessageBox.Show("Could not convert hand offsets to numbers.");
                return;
            }
            catch (DrawableException exc)
            {
                MessageBox.Show(exc.Message);
                return;
            }
        }

        #endregion

        private Exporter GetExporter(DrawablesOutput output)
        {
            switch ((string)cbxGenerateType.SelectedValue)
            {
                default:
                case "Common Pistol":
                    return new PistolExporter(output);
                case "Common Shortsword":
                    return new ShortswordExporter(output);
                case "Tesla Staff":
                    return new TeslaStaffExporter(output);
            }
        }
    }
}
