using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;

namespace Serilog.Sinks;

class RoutingSink : ILogEventSink, IDisposable
{
    readonly RawStreamSink stdout;
    readonly RawStreamSink stderr;
    readonly LogEventLevel errorFromLevel;

    public RoutingSink(RawStreamSink stdout, RawStreamSink stderr, LogEventLevel errorFromLevel)
    {
        this.stdout = stdout;
        this.stderr = stderr;
        this.errorFromLevel = errorFromLevel;
    }

    public void Emit(LogEvent logEvent)
    {
        var sink = logEvent.Level >= errorFromLevel ? stderr : stdout;
        sink.Emit(logEvent);
    }

    public void Dispose()
    {
        DisposeCatching(stdout);
        DisposeCatching(stderr);
    }

    void DisposeCatching(IDisposable disposable)
    {
        try
        {
            disposable.Dispose();
        }
        catch (Exception e)
        {
            SelfLog.WriteLine(e.ToString());
        }
    }
}