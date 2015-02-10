using System.Collections.Generic;
using ArchiMetrics.Common.Metrics;

namespace MetricsExtractor.Custom
{
    public class TypeMetricWithNamespace : ITypeMetric
    {
        public TypeMetricWithNamespace(ITypeMetric typeMetric)
        {
            var type = typeMetric.GetType();
            var myType = typeof(TypeMetricWithNamespace);
            foreach (var property in type.GetProperties())
            {
                var prop = myType.GetProperty(property.Name);
                prop.SetValue(this, property.GetValue(typeMetric));
            }
        }

        public IEnumerable<ITypeCoupling> ClassCouplings { get; private set; }

        public int LinesOfCode { get; private set; }

        public int SourceLinesOfCode { get; private set; }

        public double MaintainabilityIndex { get; private set; }

        public int CyclomaticComplexity { get; private set; }

        public string Name { get; private set; }

        public AccessModifierKind AccessModifier { get; private set; }

        public TypeMetricKind Kind { get; private set; }

        public IEnumerable<IMemberMetric> MemberMetrics { get; private set; }

        public int DepthOfInheritance { get; private set; }

        public int ClassCoupling { get; private set; }

        public int AfferentCoupling { get; private set; }

        public int EfferentCoupling { get; private set; }

        public double Instability { get; private set; }

        public bool IsAbstract { get; private set; }

        public string Namespace { get; private set; }
        
        public string FullName { get { return string.Format("{0}.{1}", Namespace, Name); } }

        public ClassRank Rank { get; private set; }

        public TypeMetricWithNamespace WithNamespace(string @namespace)
        {
            Namespace = @namespace;
            return this;
        }

        public TypeMetricWithNamespace WithRank(ClassRank rank)
        {
            Rank = rank;
            return this;
        }

        public override bool Equals(object obj)
        {
            var typeMetric = obj as TypeMetricWithNamespace;
            if (typeMetric == null)
                return false;

            return typeMetric.FullName == FullName;
        }
        
        public override int GetHashCode()
        {
            // TODO: write your implementation of GetHashCode() here
            return FullName.GetHashCode();
        }
    }
}
