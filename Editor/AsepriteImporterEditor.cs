using System;
using System.Linq;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace FasterGames.Aseprite.Editor
{
    [CustomEditor(typeof(AsepriteImporter))]
    public class AsepriteImporterEditor : ScriptedImporterEditor
    {
        public override bool showImportedObject => false;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(AsepriteImporter.loopTime)));

            
            serializedObject.FindProperty(nameof(AsepriteImporter.generateRootTransform)).boolValue = EditorGUILayout.BeginToggleGroup(
                "Generate Root Transform", serializedObject.FindProperty(nameof(AsepriteImporter.generateRootTransform)).boolValue);
            
            if (serializedObject.FindProperty(nameof(AsepriteImporter.generateRootTransform)).boolValue)
            {
                var sortingLayers = SortingLayer.layers.Select(l => l.name).ToArray();
                var currentIndex = Array.IndexOf(sortingLayers, serializedObject.FindProperty(nameof(AsepriteImporter.sortingLayerName)).stringValue);
                currentIndex = currentIndex == -1 ? 0 : currentIndex;
                var desiredIndex =
                    EditorGUILayout.Popup(nameof(AsepriteImporter.sortingLayerName), currentIndex, sortingLayers);

                if (desiredIndex != currentIndex)
                {
                    serializedObject.FindProperty(nameof(AsepriteImporter.sortingLayerName)).stringValue =
                        sortingLayers[desiredIndex];
                }


                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(AsepriteImporter.baseSortingOrder)));
            }

            EditorGUILayout.EndToggleGroup();

            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(AsepriteImporter.textureSettings)));

            if (assetTarget != null && assetTarget is AsepriteAsset)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Imported Object", EditorStyles.boldLabel);
                
                var aseprite = (AsepriteAsset) assetTarget;
                var serializedSprite = new SerializedObject(aseprite);

                EditorGUILayout.PropertyField(serializedSprite.FindProperty(nameof(AsepriteAsset.width)));
                EditorGUILayout.PropertyField(serializedSprite.FindProperty(nameof(AsepriteAsset.height)));
                EditorGUILayout.PropertyField(serializedSprite.FindProperty(nameof(AsepriteAsset.frameCount)));
                EditorGUILayout.PropertyField(serializedSprite.FindProperty(nameof(AsepriteAsset.layerCount)));

                var wasEnabled = GUI.enabled;
                GUI.enabled = false;
                for (var i = 0; i < aseprite.layerNames.Length; i++)
                {
                    EditorGUILayout.LabelField($"{nameof(AsepriteAsset.layerNames)}[{i}]", aseprite.layerNames[i]);
                }

                GUI.enabled = wasEnabled;
            }

            serializedObject.ApplyModifiedProperties();
            base.ApplyRevertGUI();
        }
    }
}