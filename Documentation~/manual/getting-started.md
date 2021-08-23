# Getting Started

To import an Aseprite file, drag it into the [Project window](https://docs.unity3d.com/Manual/ProjectView.html). That's it!

## Additional configuration

### Prefab Options

Options for controlling prefab generation.

- `sortingLayerName`: The sorting layer name to use for the root transform SpriteRenderer components
- `baseSortingOrder`: The starting sorting order from which the layers will start. Aseprite layer index is added to this value.

### Texture Options

The texture settings Unity will use for Texture import. See [Unity Docs: Texture Import Settings](https://docs.unity3d.com/Manual/class-TextureImporter.html)

- `sliceOptions` - Extension to the default set, used to control Spritesheet dimension data, in the event you are importing a spritesheet. Leaving as default (`0`s) will import as one sprite per layer. Slicing will import as multiple sprites per layer.

### Rig Options

Options for controlling animation and rigging generation.

- `Create Rig` - If disabled, no rig (animations, avatars, etc) will be created. 
- `loopTags` - Controls which frame tags from the aseprite file will import as looping animations. By default, all animations are non-looping.
