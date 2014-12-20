using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace MetricsExtractor.CodeMetrics
{
    /// <remarks />
    [XmlType(AnonymousType = true)]
    public class Metric
    {
        [XmlAttribute("Name")]
        public MetricKind Kind { get; set; }

        /// <remarks />
        [XmlAttribute("Value")]
        public string StringValue { get; set; }

        public double Value { get { return double.Parse(Regex.Replace(StringValue, @"[^\d]", "")); } }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Kind, Value);
        }
    }

    public enum MetricKind
    {
        MaintainabilityIndex,
        CyclomaticComplexity,
        ClassCoupling,
        DepthOfInheritance,
        LinesOfCode
    }
}