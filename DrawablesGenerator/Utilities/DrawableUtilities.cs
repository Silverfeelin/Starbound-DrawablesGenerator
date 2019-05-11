using System;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Silverfeelin.StarboundDrawables;

namespace DrawablesGeneratorTool.Utilities
{
    public static class DrawableUtilities
    {
        /// <summary>
        /// Returns whether the given string is a valid positive or negative number.
        /// </summary>
        /// <param name="value">The string to check for validity</param>
        /// <returns>True if the given string is a valid positive or negative integer.</returns>
        public static bool IsNumber(string value)
        {
            var regex = new Regex("^-?[0-9]+$");
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
            return value < minimum ? minimum : value > maximum ? maximum : value;
        }

        /// <summary>
        /// Transforms the directives from a <see cref="DrawablesOutput"/> into a single directives string.
        /// Requires the given output to be generated with ReplaceBlank and ReplaceWhite set to true.
        /// </summary>
        /// <param name="output">Output to read directives from.</param>
        /// <param name="baseScale">Base scale. Should be the biggest dimension in the source image this string will be applied to by the user.
        /// EG. If the user wishes to apply a drawable bigger than 30x50 to a 30x50 object, the base scale should be 50.</param>
        /// <param name="fade">Apply a small fade at the end of every drawable. This results in near-identical visuals but allows
        /// for skipping over transparent pixels.</param>
        /// <returns>Directives string.</returns>
        public static string GenerateSingleTextureDirectives(DrawablesOutput output, int baseScale = 64, bool fade = false)
        {
            int w = output.ImageWidth,
                h = output.ImageHeight;

            var max = w > h ? w : h;
            var scale = (int)Math.Ceiling((double)max / baseScale);

            var dir = new StringBuilder();
            dir.AppendFormat("?setcolor=fff?replace;fff0=fff?scalenearest={0}?crop=0;0;{1};{2}", scale, w, h);

            foreach (var drawable in output.Drawables)
            {
                if (drawable == null) continue;

                dir.AppendFormat("?blendmult={0};{1};{2}{3}", drawable.Texture, -drawable.X, -drawable.Y, drawable.Directives);

                if (fade)
                    dir.Append("?fade;80ff80;0.0001518");
            }
            dir.Append("?replace;fff=0000");

            return dir.ToString();
        }

        /// <summary>
        /// Sets up the properties of an instantiated generator, using the given parameters.
        /// </summary>
        /// <param name="generator">Generator to set up.</param>
        /// <param name="handX">String value for the horizontal hand offset in pixels, presumably from a text field.
        /// Value should be convertable to an integer.</param>
        /// <param name="handY">String value for the vertical hand offset in pixels, presumably from a text field.
        /// Value should be convertable to an integer.</param>
        /// <param name="ignoreColor">String value of the color to ignore, presumably from a text field.
        /// If given, value should be formatted RRGGBB or RRGGBBAA (hexadecimal string).</param>
        /// <returns>Reference to the given object.</returns>
        public static DrawablesGenerator SetUpGenerator(DrawablesGenerator generator, string handX, string handY, string ignoreColor)
        {
            generator.OffsetX = Convert.ToInt32(handX) + 1;
            generator.OffsetY = Convert.ToInt32(handY);

            generator.RotateFlipStyle = System.Drawing.RotateFlipType.RotateNoneFlipY;

            generator.ReplaceBlank = true;
            generator.ReplaceWhite = true;

            var colorString = ignoreColor.Replace("#", "");
            if (colorString.Length != 6 && colorString.Length != 8) return generator;

            var r = colorString.Substring(0, 2).HexToInt();
            r = Clamp(r, 0, 255);
            var g = colorString.Substring(2, 2).HexToInt();
            g = Clamp(g, 0, 255);
            var b = colorString.Substring(4, 2).HexToInt();
            b = Clamp(b, 0, 255);
            var a = colorString.Length == 8 ? colorString.Substring(6, 2).HexToInt() : 255;
            a = Clamp(a, 0, 255);

            generator.IgnoreColor = System.Drawing.Color.FromArgb(a, r, g, b);

            return generator;
        }

        public static JArray GenerateInventoryIcon(DrawablesOutput output)
        {
            var drawables = new JArray();

            for (var i = 0; i < output.Drawables.GetLength(0); i++)
            {
                for (var j = 0; j < output.Drawables.GetLength(1); j++)
                {
                    var item = output.Drawables[i,j];
                    if (item == null) continue;

                    var drawable = new JObject
                    {
                        ["image"] = item.ResultImage
                    };

                    bool cropH = false, cropV = false;
                    int hRest = 0, vRest = 0;
                    if (i == output.Drawables.GetLength(0) - 1)
                    {
                        hRest = output.ImageWidth % 32;
                        if (hRest != 0)
                            cropH = true;
                    }
                    if (j == output.Drawables.GetLength(1) - 1)
                    {
                        vRest = output.ImageHeight % 8;
                        if (vRest != 0)
                            cropV = true;
                    }
                    
                    if (cropH || cropV)
                    {
                        
                        drawable["image"] += "?crop;0;0;" + (cropH ? hRest : 32) + ";" + (cropV ? vRest : 8);
                    }

                    var position = new JArray { item.X, item.Y };
                    drawable["position"] = position;
                    drawables.Add(drawable);
                }
            }
            return drawables;
        }
    }
}
