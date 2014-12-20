using System;
using System.IO;
using RazorEngine.Templating;

namespace MetricsExtractor.ReportTemplate
{
    public class ReportTemplateFactory
    {
        public string GetReport(EstadoDoProjeto estadoDoProjeto)
        {
            // Generate the email body from the template file.
            var templateFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ReportTemplate");
            var templateFilePath = Path.Combine(templateFolderPath, "Index.cshtml");
/*
            var templateConfig = new TemplateServiceConfiguration
            {
                Resolver = new DelegateTemplateResolver(name =>
                {
                    //no caching cause RazorEngine handles that itself
                    var templatePath = Path.Combine(templateFolderPath, name);
                    using (var reader = new StreamReader(templatePath)) // let it throw if doesn't exist
                        return reader.ReadToEnd();
                })
            };
*/
            var service = new TemplateService(/*templateConfig*/);
            RazorEngine.Razor.SetTemplateService(service);
            var emailHtmlBody = service.Parse(File.ReadAllText(templateFilePath), estadoDoProjeto, null, null);

            return emailHtmlBody;
        }
    }
}
