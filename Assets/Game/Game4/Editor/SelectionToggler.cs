//using UnityEditor;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//[CreateAssetMenu(fileName = "ObjectList", menuName = "ScriptableObjects/ObjectList", order = 1)]
//public class ObjectList : ScriptableObject
//{
//    public string[] ObjectNames;
//}

//public class SelectionToggler : EditorWindow
//{
//    private static ObjectList objectList;
//    private static int currentIndex = 0;

//    [MenuItem("Tools/Toggle Selection %tab")] // Ctrl (Cmd on macOS) + Tab
//    private static void ToggleSelection()
//    {
//        EnsureObjectListExists();
//        if (objectList == null || objectList.ObjectNames == null || objectList.ObjectNames.Length == 0)
//        {
//            Debug.LogWarning("No objects specified for toggling. Make sure the ObjectList is assigned and contains objects.");
//            return;
//        }

//        // Increment the index to select the next object.
//        currentIndex = (currentIndex + 1) % objectList.ObjectNames.Length;

//        // Find the GameObject by name.
//        GameObject obj = GameObject.Find(objectList.ObjectNames[currentIndex]);

//        if (obj == null)
//        {
//            Debug.LogWarning($"GameObject '{objectList.ObjectNames[currentIndex]}' not found in the scene.");
//            return;
//        }

//        // Select the found object in the Hierarchy.
//        Selection.activeGameObject = obj;

//        // Ping the object in the Hierarchy to highlight it.
//        EditorGUIUtility.PingObject(obj);
//    }

//    private static void EnsureObjectListExists()
//    {
//        string sceneName = SceneManager.GetActiveScene().name;
//        string assetPath = $"Assets/Editor/Assets/{sceneName}.asset";

//        objectList = AssetDatabase.LoadAssetAtPath<ObjectList>(assetPath);
//        if (objectList == null)
//        {
//            objectList = ScriptableObject.CreateInstance<ObjectList>();

//            // Ensure the directory exists
//            System.IO.Directory.CreateDirectory("Assets/Editor/Assets");

//            AssetDatabase.CreateAsset(objectList, assetPath);
//            AssetDatabase.SaveAssets();
//            Debug.Log($"ObjectList asset was not found and has been created at {assetPath}.");
//        }
//    }

//    [MenuItem("Tools/Assign Object List")]
//    private static void AssignObjectList()
//    {
//        EnsureObjectListExists();
//        Debug.Log("ObjectList has been assigned or created.");
//    }

//    [MenuItem("GameObject/Add to Object List", false, 10)]
//    private static void AddToObjectList(MenuCommand command)
//    {
//        EnsureObjectListExists();

//        var selectedObjects = Selection.gameObjects;
//        if (selectedObjects.Length == 0)
//        {
//            Debug.LogWarning("No GameObjects selected.");
//            return;
//        }

//        var names = new System.Collections.Generic.List<string>(objectList.ObjectNames ?? new string[0]);
//        foreach (var obj in selectedObjects)
//        {
//            if (!names.Contains(obj.name))
//            {
//                names.Add(obj.name);
//                Debug.Log($"Added '{obj.name}' to the Object List.");
//            }
//            else
//            {
//                Debug.LogWarning($"'{obj.name}' is already in the Object List.");
//            }
//        }

//        objectList.ObjectNames = names.ToArray();
//        EditorUtility.SetDirty(objectList);
//    }

//    [MenuItem("GameObject/Remove from Object List", false, 10)]
//    private static void RemoveFromObjectList(MenuCommand command)
//    {
//        EnsureObjectListExists();

//        var selectedObjects = Selection.gameObjects;
//        if (selectedObjects.Length == 0)
//        {
//            Debug.LogWarning("No GameObjects selected.");
//            return;
//        }

//        var names = new System.Collections.Generic.List<string>(objectList.ObjectNames ?? new string[0]);
//        foreach (var obj in selectedObjects)
//        {
//            if (names.Contains(obj.name))
//            {
//                names.Remove(obj.name);
//                Debug.Log($"Removed '{obj.name}' from the Object List.");
//            }
//            else
//            {
//                Debug.LogWarning($"'{obj.name}' is not in the Object List.");
//            }
//        }

//        objectList.ObjectNames = names.ToArray();
//        EditorUtility.SetDirty(objectList);
//    }
//}
