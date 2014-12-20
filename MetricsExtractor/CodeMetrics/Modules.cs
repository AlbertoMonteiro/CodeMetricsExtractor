using System.Xml.Serialization;

namespace MetricsExtractor.CodeMetrics
{
    /// <remarks />
    [XmlType(AnonymousType = true)]
    public class Modules
    {
        /// <remarks />
        public Module Module { get; set; }
    }
}