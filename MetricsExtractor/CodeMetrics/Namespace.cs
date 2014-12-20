using System.Xml.Serialization;

namespace MetricsExtractor.CodeMetrics
{
    /// <remarks />
    [XmlType(AnonymousType = true)]
    public class Namespace
    {
        /// <remarks />
        [XmlArrayItem("Metric", IsNullable = false)]
        public Metric[] Metrics { get; set; }

        /// <remarks />
        [XmlArrayItem("Type", IsNullable = false)]
        public NamespaceType[] Types { get; set; }

        /// <remarks />
        [XmlAttribute]
        public string Name { get; set; }
    }
}