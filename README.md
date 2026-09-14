# SIC — Simple Image Converter

A lightweight, cross-platform desktop app for converting images between formats, built with Avalonia UI and .NET.

## Features

- **Batch & single-file conversion** — convert one image or an entire folder at once
- **Drag & drop** — drop a file or folder straight onto the app
- **Multiple output formats** — convert between common image formats (PNG, JPEG, WEBP, BMP, GIF, TIFF and more, powered by ImageMagick)
- **Quality / compression control** — adjustable quality slider for lossy formats
- **Resize on export** — scale output resolution by percentage with a live preview of target dimensions
- **Background color picker** — set a background fill for formats that don't support transparency
- **Metadata preservation** — optional toggle to keep or strip image metadata
- **Live logs panel** — filterable log viewer showing conversion progress and errors
- **Dark theme UI** — custom dark theme, currently being migrated to Material Design

## Tech Stack

- **UI Framework:** [Avalonia UI](https://avaloniaui.net/) (cross-platform, Windows + Linux)
- **Architecture:** MVVM with [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet)
- **Image Processing:** [Magick.NET](https://github.com/dlemstra/Magick.NET) (ImageMagick wrapper)
- **UI Theme:** [Material.Avalonia](https://github.com/AvaloniaCommunity/Material.Avalonia)
- **Language / Runtime:** C# / .NET

## Download

Grab the latest build from the [Releases](../../releases) page.

- `SIC-win-x64.exe` — Windows (self-contained, no .NET install required)
## Usage

1. Select a source file or folder (via the buttons or drag & drop)
2. Choose an output folder
3. Pick your target format and adjust quality / resolution / background color as needed
4. Click **Convert**

## Building from Source

```bash
git clone <repo-url>
cd SIC
dotnet restore
dotnet build
```

### Publish a self-contained build

```bash
# Windows
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o ./publish/win-x64

# Linux
dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o ./publish/linux-x64
```

## License

MIT
