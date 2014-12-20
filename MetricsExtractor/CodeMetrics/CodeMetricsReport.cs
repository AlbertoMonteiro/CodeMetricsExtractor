using System.Xml.Serialization;

namespace MetricsExtractor.CodeMetrics
{
    /// <remarks />
    [XmlType(AnonymousType = true), XmlRoot(Namespace = "", IsNullable = false)]
    public class CodeMetricsReport
    {
        /// <remarks />
        [XmlArrayItem("Target", IsNullable = false)]
        public Target[] Targets { get; set; }

        /// <remarks />
        [XmlAttribute]
        public double Version { get; set; }
    }
}
