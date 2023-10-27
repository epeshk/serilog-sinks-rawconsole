using System.Globalization;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks;

namespace Serilog
{
    /// <summary>
    /// Adds the WriteTo.RawConsole(), WriteTo.RawStream extension method to <see cref="LoggerConfiguration"/>.
    /// </summary>
    public static class RawConsoleLoggerConfigurationExtensions
    {
#if UTF8_RENDERING
        const string DefaultConsoleOutputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";

        /// <summary>
        /// Writes log events to <see cref="System.Console"/>.
        /// </summary>
        /// <param name="sinkConfiguration">Logger sink configuration.</param>
        /// <param name="restrictedToMinimumLevel">The minimum level for
        /// events passed through the sink. Ignored when <paramref name="levelSwitch"/> is specified.</param>
        /// <param name="outputTemplate">A message template describing the format used to write to the sink.
        /// The default is <code>"[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"</code>.</param>
        /// <param name="syncRoot">An object that will be used to `lock` (sync) access to the console output. If you specify this, you
        /// will have the ability to lock on this object, and guarantee that the console sink will not be about to output anything while
        /// the lock is held.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="levelSwitch">A switch allowing the pass-through minimum level
        /// to be changed at runtime.</param>
        /// <param name="standardErrorFromLevel">Specifies the level at which events will be written to standard error.</param>
        /// <param name="standardOutputBuffered">If <c>false</c>, log events will be flushed to console standard output immediately.</param>
        /// <param name="standardErrorBuffered">If <c>false</c>, log events will be flushed to console standard error immediately.</param>
        /// <returns>Configuration object allowing method chaining.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="sinkConfiguration"/> is <code>null</code></exception>
        /// <exception cref="ArgumentNullException">When <paramref name="outputTemplate"/> is <code>null</code></exception>
        public static LoggerConfiguration RawConsole(
            this LoggerSinkConfiguration sinkConfiguration,
            string outputTemplate=DefaultConsoleOutputTemplate,
            IFormatProvider? formatProvider = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            LoggingLevelSwitch? levelSwitch = null,
            LogEventLevel? standardErrorFromLevel = null,
            bool standardOutputBuffered = true,
            bool standardErrorBuffered = true,
            object? syncRoot = null)
        {
            return RawConsole(
                sinkConfiguration,
                new LegacyUtf8TextFormatter(outputTemplate, formatProvider ?? CultureInfo.InvariantCulture),
                restrictedToMinimumLevel,levelSwitch, standardErrorFromLevel, standardOutputBuffered, standardErrorBuffered, syncRoot);
        }
#endif

        /// <summary>
        /// Writes log events to <see cref="System.Console"/>.
        /// </summary>
        /// <param name="sinkConfiguration">Logger sink configuration.</param>
        /// <param name="restrictedToMinimumLevel">The minimum level for
        /// events passed through the sink. Ignored when <paramref name="levelSwitch"/> is specified.</param>
        /// <param name="formatter">Controls the rendering of log events into text, for example to log JSON.</param>
        /// <param name="syncRoot">An object that will be used to `lock` (sync) access to the console output. If you specify this, you
        /// will have the ability to lock on this object, and guarantee that the console sink will not be about to output anything while
        /// the lock is held.</param>
        /// <param name="levelSwitch">A switch allowing the pass-through minimum level
        /// to be changed at runtime.</param>
        /// <param name="standardErrorFromLevel">Specifies the level at which events will be written to standard error.</param>
        /// <param name="standardOutputBuffered">If <c>false</c>, log events will be flushed to console standard output immediately.</param>
        /// <param name="standardErrorBuffered">If <c>false</c>, log events will be flushed to console standard error immediately.</param>
        /// <returns>Configuration object allowing method chaining.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="sinkConfiguration"/> is <code>null</code></exception>
        /// <exception cref="ArgumentNullException">When <paramref name="formatter"/> is <code>null</code></exception>
        public static LoggerConfiguration RawConsole(
            this LoggerSinkConfiguration sinkConfiguration,
            IBufferWriterFormatter formatter,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            LoggingLevelSwitch? levelSwitch = null,
            LogEventLevel? standardErrorFromLevel = null,
            bool standardOutputBuffered = true,
            bool standardErrorBuffered = true,
            object? syncRoot = null)
        {
            if (sinkConfiguration is null) throw new ArgumentNullException(nameof(sinkConfiguration));
            if (formatter is null) throw new ArgumentNullException(nameof(formatter));

