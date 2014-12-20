
using System.IO;

namespace MetricsExtractor
{
    public class MetricConfiguration
    {
        public string Solution { get; set; }

        public string SolutionDirectory { get { return Path.GetDirectoryName(Solution); } }

        public string[] IgnoredProjects { get; set; }
    }
}
