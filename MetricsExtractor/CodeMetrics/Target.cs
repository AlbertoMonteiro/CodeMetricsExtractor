using System.IO;
using System.Xml.Serialization;

namespace MetricsExtractor.CodeMetrics
{
    /// <remarks />
    [XmlType(AnonymousType = true)]
    public class Target
    {
        /// <remarks />
        public Modules Modules { get; set; }

        /// <remarks />
        [XmlAttribute]
        public string Name { get; set; }

        public override string ToString()
        {
            return Path.GetFileNameWithoutExtension(Name);
        }
    }
}