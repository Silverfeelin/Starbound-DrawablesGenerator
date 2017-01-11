using Newtonsoft.Json.Linq;
using Silverfeelin.StarboundDrawables;

namespace DrawablesGeneratorTool
{
    public class PistolExporter : Exporter, IExporter
    {
        public override string Template
        {
            get
            {
                return Properties.Resources.Gun;
            }
        }

        public PistolExporter(DrawablesOutput output) : base(output)
        {
        }
    }
}
