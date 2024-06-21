using UnityEngine;
using UnityEditor;

namespace BUDDYWORKS.ExpressionsExtension
{  
    public class PrefabSpawner : MonoBehaviour
    {
        // VRCF Prefab definitions
        static string prefabEE_VRCF = "fb61f054361106040888ef1bb33b44ad";
        static string VRCF_Path = "Packages/com.vrcfury.vrcfury";

        // Toolbar Menu
        [MenuItem("BUDDYWORKS/Expression Extension/Spawn Prefab... [VRCFury]", false, 0)]
        [MenuItem("GameObject/BUDDYWORKS/Expression Extension/Spawn Prefab... [VRCFury]", false, 0)]
        private static void SpawnEE()
        {
            SpawnPrefab(prefabEE_VRCF);
        }

        // Enable or disable menu items dynamically

        [MenuItem("BUDDYWORKS/Expression Extension/Spawn Prefab... [VRCFury]", true)]
        [MenuItem("GameObject/BUDDYWORKS/Expression Extension/Spawn Prefab... [VRCFury]", true)]
        private static bool ValidateSpawnPE()
        {
            return AssetDatabase.IsValidFolder(VRCF_Path) != false;
        }

        // Prefab Spawner
        private static void SpawnPrefab(string guid)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(guid);

            if (string.IsNullOrEmpty(prefabPath))
            {
                Debug.LogError("Prefab with GUID " + guid + " not found.");
                return;
            }

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            GameObject selectedObject = Selection.activeGameObject;

            if (prefab == null)
            {
                Debug.LogError("Failed to load prefab with GUID " + guid + " at path " + prefabPath);
                return;
            }

            GameObject instantiatedPrefab = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

            if (selectedObject != null)
            {
                instantiatedPrefab.transform.parent = selectedObject.transform;
            }

            if (instantiatedPrefab != null)
            {
                EditorGUIUtility.PingObject(instantiatedPrefab);
            }
            else
            {
                Debug.LogError("Failed to instantiate prefab with GUID " + guid);
            }
        }
    }
}