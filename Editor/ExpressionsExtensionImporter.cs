using UnityEngine;
using UnityEditor;

namespace BUDDYWORKS.ExpressionsExtension
{  
    public class PrefabSpawner : MonoBehaviour
    {
        // VRCF Prefab definitions
        static string prefabEE_UnSynced_VRCF = "fb61f054361106040888ef1bb33b44ad";
        static string prefabEE_Synced_VRCF = "9c1b692672e052d44a475587f7d535fd";
        static string VRCF_Path = "Packages/com.vrcfury.vrcfury";

        // Toolbar Menu
        [MenuItem("BUDDYWORKS/Expression Extension/Spawn Unsynced Prefab... [VRCFury]", false, 0)]
        [MenuItem("GameObject/BUDDYWORKS/Expression Extension/Spawn Unsynced Prefab... [VRCFury]", false, 0)]
        private static void SpawnEE_Unsynced()
        {
            SpawnPrefab(prefabEE_UnSynced_VRCF);
        }

        [MenuItem("BUDDYWORKS/Expression Extension/Spawn Synced Prefab... [VRCFury]", false, 1)]
        [MenuItem("GameObject/BUDDYWORKS/Expression Extension/Spawn Synced Prefab... [VRCFury]", false, 0)]
        private static void SpawnEE_Synced()
        {
            SpawnPrefab(prefabEE_Synced_VRCF);
        }

        [MenuItem("BUDDYWORKS/Expression Extension/Sync cost: 25 memory", false , 20)]
        private static void Placeholder()
        {
            Debug.Log("Using the Synced prefab costs 25 parameter memory in your avatar.");
        }
        // Enable or disable menu items dynamically

        [MenuItem("BUDDYWORKS/Expression Extension/Spawn Unsynced Prefab... [VRCFury]", true)]
        [MenuItem("GameObject/BUDDYWORKS/Expression Extension/Spawn Unsynced Prefab... [VRCFury]", true)]
        [MenuItem("BUDDYWORKS/Expression Extension/Spawn Synced Prefab... [VRCFury]", true)]
        [MenuItem("GameObject/BUDDYWORKS/Expression Extension/Spawn Synced Prefab... [VRCFury]", true)]
        private static bool ValidateSpawnEE()
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