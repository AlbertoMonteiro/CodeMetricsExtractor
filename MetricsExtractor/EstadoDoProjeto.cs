using System;
using System.Collections.Generic;
using System.Linq;
using MetricsExtractor.Custom;

namespace MetricsExtractor
{
    [Serializable]
    public class EstadoDoProjeto
    {
        private Dictionary<ClassRank, int> totalDeClassesPorRank;
        public int Manutenibilidade { get; set; }
        public int TotalDeMetodos { get; set; }
        public IList<MetodoRuim> MetodosRuins { get; set; }

        public double PercentualDeMetodosRuins
        {
            get { return MetodosRuins.Count/(double) TotalDeMetodos; }
        }

        public double PercentualDeMetodosRuinsComplexidadeCiclomatica
        {
            get { return MetodosComAltaComplexidadeClicomatica/(double) TotalDeMetodos; }
        }

        public int MetodosComAltaComplexidadeClicomatica
        {
            get { return MetodosRuins.Count(m => m.Complexidade > 10); }
        }

        public double PercentualDeMetodosGrandes
        {
            get { return MetodosGrandes/(double) TotalDeMetodos; }
        }

        public int MetodosGrandes
        {
            get { return MetodosRuins.Count(m => m.QuantidadeDeLinhas > 30); }
        }

        public double LinhasDeCodigo { get; set; }

        public double ProfuDeHeranca { get; set; }

        public Dictionary<ClassRank, int> TotalDeClassesPorRank
        {
            get
            {
                return totalDeClassesPorRank ??
                       (totalDeClassesPorRank = TypesWithMetrics.ToDictionary(x => x.Key, x => x.Value.Count));
            }
        }

        public Dictionary<ClassRank, List<TypeMetricWithNamespace>> TypesWithMetrics { get; set; }

        public override string ToString()
        {
            return string.Join(" - ",
                GetType().GetProperties().Select(p => string.Format("{0}: {1}", p.Name, p.GetValue(this))));
        }
    }
}