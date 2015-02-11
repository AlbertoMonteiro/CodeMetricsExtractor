using ArchiMetrics.Common.Metrics;
using MetricsExtractor.Custom;

namespace MetricsExtractor
{
    internal class MetodoComTipo
    {
        public TypeMetricWithNamespace Tipo { get; set; }

        public IMemberMetric Metodo { get; set; }
    }
}