using System;

namespace MetricsExtractor
{
    [Serializable]
    public class MetodoRuim
    {
        public string ClassName { get; set; }

        public string NomeMetodo { get; set; }

        public double Manutenibilidade { get; set; }
        
        public double Complexidade { get; set; }

        public double QuantidadeDeLinhas { get; set; }
    }
}
