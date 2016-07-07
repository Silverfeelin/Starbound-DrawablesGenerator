using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawablesGenerator
{
    public class DrawableOutput
    {
        public List<Drawable> Drawables { get; }

        public int ImageWidth { get; set; }

        public int ImageHeight { get; set; }

        public double BlockOffsetX { get; set; }

        public double BlockOffsetY { get; set; }

        public DrawableOutput(List<Drawable> drawables, int imageWidth, int imageHeight) : this(drawables, imageWidth, imageHeight, 0, 0)
        {
        }

        public DrawableOutput(List<Drawable> drawables, int imageWidth, int imageHeight, double blockOffsetX, double blockOffsetY)
        {
            this.Drawables = drawables;
            this.ImageWidth = imageWidth;
            this.ImageHeight = imageHeight;
            this.BlockOffsetX = blockOffsetX;
            this.BlockOffsetY = blockOffsetY;
        }
        
        public string GenerateCommand()
        {
            string command = "/spawnitem teslastaff 1 '{0}'";

            string content = GenerateText().Replace(Environment.NewLine, "").Replace(" ", "");
            return string.Format(command, content);
        }

        public string GenerateText()
        {
            JObject parameters = JObject.Parse(Properties.Resources.ActiveItem);
            JToken parts = parameters["animation"]["animatedParts"]["parts"];

            string prefix = "D_";
            int i = 1;

            foreach (Drawable item in Drawables)
            {
                JObject part = JObject.Parse("{'properties':{'centered':false,'image':'/assetMissing.png','offset':[0,0]}}");
                part["properties"]["image"] = item.ResultImage;
                part["properties"]["offset"][0] = item.BlockX + BlockOffsetX;
                part["properties"]["offset"][1] = item.BlockY + BlockOffsetY;
                parts[prefix + i++] = part;
            }

            return parameters.ToString(Newtonsoft.Json.Formatting.Indented);
        }

        public string GenerateSingleTextureDirectives(int scaleBase = 64)
        {
            int w = ImageWidth,
                h = ImageHeight;
            int max = w > h ? w : h;
            int scale = (int)Math.Ceiling((double)max / scaleBase);

            string dir = string.Format("?replace;00000000=ffffff;ffffff00=ffffff?setcolor=ffffff?scalenearest={0}?crop=0;0;{1};{2}", scale, w, h);

            foreach (var item in Drawables)
            {
                dir += string.Format("?blendmult={0};{1};{2}{3}", item.Texture, -item.PixelX, -item.PixelY, item.Directives);
            }

            return dir;
        }
    }
}
