using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Buddyworks.ExpressionsExtension
{
    public class AnimationReassign : EditorWindow
    {
        private SkinnedMeshRenderer skinnedMeshRenderer;
        private string[] blendShapeNames;
        private Dictionary<AnimationClip, int> selectedBlendShapes = new Dictionary<AnimationClip, int>();
        private Vector2 scrollPosition;

        private string[] animationDirectories = new string[]
        {
            "Packages/wtf.buddyworks.expressionsextension/Expressions Extension/Animations/Bank_Eyes",
            "Packages/wtf.buddyworks.expressionsextension/Expressions Extension/Animations/Bank_Mouth"
        };

        private string specialAnimationPath = "Packages/wtf.buddyworks.expressionsextension/Expressions Extension/Animations/Bank_Additive/Pupils.anim";
        private List<AnimationClip> animationClips = new List<AnimationClip>();
        private AnimationClip specialAnimationClip;

        [MenuItem("BUDDYWORKS/Expression Extension/Assign Custom Blendshapes (Experimental)")]
        public static void ShowWindow()
        {
            GetWindow<AnimationReassign>("Expressions Extension");
        }

        private void OnEnable()
        {
            LoadAnimationClips();
        }

        private void LoadAnimationClips()
        {
            animationClips.Clear();

            foreach (string directory in animationDirectories)
            {
                string[] guids = AssetDatabase.FindAssets("t:AnimationClip", new[] { directory });

                foreach (string guid in guids)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(assetPath);
                    if (!clip) continue;
                    animationClips.Add(clip);
                    selectedBlendShapes[clip] = 0;
                }
            }

            specialAnimationClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(specialAnimationPath);
            if (specialAnimationClip)
            {
                selectedBlendShapes[specialAnimationClip] = 0;
            }
        }

        private void OnGUI()
        {
            GUILayout.Label("Mesh containing your face blendshapes", EditorStyles.boldLabel);
            skinnedMeshRenderer = (SkinnedMeshRenderer)EditorGUILayout.ObjectField(skinnedMeshRenderer, typeof(SkinnedMeshRenderer), true);

            if (!skinnedMeshRenderer) return;
            if (blendShapeNames == null || blendShapeNames.Length != skinnedMeshRenderer.sharedMesh.blendShapeCount)
            {
                blendShapeNames = Enumerable.Range(0, skinnedMeshRenderer.sharedMesh.blendShapeCount)
                    .Select(i => skinnedMeshRenderer.sharedMesh.GetBlendShapeName(i))
                    .Where(name => !name.StartsWith("vrc.", StringComparison.OrdinalIgnoreCase))
                    .ToArray();
            }

            GUILayout.Space(12);
            GUILayout.Label("WARNING: This will overwrite native animations!", EditorStyles.boldLabel);
            GUILayout.Label("Reimport package to restore!", EditorStyles.boldLabel);
            GUILayout.Label("---------------------------------", EditorStyles.boldLabel);
            GUILayout.Label("Do not use blendshapes", EditorStyles.boldLabel);
            GUILayout.Label("referenced in the descriptor!", EditorStyles.boldLabel);
            GUILayout.Space(12);
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            foreach (AnimationClip clip in animationClips)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(clip.name, GUILayout.Width(200));
                selectedBlendShapes[clip] = EditorGUILayout.Popup(selectedBlendShapes[clip], blendShapeNames);
                GUILayout.EndHorizontal();
            }

            if (specialAnimationClip)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(specialAnimationClip.name, GUILayout.Width(200));
                selectedBlendShapes[specialAnimationClip] = EditorGUILayout.Popup(selectedBlendShapes[specialAnimationClip], blendShapeNames);
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();

            if (GUILayout.Button("Generate"))
            {
                GenerateExpressions();
            }
            
            GUILayout.Label("Expressions Extension - BUDDYWORKS", EditorStyles.boldLabel);
            Rect labelRect = GUILayoutUtility.GetLastRect();
            if (Event.current.type == EventType.MouseDown && labelRect.Contains(Event.current.mousePosition))
            {
                Application.OpenURL("https://github.com/BUDDYWORKS-VR/expressions-extension");
            }
        }

        private void GenerateExpressions()
        {
            foreach ((AnimationClip clip, int blendShapeIndex) in selectedBlendShapes)
            {
                string blendShapeName = blendShapeNames[blendShapeIndex];

                RemoveAllProperties(clip);

                if (clip == specialAnimationClip)
                {
                    GenerateSpecialAnimation(clip, blendShapeName);
                }
                else
                {
                    GenerateStandardAnimation(clip, blendShapeName);
                }
            }

            Debug.Log("Expressions generated successfully.");
        }

        private static void RemoveAllProperties(AnimationClip clip)
        {
            var bindings = AnimationUtility.GetCurveBindings(clip);
            foreach (EditorCurveBinding binding in bindings)
            {
                AnimationUtility.SetEditorCurve(clip, binding, null);
            }
        }

        private void GenerateStandardAnimation(AnimationClip clip, string blendShapeName)
        {
            Transform transform = skinnedMeshRenderer.transform;
            string path = AnimationUtility.CalculateTransformPath(transform, transform.parent);
            string propertyName = $"blendShape.{blendShapeName}";

            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(0.0f, 100.0f);

            AnimationUtility.SetEditorCurve(clip, EditorCurveBinding.FloatCurve(path, typeof(SkinnedMeshRenderer), propertyName), curve);

            Debug.Log($"Added blend shape '{blendShapeName}' to clip '{clip.name}' at value 100.");
        }

        private void GenerateSpecialAnimation(AnimationClip clip, string blendShapeName)
        {
            Transform transform = skinnedMeshRenderer.transform;
            string localPath = AnimationUtility.CalculateTransformPath(transform, transform.parent);

            string propertyName = $"blendShape.{blendShapeName}";

            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(0.0f, 0.0f);
            curve.AddKey(1.0f, 100.0f);

            AnimationUtility.SetEditorCurve(clip, EditorCurveBinding.FloatCurve(localPath, typeof(SkinnedMeshRenderer), propertyName), curve);

            Debug.Log($"Added special animation for blend shape '{blendShapeName}' to clip '{clip.name}' with keyframes at 0 and 1.");
        }
    }
}
