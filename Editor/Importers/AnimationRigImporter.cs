using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FasterGames.Aseprite.Editor.Library;
using FasterGames.Aseprite.Editor.Library.Chunks;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace FasterGames.Aseprite.Editor.Importers
{
    public class AnimationRigImporter : BaseImporter<AseFile, AnimationRigImporter.Result>
    {
        [Serializable]
        public class UserOptions
        {
            public bool createRig = true;
            public List<string> loopTags = new List<string>();
        }
        
        public class Options
        {
            public UserOptions userOptions;
            public Dictionary<string, List<Sprite>> spritesByLayerName;
            public GameObject rootObject;
        }
        
        public class Result
        {
            public Avatar avatar;
            public Dictionary<string, AvatarMask> layerMasks;
            public List<AnimationClip> clips;
        }

        private Options m_Options;
        
        public AnimationRigImporter(AseFile source, Options opts) : base(source)
        {
            this.m_Options = opts;
        }

        public override Result ImportAsset(AssetImportContext ctx)
        {
            // nothing to do
            if (!m_Options.userOptions.createRig)
                return null;
            
            // get the layers, for future use
            var layerChunks = Source.GetChunks<LayerChunk>();

            // create the avatar
            var rootAvatar = AvatarBuilder.BuildGenericAvatar(m_Options.rootObject, m_Options.rootObject.name);
            rootAvatar.name = AsepriteMetadata.FormatName(ctx.assetPath, null, null);

            if (!rootAvatar.isValid)
            {
                ctx.LogImportError("Failed to create valid avatar", rootAvatar);
                return null;
            }
            
            ctx.AddObjectToAsset(rootAvatar.name, rootAvatar);

            m_Options.rootObject.GetComponent<Animator>().avatar = rootAvatar;

            Dictionary<string, AvatarMask> layerMasks = new Dictionary<string, AvatarMask>();
            
            // build the layer avatar masks
            for (var layerIndex = 0; layerIndex < layerChunks.Count; layerIndex++)
            {
                var layerName = layerChunks[layerIndex].LayerName;
                var layerAvatarMaskName = AsepriteMetadata.FormatName(ctx.assetPath, layerName, null);
                var layerAvatarMask = new AvatarMask {name = layerAvatarMaskName};
                layerAvatarMask.AddTransformPath(m_Options.rootObject.transform, true);
                for (var transformIndex = 0; transformIndex < layerAvatarMask.transformCount; transformIndex++)
                {
                    var currentPath = layerAvatarMask.GetTransformPath(transformIndex);
                    layerAvatarMask.SetTransformActive(transformIndex, currentPath == layerName);
                }
                
                layerMasks.Add(layerName, layerAvatarMask);
                
                ctx.AddObjectToAsset(layerAvatarMask.name, layerAvatarMask);
            }
            
            // store the aseprite anims for future use
            var frameTags = Source.GetAnimations();

            // store the frame durations for future use
            var frameDurations = Source.Frames.Select(f => f.FrameDuration).ToList();

            var clips = new List<AnimationClip>();
            
            foreach (var frameTag in frameTags)
            {
                var loopTime = m_Options.userOptions.loopTags.Contains(frameTag.TagName);
                var fullAnimationName = AsepriteMetadata.FormatName(ctx.assetPath, null, frameTag.TagName);
                var fullClip = GenerateAnimation(fullAnimationName, frameTag, frameDurations, m_Options.spritesByLayerName, loopTime);

                clips.Add(fullClip);
                ctx.AddObjectToAsset(fullClip.name, fullClip);

                foreach (var spriteLayerPair in m_Options.spritesByLayerName)
                {
                    var layerName = spriteLayerPair.Key;
                    var layerSprites = spriteLayerPair.Value;
                    var layerAnimationName = AsepriteMetadata.FormatName(ctx.assetPath, layerName, frameTag.TagName);

                    var layerClip = GenerateAnimation(layerAnimationName, frameTag, frameDurations, layerName, layerSprites, loopTime);
                    
                    clips.Add(layerClip);
                    ctx.AddObjectToAsset(layerClip.name, layerClip);
                }
            }

            return new Result()
            {
                avatar = rootAvatar,
                clips = clips,
                layerMasks = layerMasks
            };
        }

        private AnimationClip GenerateAnimation(string name, FrameTag tag, List<ushort> tagDurations,
            Dictionary<string, List<Sprite>> allLayerSprites, bool loopTime)
        {
            // create the clip
            var clip = new AnimationClip
            {
                name = name,
                frameRate = 25,
            };

            foreach (var pair in allLayerSprites)
            {
                var layerName = pair.Key;
                var layerSprites = pair.Value;
                
                GenerateAnimation(clip, tag, tagDurations, layerName, layerSprites);
            }
            
            GenerateAnimationPost(clip, tag, loopTime);

            return clip;
        }
        
        private AnimationClip GenerateAnimation(string name, FrameTag tag, List<ushort> tagDurations,
            string layerName, List<Sprite> layerSprites, bool loopTime)
        {
            // create the clip
            var clip = new AnimationClip
            {
                name = name,
                frameRate = 25
            };
            
            GenerateAnimation(clip, tag, tagDurations, layerName, layerSprites);
            GenerateAnimationPost(clip, tag, loopTime);

            return clip;
        }
        
        private AnimationClip GenerateAnimation(AnimationClip clip, FrameTag tag, List<ushort> tagDurations,
            string layerName, List<Sprite> layerSprites)
        {
            // and the sprite value binding
            var spriteBinding = new EditorCurveBinding
            {
                type = typeof(SpriteRenderer),
                path = layerName,
                propertyName = "m_Sprite"
            };

            // calculate the number of keyframes
            var length = tag.FrameTo - tag.FrameFrom + 1;

            // length, plus last frame to keep the duration
            var spriteKeyFrames = new ObjectReferenceKeyframe[length + 1];

            float time = 0;

            int from = (tag.Animation != LoopAnimation.Reverse) ? tag.FrameFrom : tag.FrameTo;
            var step = (tag.Animation != LoopAnimation.Reverse) ? 1 : -1;

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
                    value = layerSprites[from + i]
                };

                time += tagDurations[keyIndex] / 1000f;

                keyIndex += step;
                spriteKeyFrames[i] = frame;
            }

            float frameTime = 1f / clip.frameRate;

            // configure the final frame
            ObjectReferenceKeyframe lastFrame = new ObjectReferenceKeyframe
            {
                time = time - frameTime,
                value = layerSprites[from + (length - 1)]
            };

            spriteKeyFrames[spriteKeyFrames.Length - 1] = lastFrame;
            
            // store the result
            AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, spriteKeyFrames);

            return clip;
        }

        private void GenerateAnimationPost(AnimationClip clip, FrameTag anim, bool loopTime)
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