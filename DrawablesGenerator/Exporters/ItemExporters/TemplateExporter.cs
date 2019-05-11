using Silverfeelin.StarboundDrawables;

namespace DrawablesGeneratorTool.Exporters.ItemExporters
{
    public class TemplateExporter : Exporter
    {
        public override string Template { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="output"></param>
        /// <param name="template"></param>
        public TemplateExporter(DrawablesOutput output, string template) : base(output)
        {
            Template = template;
        }
    }
}
