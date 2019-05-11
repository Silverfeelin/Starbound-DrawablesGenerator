using Silverfeelin.StarboundDrawables;

namespace DrawablesGeneratorTool.Exporters.ItemExporters
{
    public class ShortswordExporter : Exporter
    {
        public override string Template => Properties.Resources.Sword;

        public ShortswordExporter(DrawablesOutput output) : base(output)
        {
        }
    }
}
