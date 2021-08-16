using System;
using System.Collections.Generic;
using FasterGames.Aseprite.Editor.Library;
using FasterGames.Aseprite.Editor.Library.Chunks;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FasterGames.Aseprite.Editor.Importers
{
    public class TextureSpriteImporter : BaseImporter<AseFile, Dictionary<string, List<Sprite>>>
    {
        [Serializable]
        public class UserOptions
        {
            // use a seamless cubemap?
            public bool seamlessCubemap;

            //     Mip map bias of the texture.
            public float mipmapBias = 0.5f;

            //     Texture coordinate wrapping mode.
            public TextureWrapMode wrapMode = TextureWrapMode.Clamp;

            //     Texture U coordinate wrapping mode.
            public TextureWrapMode wrapModeU;

            //     Texture V coordinate wrapping mode.
            public TextureWrapMode wrapModeV;

            //     Texture W coordinate wrapping mode for Texture3D.
            public TextureWrapMode wrapModeW;

            //     If the provided alpha channel is transparency, enable this to dilate the color
            //     to avoid filtering artifacts on the edges.
            public bool alphaIsTransparency = true;

            //     Sprite texture import mode.
            public SpriteImportMode spriteMode = SpriteImportMode.Multiple;

            //     The number of pixels in the sprite that correspond to one unit in world space.
            public float spritePixelsPerUnit = 100;

            //     The tessellation detail to be used for generating the mesh for the associated
            //     sprite if the SpriteMode is set to Single. For Multiple sprites, use the SpriteEditor
            //     to specify the value per sprite. Valid values are in the range [0-1], with higher
            //     values generating a tighter mesh. A default of -1 will allow Unity to determine
            //     the value automatically.
            public float spriteTessellationDetail;

            //     The number of blank pixels to leave between the edge of the graphic and the mesh.
            public uint spriteExtrude = 1;

            //     SpriteMeshType defines the type of Mesh that TextureImporter generates for a
            //     Sprite.
            public SpriteMeshType spriteMeshType = SpriteMeshType.Tight;

            //     Edge-relative alignment of the sprite graphic.
            public int spriteAlignment = (int) SpriteAlignment.Center;

            //     Pivot point of the Sprite relative to its graphic's rectangle.
            public Vector2 spritePivot = Vector2.zero;

            //     Border sizes of the generated sprites.
            public Vector4 spriteBorder = Vector4.zero;

            //     Generates a default physics shape for a Sprite if a physics shape has not been
            //     set by the user.
            public bool spriteGenerateFallbackPhysicsShape = true;


            //     Anisotropic filtering level of the texture.
            public int aniso = 1;

            //     Filtering mode of the texture.
            public FilterMode filterMode = FilterMode.Point;

            //     Convolution mode.
            public TextureImporterCubemapConvolution cubemapConvolution;

            //     Which type of texture are we dealing with here.
            public TextureImporterType textureType = TextureImporterType.Sprite;

            //     Shape of imported texture.
            public TextureImporterShape textureShape = TextureImporterShape.Texture2D;

            //     Mipmap filtering mode.
            public TextureImporterMipFilter mipmapFilter;

            //     Generate mip maps for the texture?
            public bool mipmapEnabled = false;

            //     Is texture storing color data?
            public bool sRGBTexture = true;

            //     Fade out mip levels to gray color?
            public bool fadeOut = false;

            //     Enable this to avoid colors seeping out to the edge of the lower Mip levels.
            //     Used for light cookies.
            public bool borderMipmap;

            //     Enables or disables coverage-preserving alpha MIP mapping.
            public bool mipMapsPreserveCoverage;

            //     Mip level where texture begins to fade out to gray.
            public int mipmapFadeDistanceStart = 1;

            //     Returns or assigns the alpha test reference value.
            public float alphaTestReferenceValue = 1f;

            //     Convert heightmap to normal map?
            public bool convertToNormalMap;

            //     Amount of bumpyness in the heightmap.
            public float heightmapScale;

            //     Normal map filtering mode.
            public TextureImporterNormalFilter normalMapFilter;

            //     Select how the alpha of the imported texture is generated.
            public TextureImporterAlphaSource alphaSource = TextureImporterAlphaSource.FromInput;

            //     Color or Alpha component TextureImporterType|Single Channel Textures uses.
            public TextureImporterSingleChannelComponent singleChannelComponent;

            //     Is texture data readable from scripts.
            public bool readable = false;

            //     Enable mipmap streaming for this texture.
            public bool streamingMipmaps;

            //     Relative priority for this texture when reducing memory size in order to hit
            //     the memory budget.
            public int streamingMipmapsPriority;

            //     Scaling mode for non power of two textures.
            public TextureImporterNPOTScale npotScale;

            //     Cubemap generation mode.
            public TextureImporterGenerateCubemap generateCubemap;

            //     Mip level where texture is faded out to gray completely.
            public int mipmapFadeDistanceEnd = 3;

            public TextureImporterSettings ToImporterSettings()
            {
                return new TextureImporterSettings()
                {
                    seamlessCubemap = seamlessCubemap,
                    mipmapBias = mipmapBias,
                    wrapMode = wrapMode,
                    wrapModeU = wrapModeU,
                    wrapModeV = wrapModeV,
                    wrapModeW = wrapModeW,
                    alphaIsTransparency = alphaIsTransparency,
                    spriteMode = (int) spriteMode,
                    spritePixelsPerUnit = spritePixelsPerUnit,
                    spriteTessellationDetail = spriteTessellationDetail,
                    spriteExtrude = spriteExtrude,
                    spriteMeshType = spriteMeshType,
                    spriteAlignment = spriteAlignment,
                    spritePivot = spritePivot,
                    spriteBorder = spriteBorder,
                    spriteGenerateFallbackPhysicsShape = spriteGenerateFallbackPhysicsShape,
                    aniso = aniso,
                    filterMode = filterMode,
                    cubemapConvolution = cubemapConvolution,
                    textureType = textureType,
                    textureShape = textureShape,
                    mipmapFilter = mipmapFilter,
                    mipmapEnabled = mipmapEnabled,
                    sRGBTexture = sRGBTexture,
                    fadeOut = fadeOut,
                    borderMipmap = borderMipmap,
                    mipMapsPreserveCoverage = mipMapsPreserveCoverage,
                    mipmapFadeDistanceStart = mipmapFadeDistanceStart,
                    alphaTestReferenceValue = alphaTestReferenceValue,
                    convertToNormalMap = convertToNormalMap,
                    heightmapScale = heightmapScale,
                    normalMapFilter = normalMapFilter,
                    alphaSource = alphaSource,
                    singleChannelComponent = singleChannelComponent,
                    readable = readable,
                    streamingMipmaps = streamingMipmaps,
                    streamingMipmapsPriority = streamingMipmapsPriority,
                    npotScale = npotScale,
                    generateCubemap = generateCubemap,
                    mipmapFadeDistanceEnd = mipmapFadeDistanceEnd
                };
            }
        }
        
        private readonly UserOptions m_UserOptions;
        
        public TextureSpriteImporter(AseFile file, UserOptions opts) : base(file)
        {
            this.m_UserOptions = opts;
        }

        public bool TryCanImportInto(string assetPath)
        {
            try
            {
                // cheat to detect if this is our first import
                var res = GenerateSprite(Texture2D.redTexture, "firstImportDetector", assetPath);
                Object.DestroyImmediate(res.texture);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public override Dictionary<string, List<Sprite>> ImportAsset(AssetImportContext ctx)
        {
            var layerSpritesByName = new Dictionary<string, List<Sprite>>();
            
            // get the layers, for future use
            var layerChunks = Source.GetChunks<LayerChunk>();
            
            // walk the layers
            for (var layerIndex = 0; layerIndex < layerChunks.Count; layerIndex++)
            {
                var layer = layerChunks[layerIndex];
                
                // grab the data for the layer
                var layerName = layer.LayerName;
                var layerFrameSelector = new FrameOptions()
                {
                    include = new List<FrameOptions.LayerFilter>()
                    {
                        new FrameOptions.LayerFilter()
                        {
                            name = layerName,
                            content = layer.LayerName,
                            type = FrameOptions.LayerFilter.LayerFilterType.Equals
                        }
                    }
                };
                var layerFrames = Source.GetFrames(layerFrameSelector);
                
                // declare storage for the sprites we're about to generate
                var layerSprites = new List<Sprite>();

                // walk the frames, creating sprites and textures
                for (var i = 0; i < layerFrames.Length; i++)
                {
                    // try to make sprite(s) for the layer
                    var spriteName = AsepriteMetadata.FormatName(ctx.assetPath, layerName, $"{i}");
                    var spriteResult = GenerateSprite(layerFrames[i], spriteName, ctx.assetPath);

                    foreach (var warning in spriteResult.importWarnings)
                    {
                        Debug.LogWarning(warning);
                    }

                    // need texture as well as sprite, add it
                    var tex = spriteResult.texture;
                    tex.name = AsepriteMetadata.FormatName(ctx.assetPath, layerName, $"tex{i}");
                    ctx.AddObjectToAsset(tex.name, tex, tex);

                    // add all the sprites
                    foreach (var sprite in spriteResult.sprites)
                    {
                        ctx.AddObjectToAsset(sprite.name, sprite, tex);
                    }

                    // store them for later reference
                    layerSprites.AddRange(spriteResult.sprites);
                }
                
                // store the layer sprite info for final animation generation
                layerSpritesByName[layerName] = layerSprites;
            }

            return layerSpritesByName;
        }
        
        private TextureGenerationOutput GenerateSprite(Texture2D tex2D, string spriteName, string path)
        {
            SourceTextureInformation textureInformation = new SourceTextureInformation()
            {
                containsAlpha = true,
                hdr = false,
                height = tex2D.height,
                width = tex2D.width
            };
            
            TextureImporterPlatformSettings platformSettings = new TextureImporterPlatformSettings()
            {
                overridden = false
            };
            
            TextureGenerationSettings settings = new TextureGenerationSettings()
            {
                assetPath = path,
                spriteImportData = new []
                {
                    new SpriteImportData()
                    {
                        alignment = SpriteAlignment.Center,
                        border = Vector4.zero,
                        name = spriteName,
                        pivot = new Vector2(0.5f, 0.5f),
                        rect = new Rect(0, 0, tex2D.width, tex2D.height),
                        spriteID = spriteName,
                        tessellationDetail = 1,
                    }
                },
                textureImporterSettings = m_UserOptions.ToImporterSettings(),
                enablePostProcessor = false,
                sourceTextureInformation = textureInformation,
                qualifyForSpritePacking = true,
                platformSettings = platformSettings,
                spritePackingTag = "aseprite",
                secondarySpriteTextures = new SecondarySpriteTexture[0]
            };
        
            var res = TextureGenerator.GenerateTexture(settings,
                new Unity.Collections.NativeArray<Color32>(tex2D.GetPixels32(), Unity.Collections.Allocator.Temp));
        
            return res;
        }
    }
}