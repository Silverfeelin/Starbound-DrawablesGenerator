using Newtonsoft.Json.Linq;
using Silverfeelin.StarboundDrawables;
using System.IO;

namespace DrawablesGeneratorTool
{
    public class TemplateExporter : Exporter, IExporter
    {
        private string template;
        public override string Template
        {
            get
            {
                return template;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="output"></param>
        /// <param name="template"></param>
        public TemplateExporter(DrawablesOutput output, string template) : base(output)
        {
            this.template = template;
        }
    }
}
