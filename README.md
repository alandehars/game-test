# Outer Rim Run

An original d20-inspired smuggler adventure starring Sabrina, a human caught between Imperial security and a Rebel relief operation. This fan-made project is unofficial and is not affiliated with or endorsed by Lucasfilm or Disney.

## Play the pixel game

On Windows, launch `OuterRimPixel.exe`. Click an action or press **1–5**. Press **S** to see Sabrina's sheet. The graphical build uses pixel-art scenes, animated rain, dice checks, and a branching story. Press **R** after an ending to restart.

The console build, `OuterRimRun.exe`, is also included.

## Build

With the .NET Framework C# compiler installed:

```bat
csc /nologo /target:winexe /optimize+ /r:System.Windows.Forms.dll /r:System.Drawing.dll /out:OuterRimPixel.exe OuterRimPixel.cs
csc /nologo /target:exe /optimize+ /out:OuterRimRun.exe OuterRimRun.cs
```

The games track health, credits, time, Imperial heat, dice results, and campaign clues. These are compact story prototypes, not full implementations of a published Star Wars RPG ruleset.
