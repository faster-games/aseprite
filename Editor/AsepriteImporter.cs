using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FasterGames.Aseprite.Editor.Library;
using FasterGames.Aseprite.Editor.Library.Chunks;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace FasterGames.Aseprite.Editor
{
    [ScriptedImporter(version: 1, new []{"ase", "aseprite"})]
    public class AsepriteImporter : ScriptedImporter
    {
        public static readonly string DefaultElementPrefix = "default";

        public bool loopTime = false;

        public bool generateRootTransform = false;
        public string sortingLayerName;
        public int baseSortingOrder = 0;
        
        public TextureSettings textureSettings;

        public override void OnImportAsset(AssetImportContext ctx)
        {
            // open and read the file
            using var fs = new FileStream(ctx.assetPath, FileMode.Open, FileAccess.Read);
            var file = new AseFile(fs);
            
            // get the layers, for future use
            var layerChunks = file.GetChunks<LayerChunk>();
            
            // create a mainObject
            var mainObject = ScriptableObject.CreateInstance<AsepriteAsset>();
            mainObject.width = file.Header.Width;
            mainObject.height = file.Header.Height;
            mainObject.frameCount = file.Header.Frames;
            mainObject.layerCount = layerChunks.Count;
            mainObject.layerNames = layerChunks.Select(c => c.LayerName).ToArray();
            ctx.AddObjectToAsset("main", mainObject);
            ctx.SetMainObject(mainObject);

            try
            {
                // cheat to detect if this is our first import
                var res = GenerateSprite(Texture2D.redTexture, "firstImportDetector", ctx.assetPath);
                DestroyImmediate(res.texture);
            }
            catch (Exception)
            {
                // first import
                EditorUtils.Once(() => { SaveAndReimport(); });
                return;
            }
            
            // store the aseprite anims for future use
            var frameTags = file.GetAnimations();

            // store the frame durations for future use
            var frameDurations = file.Frames.Select(f => f.FrameDuration).ToList();
            
            // declare storage for layer sprites by name
            var layerSpritesByName = new Dictionary<string, List<Sprite>>();

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
                var layerFrames = file.GetFrames(layerFrameSelector);
                
                // declare storage for the sprites we're about to generate
                var layerSprites = new List<Sprite>();

                // walk the frames, creating sprites and textures
                for (var i = 0; i < layerFrames.Length; i++)
                {
                    // try to make sprite(s) for the layer
                    var spriteResult = GenerateSprite(layerFrames[i], $"{layerName}_{i}", ctx.assetPath);

                    foreach (var warning in spriteResult.importWarnings)
                    {
                        Debug.LogWarning(warning);
                    }

                    // need texture as well as sprite, add it
                    var tex = spriteResult.texture;
                    tex.name = $"{layerName}_tex{i}";
                    ctx.AddObjectToAsset(tex.name, tex, tex);

                    // add all the sprites
                    foreach (var sprite in spriteResult.sprites)
                    {
                        ctx.AddObjectToAsset(sprite.name, sprite, tex);
                    }

                    // store them for later reference
                    layerSprites.AddRange(spriteResult.sprites);
                }

                // add animations
                foreach (var frameTag in frameTags)
                {
                    var animationResult = GenerateAnimation(frameTag, frameDurations, layerSprites, layerName);

                    ctx.AddObjectToAsset(animationResult.name, animationResult);
                }
                
                // store the layer sprite info for final animation generation
                layerSpritesByName[layerName] = layerSprites;
            }
            
            // generate full animations, with bound paths for components
            foreach (var frameTag in frameTags)
            {
                var fullAnimationResult = GenerateFullAnimation(frameTag, frameDurations, layerSpritesByName);
                
                ctx.AddObjectToAsset(fullAnimationResult.name, fullAnimationResult);
            }

            // if requested, ensure a root transform object exists and is up to date
            if (generateRootTransform)
            {
                var rootTransformPath = Path.GetDirectoryName(ctx.assetPath) + "/" +
                                        Path.GetFileNameWithoutExtension(ctx.assetPath) + ".prefab";

                var rootTransform = new GameObject(Path.GetFileNameWithoutExtension(ctx.assetPath));
                rootTransform.AddComponent<Animator>();

                // walk once to ensure created
                for (var layerIndex = 0; layerIndex < layerChunks.Count; layerIndex++)
                {
                    var layerName = layerChunks[layerIndex].LayerName;
                    var layerObject = new GameObject(layerName);
                    layerObject.transform.SetParent(rootTransform.transform);

                    var sr = layerObject.AddComponent<SpriteRenderer>();
                    sr.sortingLayerName = sortingLayerName;
                    sr.sortingOrder = baseSortingOrder + layerIndex;
                }
                
                // again to set child index
                for (var layerIndex = 0; layerIndex < layerChunks.Count; layerIndex++)
                {
                    var layerName = layerChunks[layerIndex].LayerName;

                    var layerObject = rootTransform.transform.Find(layerName);
                    layerObject.SetSiblingIndex(layerChunks.Count - layerIndex - 1);
                }

                PrefabUtility.SaveAsPrefabAsset(rootTransform, rootTransformPath);
                DestroyImmediate(rootTransform);
                
                EditorUtils.Once(() =>
                {
                    AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                });
            }
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
                textureImporterSettings = textureSettings.ToImporterSettings(),
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

        private AnimationClip GenerateFullAnimation(FrameTag anim, List<ushort> frameDurations,
            Dictionary<string, List<Sprite>> allComponentSprites)
        {
            // create the clip
            var clip = new AnimationClip
            {
                name = anim.TagName,
                frameRate = 25
            };

            foreach (var pair in allComponentSprites)
            {
                var componentName = pair.Key;
                var componentSprites = pair.Value;
                
                GenerateAnimation(clip, anim, frameDurations, componentSprites, componentName, bindPath: true);
            }
            
            GenerateAnimationPost(clip, anim);

            return clip;
        }

        private AnimationClip GenerateAnimation(FrameTag anim, List<ushort> frameDurations,
            List<Sprite> componentSprites, string componentName)
        {
            // create the clip
            var clip = new AnimationClip
            {
                name = $"{componentName}_{anim.TagName}",
                frameRate = 25
            };
            
            GenerateAnimation(clip, anim, frameDurations, componentSprites, componentName);
            GenerateAnimationPost(clip, anim);

            return clip;
        }

        private void GenerateAnimation(AnimationClip clip, FrameTag anim, List<ushort> frameDurations, List<Sprite> componentSprites, string componentName, bool bindPath = false)
        {
            // and the sprite value binding
            var spriteBinding = new EditorCurveBinding
            {
                type = typeof(SpriteRenderer),
                path = bindPath ? componentName : "",
                propertyName = "m_Sprite"
            };
            
            // calculate the number of keyframes
            var length = anim.FrameTo - anim.FrameFrom + 1;
            
            // length, plus last frame to keep the duration
            var spriteKeyFrames = new ObjectReferenceKeyframe[length + 1];
            
            float time = 0;

            int from = (anim.Animation != LoopAnimation.Reverse) ? anim.FrameFrom : anim.FrameTo;
            var step = (anim.Animation != LoopAnimation.Reverse) ? 1 : -1;
            
            var keyIndex = from;

            // walk the keyframes
            for (var i = 0; i < length; i++)
            {
                if (i >= length)
                {
                    keyIndex = from;
                }

                // create the reference frame to bind the sprite for this frame
                ObjectReferenceKeyframe frame = new ObjectReferenceKeyframe
                {
                    time = time,
                    value = componentSprites[from + i]
                };

                time += frameDurations[keyIndex] / 1000f;

                keyIndex += step;
                spriteKeyFrames[i] = frame;
            }

            float frameTime = 1f / clip.frameRate;

            // configure the final frame
            ObjectReferenceKeyframe lastFrame = new ObjectReferenceKeyframe
            {
                time = time - frameTime,
                value = componentSprites[from + (length - 1)]
            };
            
            spriteKeyFrames[spriteKeyFrames.Length - 1] = lastFrame;
            
            // store the result
            AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, spriteKeyFrames);
        }
        
        private void GenerateAnimationPost(AnimationClip clip, FrameTag anim)
        {
            // get the clip settings, to edit it
            AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);

            // ensure the wrap mode and loop values are correct
            switch (anim.Animation)
            {
                case LoopAnimation.Forward:
                    clip.wrapMode = WrapMode.Loop;
                    settings.loopTime = true;
                    break;
                case LoopAnimation.Reverse:
                    clip.wrapMode = WrapMode.Loop;
                    settings.loopTime = true;
                    break;
                case LoopAnimation.PingPong:
                    clip.wrapMode = WrapMode.PingPong;
                    settings.loopTime = true;
                    break;
            }

            // editor override to disable wrapping
            if (!loopTime)
            {
                clip.wrapMode = WrapMode.Once;
                settings.loopTime = false;
            }

            // save the result
            AnimationUtility.SetAnimationClipSettings(clip, settings);
        }
    }
}