            ILogEventSink sink = standardErrorFromLevel.HasValue
                ? new RoutingSink(
                    new RawStreamSink(Console.OpenStandardOutput(), formatter, standardOutputBuffered, syncRoot),
                    new RawStreamSink(Console.OpenStandardError(), formatter, standardErrorBuffered, syncRoot),
                    standardErrorFromLevel.Value
                )
                : new RawStreamSink(Console.OpenStandardOutput(), formatter, standardOutputBuffered, syncRoot);

            return sinkConfiguration.Sink(sink, restrictedToMinimumLevel, levelSwitch);
        }

#if UTF8_RENDERING
        /// <summary>
        /// Writes log events to <see cref="System.IO.Stream"/>.
        /// </summary>
        /// <param name="sinkConfiguration">Logger sink configuration.</param>
        /// <param name="outputStream">Target stream.</param>
        /// <param name="restrictedToMinimumLevel">The minimum level for
        /// events passed through the sink. Ignored when <paramref name="levelSwitch"/> is specified.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="outputTemplate">A message template describing the format used to write to the sink.
        /// The default is <code>"[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"</code>.</param>
        /// <param name="syncRoot">An object that will be used to `lock` (sync) access to the stream. If you specify this, you
        /// will have the ability to lock on this object, and guarantee that the stream sink will not be about to output anything while
        /// the lock is held.</param>
        /// <param name="levelSwitch">A switch allowing the pass-through minimum level
        /// to be changed at runtime.</param>
        /// <returns>Configuration object allowing method chaining.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="sinkConfiguration"/> is <code>null</code></exception>
        /// <exception cref="ArgumentNullException">When <paramref name="outputStream"/> is <code>null</code></exception>
        public static LoggerConfiguration RawStream(
            this LoggerSinkConfiguration sinkConfiguration,
            Stream outputStream,
            string outputTemplate = DefaultConsoleOutputTemplate,
            IFormatProvider? formatProvider = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            LoggingLevelSwitch? levelSwitch = null,
            bool buffered = true,
            object? syncRoot = null)
        {
            return RawStream(
                sinkConfiguration, outputStream,
                new LegacyUtf8TextFormatter(outputTemplate, formatProvider),
                restrictedToMinimumLevel, levelSwitch, buffered, syncRoot);
        }
#endif

        /// <summary>
        /// Writes log events to <see cref="System.IO.Stream"/>.
        /// </summary>
        /// <param name="sinkConfiguration">Logger sink configuration.</param>
        /// <param name="outputStream">Target stream.</param>
        /// <param name="restrictedToMinimumLevel">The minimum level for
        /// events passed through the sink. Ignored when <paramref name="levelSwitch"/> is specified.</param>
        /// <param name="formatter">Controls the rendering of log events into text, for example to log JSON.</param>
        /// The default is <code>"[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"</code>.</param>
        /// <param name="syncRoot">An object that will be used to `lock` (sync) access to the stream. If you specify this, you
        /// will have the ability to lock on this object, and guarantee that the stream sink will not be about to output anything while
        /// the lock is held.</param>
        /// <param name="levelSwitch">A switch allowing the pass-through minimum level
        /// to be changed at runtime.</param>
        /// <param name="buffered">If <c>false</c>, log events will be flushed to the output immediately.</param>
        /// <returns>Configuration object allowing method chaining.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="sinkConfiguration"/> is <code>null</code></exception>
        /// <exception cref="ArgumentNullException">When <paramref name="outputStream"/> is <code>null</code></exception>
        /// <exception cref="ArgumentNullException">When <paramref name="formatter"/> is <code>null</code></exception>
        public static LoggerConfiguration RawStream(
            this LoggerSinkConfiguration sinkConfiguration,
            Stream outputStream,
            IBufferWriterFormatter formatter,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            LoggingLevelSwitch? levelSwitch = null,
            bool buffered = true,
            object? syncRoot = null)
        {
            if (sinkConfiguration is null) throw new ArgumentNullException(nameof(sinkConfiguration));
            if (outputStream is null) throw new ArgumentNullException(nameof(outputStream));
            if (formatter is null) throw new ArgumentNullException(nameof(formatter));

            var sink = new RawStreamSink(outputStream, formatter, buffered, syncRoot);

            return sinkConfiguration.Sink(sink, restrictedToMinimumLevel, levelSwitch);
        }
    }
}
