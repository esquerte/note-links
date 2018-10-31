using Microsoft.Extensions.Logging;

namespace NoteLinks.Service.Logging.FileLogger
{
    public class FileLoggerProvider : ILoggerProvider
    {
        private FileLoggerConfiguration _config;
        public FileLoggerProvider(FileLoggerConfiguration config)
        {
            _config = config;
        }
        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(categoryName, _config);
        }

        public void Dispose()
        {
        }
    }
}
