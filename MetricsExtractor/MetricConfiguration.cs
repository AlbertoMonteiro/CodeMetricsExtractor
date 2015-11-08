using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using DocoptNet;
using Newtonsoft.Json;
using static System.IO.Path;

namespace MetricsExtractor
{
    public class MetricConfiguration
    {
        private readonly IDictionary<string, ValueObject> _arguments;

        public MetricConfiguration(IDictionary<string, ValueObject> arguments)
        {
            _arguments = arguments;
            if (!string.IsNullOrWhiteSpace(JsonConfigFile))
            {
                var converted = JsonConvert.DeserializeAnonymousType(File.ReadAllText(JsonConfigFile), new { IgnoredProjects, IgnoredNamespaces, IgnoredTypes });
                IgnoredProjects = converted.IgnoredProjects;
                IgnoredNamespaces = converted.IgnoredNamespaces;
                IgnoredTypes = converted.IgnoredTypes;
            }
            else
            {
                IgnoredProjects = GetImmutableArray("ignoredProjects");
                IgnoredNamespaces = GetImmutableArray("ignoredNamespaces");
                IgnoredTypes = GetImmutableArray("ignoredTypes");
            }
        }

        public string Solution => _arguments["<solution>"].ToString();

        public string SolutionDirectory => GetDirectoryName(Solution);

        public ImmutableArray<string> IgnoredProjects { get; }

        public ImmutableArray<string> IgnoredNamespaces { get; }

        public ImmutableArray<string> IgnoredTypes { get; }

        public string JsonConfigFile => _arguments["<jsonfileconfig>"].ToString();

        private ImmutableArray<string> GetImmutableArray(string key)
            => _arguments[$"<{key}>"] != null
                ? _arguments[$"<{key}>"].ToString().Split(';').ToImmutableArray()
                : ImmutableArray<string>.Empty;
    }
}
