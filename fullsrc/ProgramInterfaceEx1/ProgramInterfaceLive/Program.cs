
using System;
using System.IO;


namespace Exercises
{
    public interface ILogger
    {
        void Log(string message);
    }


    public sealed class ConsoleLogger : ILogger
    {
        public void Log(string message) => Console.WriteLine("Console" + message);

    }
    public sealed class FileLogger : ILogger
    {
        private string _path;
        public FileLogger(string path = "log.txt") => _path = path;

        public void Log(string message) => File.AppendAllText(_path, "[File]" + message + Environment.NewLine);

    }

    // Consumer

    public sealed class ReportService
    {
        private ILogger _logger;

        public ReportService(ILogger logger) => _logger = logger;

        public void Generate()
        {
            _logger.Log("Generating report...");
            // do something
            _logger.Log("Report ready.");
        }
    }

    public static class ProgramInterfacesEx1
    {
        public static void Main(string[] args) {
            var mode = (args.Length > 0 ? args[0] : "console").ToLowerInvariant();

            ILogger logger = mode == "file" ? new FileLogger() : new ConsoleLogger();

            var servc = new ReportService(logger);
            servc.Generate();
            if (mode == "file") { Console.WriteLine("Wrote logs to ..."); }
        }


    }

}

