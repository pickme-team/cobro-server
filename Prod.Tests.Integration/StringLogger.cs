using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Prod.Tests.Intergration;

public class StringLogger : ILogger
{
    private readonly ConcurrentBag<string> _logs = new ConcurrentBag<string>();

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        _logs.Add($"[{logLevel}]: {formatter(state, exception)}");
    }

    public string Data => string.Join(Environment.NewLine, _logs);

    public bool IsEnabled(LogLevel logLevel) => true;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
}