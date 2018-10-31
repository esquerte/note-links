using Microsoft.Extensions.Logging;
using NoteLinks.Service.Logging.FileLogger;
using System;

namespace NoteLinks.Service.Extensions
{
    public static class FileLoggerExtensions
    {
        public static ILoggerFactory AddFileLogger(this ILoggerFactory loggerFactory, FileLoggerConfiguration config)
        {
            loggerFactory.AddProvider(new FileLoggerProvider(config));
            return loggerFactory;
        }
        public static ILoggerFactory AddFileLogger(this ILoggerFactory loggerFactory)
        {
            var config = new FileLoggerConfiguration();
            return loggerFactory.AddFileLogger(config);
        }
        public static ILoggerFactory AddFileLogger(this ILoggerFactory loggerFactory, Action<FileLoggerConfiguration> configure)
        {
            var config = new FileLoggerConfiguration();
            configure(config);
            return loggerFactory.AddFileLogger(config);
        }
    }
}
