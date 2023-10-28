using System.Buffers;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;

namespace Serilog.Sinks;

class RawStreamSink : ILogEventSink, IDisposable
{
  [ThreadStatic] static ArrayBufferWriter<byte>? perThreadBuffer; 

  readonly object sync = new();
  readonly Stream output;
  readonly IBufferWriterFormatter formatter;
  readonly bool buffered;
  readonly ArrayBufferWriter<byte> sharedBuffer = new(DefaultWriteBufferCapacity);

  const int DefaultWriteBufferCapacity = 8192;
  const int DefaultPerThreadBufferCapacity = 512;
  const int FlushWriteBufferCapacity = 4096;

  public RawStreamSink(
    Stream output,
    IBufferWriterFormatter formatter,
    bool buffered,
    object? sync)
  {
    this.output = buffered ? output : new FlushingWrapper(output);
    this.formatter = formatter;
    this.buffered = buffered;
    this.sync = sync ?? new();
  }

  public void Emit(LogEvent logEvent)
  {
    if (!Monitor.TryEnter(sync))
    {
      RenderAndThenEmitUnderLock(logEvent);
      return;
    }

    try
    {
      formatter.Format(logEvent, sharedBuffer);
      if (!buffered || sharedBuffer.WrittenCount >= FlushWriteBufferCapacity)
      {
        output.Write(sharedBuffer.WrittenSpan);
        sharedBuffer.ResetWrittenCount();
      }
    }
    finally
    {
      Monitor.Exit(sync);
    }
  }

  void RenderAndThenEmitUnderLock(LogEvent logEvent)
  {
    var curThreadBuffer = perThreadBuffer ??= new ArrayBufferWriter<byte>(DefaultPerThreadBufferCapacity);
    curThreadBuffer.ResetWrittenCount();

    formatter.Format(logEvent, curThreadBuffer);

    lock (sync)
    {
      sharedBuffer.Write(curThreadBuffer.WrittenSpan);
      if (!buffered || sharedBuffer.WrittenCount >= FlushWriteBufferCapacity)
      {
        output.Write(sharedBuffer.WrittenSpan);
        sharedBuffer.ResetWrittenCount();
      }
    }

    curThreadBuffer.ResetWrittenCount();
  }

  public void Dispose()
  {
    lock (sync)
    {
      if (sharedBuffer.WrittenCount == 0)
        return;
      output.Write(sharedBuffer.WrittenSpan);
      sharedBuffer.ResetWrittenCount();
    }

    output.Flush();
    output.Dispose();
  }
}