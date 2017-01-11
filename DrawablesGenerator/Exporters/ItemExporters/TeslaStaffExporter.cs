using Newtonsoft.Json.Linq;
using Silverfeelin.StarboundDrawables;

namespace DrawablesGeneratorTool
{
    public class TeslaStaffExporter : Exporter, IExporter
    {
        public override string Template
        {
            get
            {
                return Properties.Resources.TeslaStaff;
            }
        }

        public TeslaStaffExporter(DrawablesOutput output) : base(output)
        {
        }

        public override void ApplyParameters(JObject descriptor)
        {
            descriptor["name"] = "teslastaff";
            descriptor["parameters"]["shortdescription"] = "Drawable Staff";
        }
    }
}
