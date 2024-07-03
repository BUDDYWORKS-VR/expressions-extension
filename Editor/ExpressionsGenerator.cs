using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BUDDYWORKS.ExpressionsExtension
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
                    if (clip != null)
                    {
                        animationClips.Add(clip);
                        selectedBlendShapes[clip] = 0;
                    }
                }
            }

            specialAnimationClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(specialAnimationPath);
            if (specialAnimationClip != null)
            {
                selectedBlendShapes[specialAnimationClip] = 0;
            }
        }

        private void OnGUI()
        {
            GUILayout.Label("Mesh containing your face blendshapes", EditorStyles.boldLabel);
            skinnedMeshRenderer = (SkinnedMeshRenderer)EditorGUILayout.ObjectField(skinnedMeshRenderer, typeof(SkinnedMeshRenderer), true);

            if (skinnedMeshRenderer != null)
            {
                if (blendShapeNames == null || blendShapeNames.Length != skinnedMeshRenderer.sharedMesh.blendShapeCount)
                {
                    blendShapeNames = new string[skinnedMeshRenderer.sharedMesh.blendShapeCount];
                    for (int i = 0; i < blendShapeNames.Length; i++)
                    {
                        blendShapeNames[i] = skinnedMeshRenderer.sharedMesh.GetBlendShapeName(i);
                    }
                }

                GUILayout.Label("WARNING: This will overwrite native animations!", EditorStyles.boldLabel);
                GUILayout.Label("Reimport package to restore!", EditorStyles.boldLabel);
                GUILayout.Label("---------------------------------", EditorStyles.boldLabel);
                GUILayout.Label("Do not use blendshapes", EditorStyles.boldLabel);
                GUILayout.Label("from your avatar descriptor like vrc.v_aa!", EditorStyles.boldLabel);

                scrollPosition = GUILayout.BeginScrollView(scrollPosition);

                foreach (var clip in animationClips)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(clip.name, GUILayout.Width(200));
                    selectedBlendShapes[clip] = EditorGUILayout.Popup(selectedBlendShapes[clip], blendShapeNames);
                    GUILayout.EndHorizontal();
                }

                if (specialAnimationClip != null)
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
            }
        }

        private void GenerateExpressions()
        {
            foreach (var entry in selectedBlendShapes)
            {
                AnimationClip clip = entry.Key;
                int blendShapeIndex = entry.Value;
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

        private void RemoveAllProperties(AnimationClip clip)
        {
            var bindings = AnimationUtility.GetCurveBindings(clip);
            foreach (var binding in bindings)
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