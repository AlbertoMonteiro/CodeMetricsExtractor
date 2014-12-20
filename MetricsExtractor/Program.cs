using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using MetricsExtractor.CodeMetrics;
using MetricsExtractor.ReportTemplate;

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

            var projectsArguments = string.Join(" ", GetProjectsPath(metricConfiguration).Select(x => string.Format("/f:\"{0}\"", x)));

            var metricsOutput = Path.Combine(metricConfiguration.SolutionDirectory, "Metrics.xml");
            RunCodeMetrics(projectsArguments, metricsOutput);
            var metricsReport = CollectCodeMetricsReport(metricsOutput);

            var modules = metricsReport.Targets.Select(x => x.Modules.Module).ToList();
            var types = modules.SelectMany(x => x.Namespaces).SelectMany(x => x.Types, (ns, type) => type.WithNamespace(ns.Name)).ToList();

            const int MAX_LINES_OF_CODE_ON_METHOD = 30;

            var metodos = types.SelectMany(x => x.Members, (type, member) => new MetodoComTipo { Tipo = type, Metodo = member }).ToList();

            var metodosRuins = GetMetodosRuins(metodos, MAX_LINES_OF_CODE_ON_METHOD);

            var resultadoGeral = CreateEstadoDoProjeto(modules, metodosRuins, metodos.Count);

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

        private static Dictionary<ClassRank, int> GetClassesGroupedByRank(List<NamespaceType> types)
        {
            var classesGroupedByRank = new Dictionary<ClassRank, int>();
            ClassRanks.ForEach(c => classesGroupedByRank.Add(c, 0));
            var classRankings = CreateClassesRank(types).ToList();
            foreach (var group in classRankings.GroupBy(c => c.Rank))
                classesGroupedByRank[@group.Key] = @group.Count();
            return classesGroupedByRank;
        }

        private static EstadoDoProjeto CreateEstadoDoProjeto(List<Module> modules, List<MetodoRuim> metodosRuins, int totalDeMetodos)
        {
            var dictionary = modules.SelectMany(m => m.Metrics).GroupBy(m => m.Kind).ToDictionary(x => x.Key, ExtractValue);
            var resultadoGeral = new EstadoDoProjeto
            {
                Manutenibilidade = (int)dictionary[MetricKind.MaintainabilityIndex],
                CCAbsoluto = dictionary[MetricKind.CyclomaticComplexity],
                LinhasDeCodigo = dictionary[MetricKind.LinesOfCode],
                AcoplamentoAbsoluto = dictionary[MetricKind.ClassCoupling],
                ProfuDeHeranca = dictionary[MetricKind.DepthOfInheritance],
                MetodosRuins = metodosRuins,
                TotalDeMetodos = totalDeMetodos
            };
            return resultadoGeral;
        }

        private static List<MetodoRuim> GetMetodosRuins(List<MetodoComTipo> metodos, int MAX_LINES_OF_CODE_ON_METHOD)
        {
            var metodosRuins = metodos
                .Where(x => (x.Metodo.MetricsDic[MetricKind.LinesOfCode] >= MAX_LINES_OF_CODE_ON_METHOD) || (x.Metodo.MetricsDic[MetricKind.CyclomaticComplexity] >= 10))
                .Select(x => new MetodoRuim
                {
                    ClassName = x.Tipo.FullName,
                    NomeMetodo = x.Metodo.Name,
                    Complexidade = x.Metodo.MetricsDic[MetricKind.CyclomaticComplexity],
                    Manutenibilidade = x.Metodo.MetricsDic[MetricKind.MaintainabilityIndex],
                    QuantidadeDeLinhas = x.Metodo.MetricsDic[MetricKind.LinesOfCode],
                })
                .OrderByDescending(x => x.QuantidadeDeLinhas).ThenByDescending(x => x.Complexidade)
                .ToList();
            return metodosRuins;
        }

        private static CodeMetricsReport CollectCodeMetricsReport(string metricsOutput)
        {
            var xmlSerializer = new XmlSerializer(typeof(CodeMetricsReport));
            CodeMetricsReport metricsReport;
            using (var fileStream = File.OpenRead(metricsOutput))
                metricsReport = (CodeMetricsReport)xmlSerializer.Deserialize(fileStream);
            return metricsReport;
        }

        private static void RunCodeMetrics(string projectsArguments, string metricsOutput)
        {
            var metricsExePath = Path.Combine(ApplicationPath, "metrics/metrics.exe");
            var arguments = string.Format("{0} /igc /iit /o:\"{1}\"", projectsArguments, metricsOutput);
            var process = Process.Start(new ProcessStartInfo(metricsExePath, arguments) { UseShellExecute = false });
            process.WaitForExit();
        }

        private static List<string> GetProjectsPath(MetricConfiguration metricConfiguration)
        {
            var solutionLines = File.ReadLines(metricConfiguration.Solution).Where(l => l.StartsWith("Project")).ToList();

            const string STR_REGEX = @"Project\(""\{[\w-]+\}""\) = ""(?<name>[\w.]+)"", ""(?<path>.+\.csproj?)"", ""\{[\w-]+\}";
            var ignorageWords = metricConfiguration.IgnoredProjects;
            var projectsName = (from solutionLine in solutionLines
                                let match = Regex.Match(solutionLine, STR_REGEX)
                                let projectPath = match.Groups["path"].Value
                                where match.Success && !ignorageWords.Any(x => match.Groups["name"].Value.EndsWith(x))
                                let projectDirectory = Path.GetDirectoryName(Path.Combine(metricConfiguration.SolutionDirectory, projectPath))
                                select Directory.EnumerateFiles(projectDirectory, match.Groups["name"].Value + ".dll", SearchOption.AllDirectories).FirstOrDefault()
                                    into path
                                    where path != null
                                    select path).ToList();

            return projectsName;
        }

        private static double ExtractValue(IGrouping<MetricKind, Metric> grouping)
        {
            switch (grouping.Key)
            {
                case MetricKind.MaintainabilityIndex:
                case MetricKind.CyclomaticComplexity:
                case MetricKind.ClassCoupling:
                case MetricKind.DepthOfInheritance:
                    return grouping.Average(x => x.Value);
                case MetricKind.LinesOfCode:
                    return grouping.Sum(x => x.Value);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static IEnumerable<ClassRanking> CreateClassesRank(IEnumerable<NamespaceType> types)
        {
            return from type in types
                   let maintainabilityIndex = type.Metrics.FirstOrDefault(x => x.Kind == MetricKind.MaintainabilityIndex).Value
                   select new ClassRanking(type.Name, ClassRanks.FirstOrDefault(r => (int)r >= maintainabilityIndex));
        }
    }
}
