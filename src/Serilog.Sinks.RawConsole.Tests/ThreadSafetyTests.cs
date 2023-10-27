using System.Text;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Sinks.RawConsole.Tests;

public class ThreadSafetyTests
{
  [Fact]
  public void FileSink_ShouldBeThreadSafe()
  {
    var evt = Some.LogEvent("Hello, world!");

    const int N = 256 * 1024;

    var stream = new MemoryStream();
        
    using (var sink = new RawStreamSink(stream, new CharByCharFormatter(), true, null))
    {
      var countdownEvent = new CountdownEvent(2);
      var t1 = new Thread(() => WriteToSink(sink, evt, N, countdownEvent));
      var t2 = new Thread(() => WriteToSink(sink, evt, N, countdownEvent));
      t1.Start();
      t2.Start();
      t1.Join();
      t2.Join();
    }

    var lines = Encoding.UTF8.GetString(stream.ToArray()).Split('\n', StringSplitOptions.RemoveEmptyEntries);

    Assert.Equal(2 * N, lines.Length);
    Assert.Single(lines.Distinct());
  }

  static void WriteToSink(ILogEventSink sink, LogEvent logEvent, int count, CountdownEvent countdownEvent)
  {
    countdownEvent.Signal();
    countdownEvent.Wait();

    while (count --> 0)
      sink.Emit(logEvent);
  }
}