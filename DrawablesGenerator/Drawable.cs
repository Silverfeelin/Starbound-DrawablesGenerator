using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawablesGenerator
{
    public class Drawable
    {
        /// <summary>
        /// Gets or sets the texture (image) of this drawable.
        /// Should be a valid asset path.
        /// </summary>
        public string Texture { get; set; }

        /// <summary>
        /// Gets or sets the directives of this drawable.
        /// </summary>
        public string Directives { get; set; }

        /// <summary>
        /// Gets the texture plus directives, which form this drawable.
        /// </summary>
        public string ResultImage
        {
            get
            {
                return this.Texture + this.Directives;
            }
        }

        /// <summary>
        /// Gets or sets the horizontal position for this drawable, in game pixels.
        /// </summary>
        public int PixelX { get; set; }

        /// <summary>
        /// Gets the horizontal position for this drawable, in blocks.
        /// A block is 8 game pixels by default.
        /// </summary>
        public double BlockX
        {
            get
            {
                return Math.Round((double)PixelX / 8d, 3);
            }
        }

        /// <summary>
        /// Gets or sets the vertical position for this drawable, in game pixels.
        /// </summary>
        public int PixelY { get; set; }

        /// <summary>
        /// Gets the vertical position for this drawable, in blocks.
        /// A block is 8 game pixels by default.
        /// </summary>
        public double BlockY
        {
            get
            {
                return Math.Round((double)PixelY / 8d, 3);
            }
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="texture">Asset path to the texture for this drawable.</param>
        /// <param name="directives">Directives to apply to the texture.</param>
        /// <param name="x">Horizontal position, in game pixels.</param>
        /// <param name="y">Vertical position, in game pixels.</param>
        public Drawable(string texture, string directives, int x, int y)
        {
            this.Texture = texture;
            this.Directives = directives;
            this.PixelX = x;
            this.PixelY = y;
        }
    }
}
