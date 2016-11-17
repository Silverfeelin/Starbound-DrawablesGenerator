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

        public virtual JObject GetDescriptor(string group)
        {
            JObject descriptor = GetActiveItemDescriptor(Template, group);
            ApplyParameters(descriptor);
            return descriptor;
        }

        public virtual string GetCommand(string group)
        {
            JObject descriptor = GetActiveItemDescriptor(Template, group);
            ApplyParameters(descriptor);
            return GenerateCommand(descriptor);
        }

        public abstract void ApplyParameters(JObject descriptor);

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

        protected JObject GetActiveItemDescriptor(string template, string group = "weapon")
        {
            JObject parameters = JObject.Parse(template);
            JToken parts = parameters["animationCustom"]["animatedParts"]["parts"];

            string prefix = "D_";
            int i = 1;

            JArray groups = new JArray();
            if (!string.IsNullOrEmpty(group))
                groups.Add(group);

            foreach (Drawable item in output.Drawables)
            {
                JObject part = JObject.Parse("{'properties':{'centered':false,'offset':[0,0]}}");
                part["properties"]["image"] = item.ResultImage;
                part["properties"]["offset"][0] = item.BlockX + Math.Round(output.OffsetX / 8d, 3);
                part["properties"]["offset"][1] = item.BlockY + Math.Round(output.OffsetY / 8d, 3);
                part["properties"]["transformationGroups"] = groups;

                parts[prefix + i++] = part;
            }

            JObject descriptor = new JObject();
            descriptor["name"] = "perfectlygenericitem";
            descriptor["count"] = 1;
            descriptor["parameters"] = parameters;

            return descriptor;
        }

        public void Copy(string contents)
        {
            Clipboard.SetText(contents);
        }
    }
}
