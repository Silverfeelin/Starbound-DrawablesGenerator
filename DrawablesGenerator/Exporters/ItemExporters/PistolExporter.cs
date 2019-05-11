using Silverfeelin.StarboundDrawables;

namespace DrawablesGeneratorTool.Exporters.ItemExporters
{
    public class PistolExporter : Exporter
    {
        public override string Template => Properties.Resources.Gun;

        public PistolExporter(DrawablesOutput output) : base(output)
        {
        }
    }
}
