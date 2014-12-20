using System.Xml.Serialization;

namespace MetricsExtractor.CodeMetrics
{
    /// <remarks />
    [XmlType(AnonymousType = true)]
    public class Module
    {
        /// <remarks />
        [XmlArrayItem("Metric", IsNullable = false)]
        public Metric[] Metrics { get; set; }

        /// <remarks />
        [XmlArrayItem("Namespace", IsNullable = false)]
        public Namespace[] Namespaces { get; set; }

        /// <remarks />
        [XmlAttribute]
        public string Name { get; set; }

        /// <remarks />
        [XmlAttribute]
        public string AssemblyVersion { get; set; }

        /// <remarks />
        [XmlAttribute]
        public string FileVersion { get; set; }
    }
}