# Matrix Screensaver

Windows Screensaver (.scr) that displays the [Matrix Digital Rain](https://github.com/Rezmason/matrix) web app as a screensaver via WebView2.

## Features

- Full Matrix Digital Rain animation (WebGL)
- Multi-monitor support
- Configurable settings:
  - 17 different versions (Classic, 3D, Megacity, Resurrections, ...)
  - Volumetric 3D effect
  - Animation speed
  - Resolution
  - Column count
  - Skip intro
- All standard screensaver modes (`/s`, `/c`, `/p`)

## Requirements

- Windows 10/11
- [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)
- [WebView2 Runtime](https://developer.microsoft.com/microsoft-edge/webview2/) (pre-installed on Windows 10 21H2+ / Windows 11)

## Installation

### Installer (Recommended)

1. Download **MatrixScreensaverSetup.exe** from [GitHub Releases](../../releases)
2. Run the installer — it checks for required runtimes and installs everything automatically
3. Optionally set Matrix as your active screensaver on the finish page

### Manual

1. Download the **Matrix-Screensaver-v1.0.zip** artifact from [GitHub Releases](../../releases)
2. Extract the ZIP
3. Right-click `Matrix.scr` → **Install**

## Build from Source

```bash
dotnet publish Matrix.csproj -c Release -r win-x64 --self-contained false -o publish
cp publish/Matrix.exe publish/Matrix.scr
cp -r web publish/web
```

For a self-contained build without .NET Runtime dependency:

```bash
dotnet publish Matrix.csproj -c Release -r win-x64 --self-contained true -o publish
```

## Credits

- **Matrix Digital Rain Web-App:** [Rezmason/matrix](https://github.com/Rezmason/matrix) (MIT License)
