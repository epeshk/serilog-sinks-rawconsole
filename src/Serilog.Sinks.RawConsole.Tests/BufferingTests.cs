namespace Serilog.Sinks.RawConsole.Tests;

public class BufferingTests
{
  [Fact]
  public void Buffered()
  {
    var stream = new FlushCountingStream();
    var some = Some.InformationEvent();
        
    using (var sink = new RawStreamSink(stream, new CharByCharFormatter(), true, null))
    {
      sink.Emit(some);
      sink.Emit(some);
      sink.Emit(some);
      sink.Emit(some);
    }
    
    Assert.Equal(1, stream.FlushesCount);
  }

  [Fact]
  public void Unbuffered()
  {
    var stream = new FlushCountingStream();
    var some = Some.InformationEvent();
        
    using (var sink = new RawStreamSink(stream, new CharByCharFormatter(), false, null))
    {
      sink.Emit(some);
      sink.Emit(some);
      sink.Emit(some);
      sink.Emit(some);
    }
    
    Assert.Equal(4, stream.FlushesCount);
  }
}

public class FlushCountingStream : Stream
{
  public int FlushesCount;
  
  public override void Flush()
  {
    Interlocked.Increment(ref FlushesCount);
  }

  public override int Read(byte[] buffer, int offset, int count)
  {
    throw new NotSupportedException();
  }

  public override long Seek(long offset, SeekOrigin origin)
  {
    throw new NotSupportedException();
  }

  public override void SetLength(long value)
  {
    throw new NotSupportedException();
  }

  public override void Write(byte[] buffer, int offset, int count)
  {
    throw new NotSupportedException();
  }

  public override void Write(ReadOnlySpan<byte> buffer)
  {
  }

  public override Task FlushAsync(CancellationToken cancellationToken)
  {
    Interlocked.Increment(ref FlushesCount);
    return Task.CompletedTask;
  }

  public override bool CanRead { get; }
  public override bool CanSeek { get; }
  public override bool CanWrite => true;
  public override long Length { get; }
  public override long Position { get; set; }
}