namespace Serilog.Sinks;

public class FlushingWrapper : Stream
{
  readonly Stream stream;

  public FlushingWrapper(Stream stream)
  {
    this.stream = stream;
    GC.SuppressFinalize(this);
  }

  public override void Flush()
  {
    stream.Flush();
  }

  protected override void Dispose(bool disposing)
  {
    stream.Dispose();
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
    stream.Write(buffer);
    stream.Flush();
  }

  public override bool CanRead { get; }
  public override bool CanSeek { get; }
  public override bool CanWrite => true;
  public override long Length { get; }
  public override long Position { get; set; }
}