using System.IO;
using System.Linq;

namespace MetricsExtractor
{
    public class MetricConfiguration
    {
        public MetricConfiguration()
        {
            IgnoredTypes = IgnoredNamespaces = IgnoredProjects = Enumerable.Empty<string>().ToArray();
        }
        public string Solution { get; set; }

        public string SolutionDirectory { get { return Path.GetDirectoryName(Solution); } }

        public string[] IgnoredProjects { get; set; }

        public string[] IgnoredNamespaces { get; set; }

        public string[] IgnoredTypes { get; set; }
    }
}
