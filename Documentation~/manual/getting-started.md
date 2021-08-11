# Getting Started

To import an Aseprite file, drag it into the [Project window](https://docs.unity3d.com/Manual/ProjectView.html). That's it!

## Additional configuration

- `loopTime`: Overrides animation loop detection to always loop animations
- `generateRootTransform`: Generates a Prefab next to the Aseprite asset containing a hierarchy that mirrors the Aseprite layers, with the necessary components to run animations. __Note: This will overwrite the prefab on re-import.__
- `sortingLayerName`: The sorting layer name to use for the root transform SpriteRenderer components
- `baseSortingOrder`: The starting sorting order from which the layers will start. Aseprite layer index is added to this value.
- `textureSettings`: The texture settings Unity will use for Texture import. See [Unity Docs: Texture Import Settings](https://docs.unity3d.com/Manual/class-TextureImporter.html)
