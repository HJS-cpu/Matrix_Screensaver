# Matrix Screensaver

Windows Screensaver (.scr) der die [Matrix Digital Rain](https://github.com/Rezmason/matrix) Web-App per WebView2 als Bildschirmschoner darstellt.

## Features

- Vollständige Matrix Digital Rain Animation (WebGL)
- Multi-Monitor-Support
- Konfigurierbare Einstellungen:
  - 17 verschiedene Versionen (Classic, 3D, Megacity, Resurrections, ...)
  - Volumetrischer 3D-Effekt
  - Animationsgeschwindigkeit
  - Auflösung
  - Spaltenanzahl
  - Intro überspringen
- Alle Standard-Screensaver-Modi (`/s`, `/c`, `/p`)

## Voraussetzungen

- Windows 10/11
- [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)
- [WebView2 Runtime](https://developer.microsoft.com/microsoft-edge/webview2/) (auf Windows 10 21H2+ / Windows 11 vorinstalliert)

## Installation

1. Das **Matrix-Screensaver** Artifact von [GitHub Actions](../../actions) herunterladen
2. ZIP entpacken
3. `Matrix.scr` → Rechtsklick → **Installieren**

## Selber bauen

```bash
dotnet publish Matrix.csproj -c Release -r win-x64 --self-contained false -o publish
cp publish/Matrix.exe publish/Matrix.scr
cp -r web publish/web
```

Für einen eigenständigen Build ohne .NET Runtime Abhängigkeit:

```bash
dotnet publish Matrix.csproj -c Release -r win-x64 --self-contained true -o publish
```

## Credits

- **Matrix Digital Rain Web-App:** [Rezmason/matrix](https://github.com/Rezmason/matrix) (MIT License)
