using System.Xml.Serialization;

namespace MetricsExtractor.CodeMetrics
{
    /// <remarks />
    [XmlType(AnonymousType = true)]
    public class NamespaceType
    {
        /// <remarks />
        [XmlArrayItem("Metric", IsNullable = false)]
        public Metric[] Metrics { get; set; }

        /// <remarks />
        [XmlArrayItem("Member", IsNullable = false)]
        public Member[] Members { get; set; }

        /// <remarks />
        [XmlAttribute]
        public string Name { get; set; }
        public string FullName { get { return string.Format("{0}.{1}", Namespace, Name); } }
        public string Namespace { get; set; }

        public NamespaceType WithNamespace(string @namespace)
        {
            Namespace = @namespace;
            return this;
        }
    }
}