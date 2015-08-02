using System.Linq;
using static System.IO.Path;
using static System.Linq.Enumerable;

namespace MetricsExtractor
{
    public class MetricConfiguration
    {
        public MetricConfiguration()
        {
            IgnoredTypes = IgnoredNamespaces = IgnoredProjects = Empty<string>().ToArray();
        }
        public string Solution { get; set; }

        public string SolutionDirectory => GetDirectoryName(Solution);

        public string[] IgnoredProjects { get; set; }

        public string[] IgnoredNamespaces { get; set; }

        public string[] IgnoredTypes { get; set; }
    }
}
