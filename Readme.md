# GuildedChatExporter

[![Build](https://github.com/yourusername/GuildedChatExporter/workflows/CI/badge.svg)](https://github.com/yourusername/GuildedChatExporter/actions)
[![Version](https://img.shields.io/github/v/release/yourusername/GuildedChatExporter?include_prereleases&sort=semver)](https://github.com/yourusername/GuildedChatExporter/releases)
[![License](https://img.shields.io/github/license/yourusername/GuildedChatExporter)](https://github.com/yourusername/GuildedChatExporter/blob/master/License.txt)

GuildedChatExporter is a tool that allows you to export message history from Guilded channels to a file.

## Features

- Export message history from direct message channels
- Export message history from specific channels
- Multiple export formats: HTML (dark and light), plain text, JSON, CSV
- Download media content (user avatars, attached files, embedded images, etc.)
- Markdown rendering
- Localization support
- Cross-platform (Windows, Linux, macOS)

## Download

- [Latest release](https://github.com/yourusername/GuildedChatExporter/releases/latest)

## Usage

### GUI

Coming soon!

### CLI

```
GuildedChatExporter.Cli guide
```

This will show you information on how to use the program.

#### Basic usage

1. List all direct message channels:

```
GuildedChatExporter.Cli getdms --cookies "your_cookie_string"
```

2. Export a specific channel:

```
GuildedChatExporter.Cli exportchannel --channel "channel_id" --cookies "your_cookie_string"
```

3. Export all direct message channels:

```
GuildedChatExporter.Cli exportdms --cookies "your_cookie_string"
```

#### Authentication

To use this program, you need to provide your Guilded authentication cookies. You can get these by:

1. Opening Guilded in your browser
2. Logging in to your account
3. Opening browser developer tools (F12)
4. Going to the 'Network' tab
5. Refreshing the page
6. Clicking on any request to guilded.gg
7. Looking for the 'Cookie' header in the request headers
8. Copying the entire cookie string

You can then provide this cookie string using the `--cookies` option or the `GUILDED_COOKIES` environment variable.

## Building from source

Requirements:

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)

```bash
# Clone the repository
git clone https://github.com/yourusername/GuildedChatExporter.git
cd GuildedChatExporter

# Build the project
dotnet build

# Run the CLI
dotnet run --project GuildedChatExporter.Cli
```

## License

GuildedChatExporter is licensed under the [MIT License](License.txt).
