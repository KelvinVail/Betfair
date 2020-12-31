using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Betfair.Core.Tests.TestDoubles
{
    public class LoggerSpy : ILogger
    {
        public string LastMessage { get; private set; }

        public string LastLevel { get; private set; }

        public int LogCount { get; private set; }

        public Dictionary<int, string> MessageList { get; private set; } = new Dictionary<int, string>();

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            LastMessage = state.ToString();
            LastLevel = logLevel.ToString();
            LogCount += 1;
            MessageList.Add(LogCount, LastMessage);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        public string GetMessageNumber(int value)
        {
            return MessageList.ContainsKey(value) ? MessageList[value] : null;
        }

        public int TimesLogged(string message)
        {
            return MessageList.Count(m => m.Value == message);
        }
    }
}
