# Outer Rim Run

A compact, original d20-inspired Windows console adventure starring Sabrina, a human smuggler caught between Imperial security and a Rebel relief operation. This is a fan-made, unofficial project and is not affiliated with or endorsed by Lucasfilm or Disney.

## Run

On Windows, launch `OuterRimRun.exe`. The executable targets the .NET Framework 4.x already included with Windows. Use the numbered actions, type `sheet` to review Sabrina, and `quit` to pause.

## Build from source

Open a Developer Command Prompt for Visual Studio (or any shell with the .NET Framework C# compiler) and run:

```bat
csc /nologo /target:exe /out:OuterRimRun.exe OuterRimRun.cs
```

The game tracks health, credits, time, Imperial heat, dice results, and campaign clues for the current session. It is a small story prototype, not a full implementation of a published Star Wars RPG ruleset.
