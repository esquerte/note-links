using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NoteLinks.Service.Logging.Service
{
    public class LoggingService : BackgroundService
    {
        private ConcurrentQueue<LoggingInfo> _loggingQueue;
        private ILogger<LoggingService> _logger;

        public LoggingService(ILogger<LoggingService> logger)
        {
            _loggingQueue = new ConcurrentQueue<LoggingInfo>();
            _logger = logger;
        }

        public void Log(LogLevel logLevel, string message)
        {
            _loggingQueue.Enqueue(new LoggingInfo(logLevel, message));
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            LoggingInfo loggingInfo;

            while (!stoppingToken.IsCancellationRequested)
            {           
                if (_loggingQueue.TryDequeue(out loggingInfo))
                {
                    // Just for test. Delete in production.
                    Thread.Sleep(5000);

                    _logger.Log(loggingInfo.LogLevel, loggingInfo.Message);                    
                }
            }

            return Task.CompletedTask;
        }
    }
}
