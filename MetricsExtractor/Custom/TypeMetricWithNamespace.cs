using System;
using System.Collections.Generic;
using System.Linq;
using ArchiMetrics.Analysis.Common.Metrics;

namespace MetricsExtractor.Custom
{
    public class TypeMetricWithNamespace : ITypeMetric
    {
        static readonly Type MyType = typeof(TypeMetricWithNamespace);

        public TypeMetricWithNamespace(ITypeMetric typeMetric)
        {
            var type = typeMetric.GetType();
            var props = type.GetProperties()
                .Join(MyType.GetProperties(), p => p.Name, p => p.Name, Tuple.Create);

            foreach (var property in props)
            {
                var value = property.Item1.GetValue(typeMetric);
                property.Item2.SetValue(this, value);
            }
        }

        public IEnumerable<ITypeCoupling> ClassCouplings { get; private set; }

        public IEnumerable<ITypeCoupling> Dependencies { get; private set; }

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

        public string FullName => $"{Namespace}.{Name}";

        public ClassRank Rank { get; private set; }

        public ITypeDocumentation Documentation { get; private set; }

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