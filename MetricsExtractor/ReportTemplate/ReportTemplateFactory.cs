using System;
using System.IO;
using RazorEngine.Templating;

namespace MetricsExtractor.ReportTemplate
{
    public class ReportTemplateFactory
    {
        public string GetReport(EstadoDoProjeto estadoDoProjeto)
        {
            var templateFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ReportTemplate");
            var templateFilePath = Path.Combine(templateFolderPath, "Index.cshtml");
            var service = new TemplateService(/*templateConfig*/);
            RazorEngine.Razor.SetTemplateService(service);
            var emailHtmlBody = service.Parse(File.ReadAllText(templateFilePath), estadoDoProjeto, null, null);

            return emailHtmlBody;
        }
    }
}
