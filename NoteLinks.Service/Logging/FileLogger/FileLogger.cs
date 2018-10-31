using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace NoteLinks.Service.Logging.FileLogger
{
    public class FileLogger : ILogger
    {
        private readonly string _name;
        private readonly FileLoggerConfiguration _config;
        private object _lock = new object();

        public FileLogger(string name, FileLoggerConfiguration config)
        {
            _name = name;
            _config = config;
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= _config.LogLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter != null)
            {
                lock (_lock)
                {

                    var jsonLine = JsonConvert.SerializeObject(new
                    {
                        dateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff"),
                        logLevel = logLevel.ToString(),
                        eventId = eventId.Id,
                        name = _name,
                        message = formatter(state, exception),
                        exception
                    });

                    File.AppendAllText(_config.FilePath, jsonLine + Environment.NewLine);
                }
            }
        }
    }
}
