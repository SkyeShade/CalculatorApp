# CalculatorApp

A simple Windows Forms calculator built with C# and .NET 8.

## Features

- Addition, subtraction, multiplication and division
- Decimal number support
- Button and keyboard input
- Simple Windows desktop interface

## Requirements

- Windows
- .NET 8 SDK, if building from source

## Build

```bash
dotnet build -c Release
```

To create a standalone Windows executable:

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

The published executable can be found in:

```text
bin/Release/net8.0-windows/win-x64/publish/
```

## License

See the repository license for details.
