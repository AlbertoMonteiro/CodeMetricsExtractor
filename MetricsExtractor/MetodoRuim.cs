using System;

namespace MetricsExtractor
{
    [Serializable]
    public class MetodoRuim
    {
        public MetodoRuim()
        {
            
        }

        public MetodoRuim(string className, string ns, string nomeMetodo, double manutenibilidade, double complexidade, double quantidadeDeLinhas)
        {
            ClassName = className;
            Namespace = ns;
            NomeMetodo = nomeMetodo;
            Manutenibilidade = manutenibilidade;
            Complexidade = complexidade;
            QuantidadeDeLinhas = quantidadeDeLinhas;
        }

        public string FullClassName => $"{Namespace}.{ClassName}";

        public string ClassName { get; }

        public string Namespace { get; }

        public string NomeMetodo { get; }

        public double Manutenibilidade { get; }

        public double Complexidade { get; }

        public double QuantidadeDeLinhas { get; }
    }
}
