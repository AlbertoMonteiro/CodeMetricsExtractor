using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiMetrics.Analysis;
using ArchiMetrics.Common;
using ArchiMetrics.Common.Metrics;
using MetricsExtractor.ReportTemplate;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MetricsExtractor
{
    class Program
    {
        private static readonly List<ClassRank> ClassRanks = Enum.GetValues(typeof(ClassRank)).Cast<ClassRank>().ToList();
        private static readonly string ApplicationPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        static void Main(string[] args)
        {
            if (!args.Any())
            {
                Console.WriteLine("No solution configured");
                return;
            }

            var metricConfiguration = DashedParameterSerializer.Deserialize<MetricConfiguration>(args);

            var runCodeMetrics = RunCodeMetrics(metricConfiguration.Solution, metricConfiguration.IgnoredProjects);
            runCodeMetrics.Wait();
            var namespaceMetrics = runCodeMetrics.Result;

            var types = namespaceMetrics.SelectMany(x => x.TypeMetrics).ToList();

            const int MAX_LINES_OF_CODE_ON_METHOD = 30;

            var metodos = types.SelectMany(x => x.MemberMetrics, (type, member) => new MetodoComTipo { Tipo = type, Metodo = member }).ToList();

            var metodosRuins = GetMetodosRuins(metodos, MAX_LINES_OF_CODE_ON_METHOD);

            var resultadoGeral = CreateEstadoDoProjeto(types, metodosRuins, metodos.Count);

            var classesGroupedByRank = GetClassesGroupedByRank(types);

            resultadoGeral.TotalDeClassesPorRank = classesGroupedByRank;

            var reportPath = GenerateReport(resultadoGeral, metricConfiguration.SolutionDirectory);

#if DEBUG
            Process.Start(reportPath);
#endif
        }

        private static string GenerateReport(EstadoDoProjeto resultadoGeral, string solutionDirectory)
        {
            var reportDirectory = Path.Combine(solutionDirectory, "CodeMetricsReport");
            var reportPath = Path.Combine(reportDirectory, "CodeMetricsReport.zip");

            var reportTemplateFactory = new ReportTemplateFactory();
            var report = reportTemplateFactory.GetReport(resultadoGeral);
            var list = Directory.GetFiles(Path.Combine(ApplicationPath, "ReportTemplate"), "*.css").ToList();

            Directory.CreateDirectory(reportDirectory);
            using (var zipArchive = new ZipArchive(File.OpenWrite(reportPath), ZipArchiveMode.Create))
            {
                foreach (var item in list)
                {
                    var fileName = Path.GetFileName(item);
                    zipArchive.CreateEntryFromFile(item, fileName);
#if DEBUG
                    File.Copy(item, Path.Combine(reportDirectory, fileName), true);
#endif
                }
                var archiveEntry = zipArchive.CreateEntry("Index.html");
                using (var stream = archiveEntry.Open())
                using (var streamWriter = new StreamWriter(stream, Encoding.UTF8))
                    streamWriter.Write(report);
#if DEBUG
                reportPath = Path.Combine(reportDirectory, "Index.html");
                File.WriteAllText(reportPath, report, Encoding.UTF8);
#endif
                zipArchive.Dispose();
            }
            return reportPath;
        }

        private static Dictionary<ClassRank, int> GetClassesGroupedByRank(List<ITypeMetric> types)
        {
            var classesGroupedByRank = new Dictionary<ClassRank, int>();
            ClassRanks.ForEach(c => classesGroupedByRank.Add(c, 0));
            var classRankings = CreateClassesRank(types).ToList();
            foreach (var group in classRankings.GroupBy(c => c.Rank))
                classesGroupedByRank[@group.Key] = @group.Count();
            return classesGroupedByRank;
        }

        private static EstadoDoProjeto CreateEstadoDoProjeto(List<ITypeMetric> modules, List<MetodoRuim> metodosRuins, int totalDeMetodos)
        {
            var resultadoGeral = new EstadoDoProjeto
            {
                Manutenibilidade = (int)modules.Average(x => x.MaintainabilityIndex),
                CCAbsoluto = modules.Average(x => x.CyclomaticComplexity),
                LinhasDeCodigo = modules.Sum(x => x.LinesOfCode),
                AcoplamentoAbsoluto = modules.Average(x => x.ClassCoupling),
                ProfuDeHeranca = modules.Average(x => x.DepthOfInheritance),
                MetodosRuins = metodosRuins,
                TotalDeMetodos = totalDeMetodos
            };
            return resultadoGeral;
        }

        private static List<MetodoRuim> GetMetodosRuins(List<MetodoComTipo> metodos, int MAX_LINES_OF_CODE_ON_METHOD)
        {
            var metodosRuins = metodos
                //.Where(x => (x.Metodo.LinesOfCode >= MAX_LINES_OF_CODE_ON_METHOD) || (x.Metodo.CyclomaticComplexity >= 10))
                .Select(x => new MetodoRuim
                {
                    ClassName = x.Tipo.Name,
                    NomeMetodo = x.Metodo.Name,
                    Complexidade = x.Metodo.CyclomaticComplexity,
                    Manutenibilidade = x.Metodo.MaintainabilityIndex,
                    QuantidadeDeLinhas = x.Metodo.LinesOfCode,
                })
                .OrderByDescending(x => x.QuantidadeDeLinhas).ThenByDescending(x => x.Complexidade)
                .ToList();
            return metodosRuins;
        }

        private static async Task<IEnumerable<INamespaceMetric>> RunCodeMetrics(string solutionPath, string[] ignoredProjects)
        {
            Console.WriteLine("Loading Solution");
            var solutionProvider = new SolutionProvider();
            var solution = await solutionProvider.Get(solutionPath);
            Console.WriteLine("Solution loaded");

            var projects = solution.Projects.Where(p => !ignoredProjects.Contains(p.Name)).Take(6).ToList();
            var elementAt = projects
                .SelectMany(p => p.Documents)
                .Where(d => d.Name.Contains("EmpresaService"))
                .Select(x => x.GetSyntaxRootAsync());

            var nodes = await Task.WhenAll(elementAt);
            var syntaxNodes = nodes.SelectMany(syntaxNode => syntaxNode.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList());
            foreach (var syntaxOneNode in syntaxNodes)
            {
                var sourceText = syntaxOneNode.GetText();
                var s = sourceText.ToString();
                var textLineCollection = sourceText.Lines.Count(l => l.Span.Length > 0);
                Console.WriteLine(textLineCollection);
            }

            Console.WriteLine("Loading metrics, wait it may take a while.");
            var metricsCalculator = new CodeMetricsCalculator();
            var metrics = new List<INamespaceMetric>();
            var calculateTasks = projects.Select(async p =>
            {
                using (new TimerMeasure(string.Format("Loading metrics from project {0}", p.Name), string.Format("{0} metrics loaded", p.Name)))
                {
                    var namespaceMetrics = await metricsCalculator.Calculate(p, solution);
                    return namespaceMetrics;
                }
            });
            metrics = (await Task.WhenAll(calculateTasks)).SelectMany(nm => nm).ToList();
            return metrics;
        }

        private static IEnumerable<ClassRanking> CreateClassesRank(List<ITypeMetric> types)
        {
            return from type in types
                   let maintainabilityIndex = type.MaintainabilityIndex
                   select new ClassRanking(type.Name, ClassRanks.FirstOrDefault(r => (int)r >= maintainabilityIndex));
        }
    }
}