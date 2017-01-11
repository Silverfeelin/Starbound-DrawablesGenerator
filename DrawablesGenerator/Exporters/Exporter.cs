using System;
using System.IO;
using System.Windows;
using Newtonsoft.Json.Linq;
using Silverfeelin.StarboundDrawables;

namespace DrawablesGeneratorTool
{
    public abstract class Exporter : IExporter
    {
        protected readonly DrawablesOutput output;

        public abstract string Template { get; }

        public Exporter(DrawablesOutput output)
        {
            this.output = output;
        }

        public virtual JObject GetDescriptor(string group, bool addInventoryIcon)
        {
            JObject descriptor = CreateDescriptor(Template, group, addInventoryIcon);
            return descriptor;
        }
        
        public virtual string GetCommand(string group, bool addInventoryIcon)
        {
            JObject descriptor = CreateDescriptor(Template, group, addInventoryIcon);
            return GenerateCommand(descriptor);
        }

        public void Export(string path, string contents)
        {
            if (File.Exists(path))
            {
                MessageBoxResult mbr = MessageBox.Show("The file already exists. Do you want to overwrite it?", "Warning", MessageBoxButton.YesNo);
                if (mbr != MessageBoxResult.Yes)
                    return;
            }

            File.WriteAllText(path, contents);
        }

        protected string GenerateCommand(JObject descriptor)
        {
            string output = string.Format("/spawnitem {0} 1 '{1}'",
                descriptor["name"].Value<string>(),
                descriptor["parameters"].ToString(Newtonsoft.Json.Formatting.None));
            return output;
        }

        protected JObject CreateDescriptor(string template, string group = "weapon", bool addInventoryIcon = false)
        {
            JObject descriptor = JObject.Parse(template);

            if (descriptor["name"] == null)
                descriptor["name"] = "perfectlygenericitem";

            if (descriptor["count"] == null)
                descriptor["count"] = 1;

            if (descriptor["parameters"] == null)
                descriptor["parameters"] = new JObject();

            JObject parameters = (JObject)descriptor["parameters"];

            if (parameters == null)
                parameters = new JObject();
            if (parameters["animationCustom"] == null)
                parameters["animationCustom"] = new JObject();
            if (parameters["animationCustom"]["animatedParts"] == null)
                parameters["animationCustom"]["animatedParts"] = new JObject();
            if (parameters["animationCustom"]["animatedParts"]["parts"] == null)
                parameters["animationCustom"]["animatedParts"]["parts"] = new JObject();

            JToken parts = parameters["animationCustom"]["animatedParts"]["parts"];

            string prefix = "D_";
            int i = 1;

            JArray groups = new JArray();
            if (!string.IsNullOrEmpty(group))
                groups.Add(group);

            foreach (Drawable item in output.Drawables)
            {
                if (item == null) continue;
                JObject part = JObject.Parse("{'properties':{'centered':false,'offset':[0,0]}}");
                part["properties"]["image"] = item.ResultImage;
                part["properties"]["offset"][0] = item.BlockX + Math.Round(output.OffsetX / 8d, 3);
                part["properties"]["offset"][1] = item.BlockY + Math.Round(output.OffsetY / 8d, 3);
                part["properties"]["transformationGroups"] = groups;

                parts[prefix + i++] = part;
            }
            
            if (addInventoryIcon)
            {
                parameters["inventoryIcon"] = DrawableUtilities.GenerateInventoryIcon(output);
            }
            
            return descriptor;
        }
    }
}
