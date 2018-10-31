using Microsoft.Extensions.Logging;
using System.IO;

namespace NoteLinks.Service.Logging.FileLogger
{
    public class FileLoggerConfiguration
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Warning;
        public int EventId { get; set; } = 0;
        public string FilePath { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "log.json");

    }
}
