using System;
using System.Diagnostics;

namespace MetricsExtractor
{
    public class TimerMeasure : IDisposable
    {
        private readonly string endMessage;
        private readonly Stopwatch stopwatch;

        public TimerMeasure(string initMessage = null, string endMessage = null)
        {
            this.endMessage = endMessage;
            stopwatch = Stopwatch.StartNew();
            if (!string.IsNullOrWhiteSpace(initMessage))
                Console.WriteLine(initMessage);
        }

        public void Dispose()
        {
            stopwatch.Stop();
            if (string.IsNullOrWhiteSpace(endMessage))
                Console.WriteLine("Tempo decorrido em: {0}", stopwatch.Elapsed);
            else
                Console.WriteLine("{0} decorrido em: {1}", endMessage, stopwatch.Elapsed);
        }
    }
}
