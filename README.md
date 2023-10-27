# Serilog.Sinks.RawConsole

Writes [Serilog](https://serilog.net) events to console or stream.

Writes by default in UTF-8 encoding, bypassing conversion from UTF-16 and avoiding heap allocations where possible.

### Getting started

Install the [Serilog.Sinks.RawConsole](https://www.nuget.org/packages/Serilog.Sinks.RawConsole/) package from NuGet:

```powershell
Install-Package Serilog.Sinks.RawConsole
```

To configure the sink in C# code, call `WriteTo.RawConsole()` or `WriteTo.RawStream` during logger configuration:

```csharp
var log = new LoggerConfiguration()
    .WriteTo.RawConsole()
    .CreateLogger();
```


### Performance

Output is buffered by default.

It is recommended to use [Serilog.Sinks.Background](https://github.com/epeshk/serilog-sinks-background) package to wrap the file sink and perform all disk access on a background worker thread.


### Building from sources

`Serilog.Sinks.RawConsole` uses [source dependency](https://github.com/epeshk/serilog-utf8-commons) for format strings support without providing an external IBufferWriterFormatter implementation. To build this library either disable `UTF8_FORMATTER` constant, or place [this](https://github.com/epeshk/serilog-utf8-commons) repository near.

_Copyright &copy; 2023 Serilog Contributors - Provided under the [Apache License, Version 2.0](http://apache.org/licenses/LICENSE-2.0.html)._
