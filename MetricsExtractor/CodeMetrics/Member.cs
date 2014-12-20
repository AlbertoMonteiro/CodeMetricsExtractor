using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace MetricsExtractor.CodeMetrics
{
    /// <remarks />
    [XmlType(AnonymousType = true)]
    public class Member
    {
        /// <remarks />
        [XmlArrayItem("Metric", IsNullable = false)]
        public Metric[] Metrics { get; set; }

        [XmlIgnore]
        public Dictionary<MetricKind, double> MetricsDic
        {
            get
            {
                return Metrics.ToDictionary(x => x.Kind, x => x.Value);
            }
        }

        /// <remarks />
        [XmlAttribute]
        public string Name { get; set; }

        /// <remarks />
        [XmlAttribute]
        public string File { get; set; }

        /// <remarks />
        [XmlAttribute]
        public ushort Line { get; set; }

        /// <remarks />
        [XmlIgnore]
        public bool LineSpecified { get; set; }
    }
}