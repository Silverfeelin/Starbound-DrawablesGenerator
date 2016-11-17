using Newtonsoft.Json.Linq;

namespace DrawablesGeneratorTool
{
    public interface IExporter
    {
        string Template { get; }
        JObject GetDescriptor(string group);
        string GetCommand(string group);
        void ApplyParameters(JObject descriptor);
    }
}
