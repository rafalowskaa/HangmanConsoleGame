# Hangman Console Game

A bilingual console implementation of Hangman, with single-player and local multiplayer modes, rankings and sound.

**C# · .NET 8 · NAudio · Newtonsoft.Json**

[Selected projects](https://github.com/rafalowskaa/BrowserGamesPlatform/blob/main/PORTFOLIO.md)

## Features

- Polish and English interface selection.
- Single-player and local multiplayer game modes.
- Round settings and player names.
- A local score ranking with a reset option.
- Background music and sound effects.
- Keyboard navigation and terminal-size monitoring.

## Run locally

Use Windows and .NET 8 SDK. The audio implementation uses NAudio; cross-platform audio support is not established.

```powershell
git clone https://github.com/rafalowskaa/HangmanConsoleGame.git
cd HangmanConsoleGame
dotnet restore WisielecConsole.csproj
dotnet run --project WisielecConsole.csproj
```

Use a terminal window of at least **80 columns × 25 rows**. The interface positions text explicitly; enlarge the window if necessary.

## Build

```powershell
dotnet build WisielecConsole.csproj --configuration Release
```

Keep the copied `Data` and `Sounds` directories alongside the compiled application.

## Structure

| Directory | Purpose |
| --- | --- |
| `Controller` | Game flow |
| `Model` | Game state, players, settings and ranking entries |
| `View` | Console presentation |
| `Resources` | Language resources |
| `Utilities` | Input validation, console and audio helpers |
| `Data` | Word list and local scores |
| `Sounds` | Audio assets |

## Project status

This is a console portfolio project. The repository has a build workflow, but no dedicated automated test project. The presence of a workflow is not a claim that its latest run passed.
