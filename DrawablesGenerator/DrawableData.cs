using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace DrawablesGenerator
{
    public class DrawableData
    {
        /// <summary>
        /// Highest amount of allowed pixels in the selected image.
        /// PixelWidth * PixelHeight should not exceed this value.
        /// </summary>
        public static readonly int PIXEL_LIMIT = 32768;

        private Bitmap selectedImage = null;
        /// <summary>
        /// Returns the currently selected image, or null if no image is selected.
        /// </summary>
        public Bitmap SelectedImage
        {
            get
            {
                return this.selectedImage;
            }
        }

        private string selectedImagePath = null;
        /// <summary>
        /// Returns the path to the currently selected image, or null if no image is selected.
        /// </summary>
        public string SelectedImagePath
        {
            get
            {
                return this.selectedImagePath;
            }
        }

        /// <summary>
        /// Returns a value indicating whether a valid image is currently selected.
        /// </summary>
        public bool ValidImage
        {
            get
            {
                return this.selectedImage != null;
            }
        }

        /// <summary>
        /// Creates a new <see cref="DrawableData"/> instance.
        /// </summary>
        public DrawableData()
        {

        }

        /// <summary>
        /// Destructor
        /// Unlocks selected image resources if the reference to this object instance is lost.
        /// </summary>
        ~DrawableData()
        {
            ResetImage();
        }

        /// <summary>
        /// Loads the given image, and checks it for validity.
        /// </summary>
        /// <param name="path">Absolute path to a valid image file.</param>
        /// <exception cref="DrawableException">Thrown when image dimensions exceed <see cref="PIXEL_LIMIT"/>.</exception>
        public void LoadImage(string path)
        {
            ResetImage();

            this.selectedImage = new Bitmap(path);

            int width = this.selectedImage.Width,
                height = this.selectedImage.Height;
            int pixels = width * height;

            if (pixels > PIXEL_LIMIT)
            {
                ResetImage();
                throw new DrawableException(string.Format("The selected image exceeds the limit of {0} pixels.\n- Width: {1}\n- Height: {2}\n- Pixels: {3}", PIXEL_LIMIT, width, height, pixels));
            }

            this.selectedImagePath = path;
        }
        
        /// <summary>
        /// Clears the currently selected image and image path. Disposes the image to unlock the resource used.
        /// </summary>
        public void ResetImage()
        {
            if (this.selectedImage != null)
            {
                this.selectedImage.Dispose();
                this.selectedImage = null;
            }
            this.selectedImagePath = null;
        }

        /// <summary>
        /// Returns whether the given string is a valid positive or negative intregal number.
        /// </summary>
        /// <param name="value">The string to check for validity</param>
        /// <returns>True if the given string is a valid positive or negative integer.</returns>
        public static bool IsNumber(string value)
        {
            Regex regex = new Regex("^-?[0-9]+$");

            return regex.IsMatch(value);
        }

        /// <summary>
        /// Returns the value within the minimum and maximum bounds.
        /// If the value is smaller than the minimum value, or bigger than the maximum value, return that value instead.
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <param name="minimum">The lowest acceptable value</param>
        /// <param name="maximum">The highest acceptable value</param>
        /// <returns>The value if it's between minimum and maximum, minimum if smaller or maximum if larger.</returns>
        public static int Clamp(int value, int minimum, int maximum)
        {
            if (value < minimum)
            {
                return minimum;
            }
            else if (value > minimum)
            {
                return maximum;
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// Returns a hexadecimal color string from a System.Drawing.Color, formatted 'RRGGBBAA'.
        /// </summary>
        /// <param name="c">The System.Drawing.Color to convert</param>
        /// <returns>Hexadecimal color string, formatted 'RRGGBBAA'</returns>
        private string ColorToRGBAHexString(Color c)
        {
            string r = c.R.ToString("X");
            string g = c.G.ToString("X");
            string b = c.B.ToString("X");
            string a = c.A.ToString("X");

            if (r.Length == 1)
            {
                r = 0 + r;
            }

            if (g.Length == 1)
            {
                g = 0 + g;
            }

            if (b.Length == 1)
            {
                b = 0 + b;
            }

            if (a.Length == 1)
            {
                a = 0 + a;
            }

            return r + g + b + a;
        }

        /// <summary>
        /// Returns a System.Drawing.Color from a hexadecimal color string, formatted 'RRGGBB' or 'RRGGBBAA'.
        /// </summary>
        /// <param name="rgba">Hexadecimal color string, formatted 'RRGGBB' or 'RRGGBBAA'</param>
        /// <returns>System.Drawing.Color for the given color string</returns>
        public static Color RGBAHexStringToColor(string rgba)
        {
            if (rgba.Length == 6 || rgba.Length == 8)
            {
                int r = HexToInt(rgba.Substring(0, 2)),
                    g = HexToInt(rgba.Substring(2, 2)),
                    b = HexToInt(rgba.Substring(4, 2)),
                    a;

                if (rgba.Length == 8)
                {
                    a = HexToInt(rgba.Substring(6, 2));
                }
                else
                {
                    a = 255;
                }

                if (r == -1 || g == -1 || b == -1 || a == -1)
                {
                    return Color.Transparent;
                }

                return Color.FromArgb(r, g, b, a);
            }
            else
            {
                return Color.Transparent;
            }
        }

        /// <summary>
        /// Returns the integral value of the given hexadecimal number.
        /// </summary>
        /// <param name="hex">Hexadecimal number string</param>
        /// <returns>Converted Integer or -1 if the conversion failed.</returns>
        public static int HexToInt(string hex)
        {
            try
            {
                uint number = Convert.ToUInt32(hex, 16);
                return (int)number;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// Generates a <see cref="DrawableOutput"/> instance containing all <see cref="Drawable"/> instances needed to form <see cref="selectedImage"/>.
        /// </summary>
        /// <param name="blockOffsetX"></param>
        /// <param name="blockOffsetY"></param>
        /// <param name="replaceAll">Value indicating whether to add a replace directive for transparent pixels. Signs with no (semi)opaque pixels are still ignored.</param>
        /// <returns>DrawableOutput containing the drawable data.</returns>
        /// <exception cref="DrawableException">Thrown when no valid image has been selected.</exception>
        public DrawableOutput GenerateDrawables(double blockOffsetX, double blockOffsetY, bool replaceBlank = false, string ignoreColor = null)
        {
            if (!this.ValidImage)
                throw new DrawableException("Please select a valid image before creating Drawables.");

            Color ignore = Color.Transparent;
            if (!string.IsNullOrEmpty(ignoreColor))
            {
                if (ignoreColor.IndexOf("#") != 0)
                    ignoreColor = "#" + ignoreColor;

                try
                {
                    ignore = System.Drawing.ColorTranslator.FromHtml(ignoreColor);
                }
                catch (Exception exc)
                {
                    Console.WriteLine("Value {0} is not a correct hexadecimal color code.", ignoreColor);
                }
            }

            Bitmap template = new Bitmap(32,8);
            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    int xi = i;

                    // Compensate for missing hexadecimal values (A/F).
                    if (i >= 9)
                        xi += 6;
                    if (i >= 19)
                        xi += 6;
                    if (i >= 29)
                        xi += 6;

                    template.SetPixel(i, j, Color.FromArgb(1, xi+1, 0, j+1));
                }
            }

            selectedImage.RotateFlip(RotateFlipType.RotateNoneFlipY);
            Bitmap b = (Bitmap)selectedImage.Clone();
            selectedImage.RotateFlip(RotateFlipType.RotateNoneFlipY);

            List<Drawable> drawables = new List<Drawable>();

            Point frameCount = new Point(
                 (int)Math.Ceiling((decimal)b.Width / 32),
                 (int)Math.Ceiling((decimal)b.Height / 8));

            Point imagePixel = new Point(0, 0);

            // Add a drawable for every signplaceholder needed.
            for (int frameWidth = 0; frameWidth < frameCount.X; frameWidth++)
            {
                for (int frameHeight = 0; frameHeight < frameCount.Y; frameHeight++)
                {
                    imagePixel = new Point(frameWidth * 32, frameHeight * 8);

                    bool containsPixels = false;

                    string texture = "/objects/outpost/customsign/signplaceholder.png";
                    string directives = "?replace";

                    for (int i = 0; i < 32; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            // Pixel falls within template but is outside of the supplied image.
                            if ((imagePixel.X > b.Width - 1 || imagePixel.Y > b.Height - 1))
                            {
                                imagePixel.Y++;
                                continue;
                            }
                            
                            Color imageColor = b.GetPixel(Convert.ToInt32(imagePixel.X), Convert.ToInt32(imagePixel.Y));

                            // Pixel color is invisible or ignored.
                            if (imageColor.Equals(ignore) || (imageColor.A < 1 && !replaceBlank))
                            {
                                imagePixel.Y++;
                                continue;
                            }
                            else if (replaceBlank && imageColor.ToArgb() == Color.White.ToArgb())
                                imageColor = Color.FromArgb(255, 254, 254, 254);

                            Color templateColor = template.GetPixel(i, j);

                            directives += string.Format(";{0}={1}", ColorToRGBAHexString(templateColor), ColorToRGBAHexString(imageColor));

                            if (imageColor.A > 1)
                                containsPixels = true;

                            imagePixel.Y++;
                        }

                        imagePixel.X++;
                        imagePixel.Y = frameHeight * 8;
                    }

                    int xb = Convert.ToInt32(frameWidth * 32),
                        yb = Convert.ToInt32(frameHeight * 8);

                    if (containsPixels)
                        drawables.Add(new Drawable(texture, directives, xb, yb));
                }
            }

            return new DrawableOutput(drawables, selectedImage.Width, selectedImage.Height, blockOffsetX, blockOffsetY);
        }
    }
}
