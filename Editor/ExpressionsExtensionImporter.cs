using UnityEngine;
using UnityEditor;

namespace Buddyworks.ExpressionsExtension
{  
    public class PrefabSpawner : MonoBehaviour
    {
        // VRCF Prefab definitions
        static string prefabEE_UnSynced_VRCF = "fb61f054361106040888ef1bb33b44ad";
        static string prefabEE_Synced_VRCF = "9c1b692672e052d44a475587f7d535fd";
        static string prefabEE_UnSynced_MA = "f1456f8d0c8efe2439dbe13581b42857";
        static string prefabEE_Synced_MA = "0742dd64b1be7054fb968129df31ad41";
        static string VRCF_Path = "Packages/com.vrcfury.vrcfury";
        static string MA_Path = "Packages/nadena.dev.modular-avatar";

        // Toolbar Menu - VRCF
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

        // Toolbar Menu - MA
        [MenuItem("BUDDYWORKS/Expression Extension/Spawn Unsynced Prefab... [ModularAvatar]", false, 2)]
        [MenuItem("GameObject/BUDDYWORKS/Expression Extension/Spawn Unsynced Prefab... [ModularAvatar]", false, 1)]
        private static void SpawnEE_Unsynced_MA()
        {
            SpawnPrefab(prefabEE_UnSynced_MA);
        }

        [MenuItem("BUDDYWORKS/Expression Extension/Spawn Synced Prefab... [ModularAvatar]", false, 3)]
        [MenuItem("GameObject/BUDDYWORKS/Expression Extension/Spawn Synced Prefab... [ModularAvatar]", false, 1)]
        private static void SpawnEE_Synced_MA()
        {
            SpawnPrefab(prefabEE_Synced_MA);
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
            return AssetDatabase.IsValidFolder(VRCF_Path);
        }

        [MenuItem("BUDDYWORKS/Expression Extension/Spawn Unsynced Prefab... [ModularAvatar]", true)]
        [MenuItem("GameObject/BUDDYWORKS/Expression Extension/Spawn Unsynced Prefab... [ModularAvatar]", true)]
        [MenuItem("BUDDYWORKS/Expression Extension/Spawn Synced Prefab... [ModularAvatar]", true)]
        [MenuItem("GameObject/BUDDYWORKS/Expression Extension/Spawn Synced Prefab... [ModularAvatar]", true)]
        private static bool ValidateSpawnEE_MA()
        {
            return AssetDatabase.IsValidFolder(MA_Path);
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

            if (!prefab)
            {
                Debug.LogError("Failed to load prefab with GUID " + guid + " at path " + prefabPath);
                return;
            }

            GameObject instantiatedPrefab = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

            if (selectedObject)
            {
                instantiatedPrefab.transform.parent = selectedObject.transform;
            }

            if (instantiatedPrefab)
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