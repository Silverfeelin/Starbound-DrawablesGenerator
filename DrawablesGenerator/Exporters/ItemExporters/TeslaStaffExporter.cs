using Silverfeelin.StarboundDrawables;

namespace DrawablesGeneratorTool.Exporters.ItemExporters
{
    public class TeslaStaffExporter : Exporter
    {
        public override string Template => Properties.Resources.TeslaStaff;

        public TeslaStaffExporter(DrawablesOutput output) : base(output)
        {
        }
    }
}
