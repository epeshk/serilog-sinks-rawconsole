using System.Buffers;
using System.Text;
using Serilog.Events;
using Serilog.Formatting;

namespace Serilog.Sinks.RawConsole.Tests;

class CharByCharFormatter : IBufferWriterFormatter
{
    public void Format(LogEvent logEvent, IBufferWriter<byte> buffer)
    {
        foreach (var c in logEvent.MessageTemplate.Text)
        {
            var span = buffer.GetSpan(1);
            span[0] = (byte)c;
            buffer.Advance(1);
        }

        var span1 = buffer.GetSpan(2);
        span1[0] = (byte)'\r';
        span1[1] = (byte)'\n';
        buffer.Advance(2);
    }

    public Encoding Encoding { get; } = Encoding.UTF8;
}
