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

        public double BlockOffsetX { get; set; }

        public double BlockOffsetY { get; set; }

        public DrawableOutput(List<Drawable> drawables) : this(drawables, 0, 0)
        {
        }

        public DrawableOutput(List<Drawable> drawables, double blockOffsetX, double blockOffsetY)
        {
            this.Drawables = drawables;
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
    }
}
