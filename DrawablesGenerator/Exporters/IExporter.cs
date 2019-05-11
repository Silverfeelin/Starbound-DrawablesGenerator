using Newtonsoft.Json.Linq;

namespace DrawablesGeneratorTool.Exporters
{
    public interface IExporter
    {
        string Template { get; }
        JObject GetDescriptor(string group, bool addInventoryIcon);
        string GetCommand(string group, bool addInventoryIcon);
    }
}
