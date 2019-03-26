using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoteLinks.Service.Logging.Service
{
    class LoggingInfo
    {
        public LogLevel LogLevel { get; }
        public string Message { get; }

        public LoggingInfo() { }

        public LoggingInfo(LogLevel logLevel, string message)
        {
            LogLevel = logLevel;
            Message = message;
        }
    }
}
