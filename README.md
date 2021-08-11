# Aseprite

An Aseprite importer for Unity3D. 🎨📤

![Project logo; A pink package on a grey background, next to the text "T4 Templates" in purple](./Documentation~/header.png)

![GitHub package.json version](https://img.shields.io/github/package-json/v/faster-games/aseprite)
[![openupm](https://img.shields.io/npm/v/com.faster-games.aseprite?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.faster-games.aseprite/)
[![CI](https://github.com/faster-games/aseprite/actions/workflows/main.yml/badge.svg)](https://github.com/faster-games/aseprite/actions/workflows/main.yml)
[![Discord](https://img.shields.io/discord/862006447919726604)](https://discord.gg/QfQE6rWQqq)


[Aseprite](https://www.aseprite.org/) is an animated sprite editor and pixel art tool. This package supports taking Aseprite files (`.ase`, `.aseprite`) and importing them into [Unity](http://unity3d.com/). Most importantly, it preserves layers - importing them individually rather than as a single combined frame. This allows for further customization inside the engine. The importer will also import animations (including frame timings) as both individual animations, and "combined" animations designed to be applied to a top-level [Game Object](https://docs.unity3d.com/ScriptReference/GameObject.html) that mirrors the structure of Aseprite layers. If desired, such a Game Object can also be generated during import.

## Installing

This package supports [openupm](https://openupm.com/packages/com.faster-games.aseprite/) - you can install it using the following command:

```
openupm add com.faster-games.aseprite
```

Or by adding directly to your `manifest.json`:

> Note: You may also use specific versions by appending `#{version}` where version is a [Release tag](https://github.com/faster-games/aseprite/releases) - e.g. `#v1.2.0`.

```
dependencies: {
	...
	"com.faster-games.aseprite": "git+https://github.com/faster-games/aseprite.git"
}
```

Or by using [Package Manager](https://docs.unity3d.com/Manual/upm-ui-giturl.html) to "Add a package from Git URL", using the following url:

```
https://github.com/faster-games/aseprite.git
```

## Documentation

<center>

[Manual 📖](https://aseprite.faster-games.com/manual/getting-started.html) | [Scripting API 🔎](https://aseprite.faster-games.com/ref/FasterGames.Aseprite.Editor.html)

</center>

The importer will automatically detect and import aseprite files, so all configuration is optional.
To modify configuration, select the Aseprite file in the [Project window](https://docs.unity3d.com/Manual/ProjectView.html).

### Quickstart

- Drag a `.ase` or `.aseprite` file into the [Project window](https://docs.unity3d.com/Manual/ProjectView.html) under `Assets`
- Select it in the [Project window](https://docs.unity3d.com/Manual/ProjectView.html)
- Note that the importer used is `AsepriteImporter` - that's us!
- Any processing errors will be shown in the [Console](https://docs.unity3d.com/Manual/Console.html)

## Supporting the project

If this project saved you some time, and you'd like to see it continue to be invested in, consider [buying me a coffee. ☕](https://www.buymeacoffee.com/bengreenier) I do this full-time, and every little bit helps! 💙
