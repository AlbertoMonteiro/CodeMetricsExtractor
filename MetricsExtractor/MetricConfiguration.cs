using System.Collections.Generic;
using System.Collections.Immutable;
using DocoptNet;
using static System.IO.Path;

namespace MetricsExtractor
{
    public class MetricConfiguration
    {
        private readonly IDictionary<string, ValueObject> _arguments;

        public MetricConfiguration(IDictionary<string, ValueObject> arguments)
        {
            _arguments = arguments;
        }

        public string Solution => _arguments["<solution>"].ToString();

        public string SolutionDirectory => GetDirectoryName(Solution);

        public ImmutableArray<string> IgnoredProjects => GetImmutableArray("ignoredProjects");

        public ImmutableArray<string> IgnoredNamespaces => GetImmutableArray("ignoredNamespaces");

        public ImmutableArray<string> IgnoredTypes => GetImmutableArray("ignoredTypes");

        private ImmutableArray<string> GetImmutableArray(string key)
            => _arguments[$"<{key}>"] != null
                ? _arguments[$"<{key}>"].ToString().Split(';').ToImmutableArray()
                : ImmutableArray<string>.Empty;
    }
}
