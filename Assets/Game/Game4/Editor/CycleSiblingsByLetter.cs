//using UnityEditor;
//using UnityEngine;
//using System.Collections.Generic;

//[InitializeOnLoad]
//public static class CycleSiblingsByLetter
//{
//    private static Dictionary<string, int> letterIndices = new Dictionary<string, int>();

//    static CycleSiblingsByLetter()
//    {
//        SceneView.duringSceneGui += OnSceneGUI;
//    }

//    private static void OnSceneGUI(SceneView sceneView)
//    {
//        Event e = Event.current;
//        if (e != null && e.type == EventType.KeyDown && e.character >= 'a' && e.character <= 'z')
//        {
//            string letter = e.character.ToString().ToUpper();
//            Debug.Log(letter);
//            CycleThroughSiblingsStartingWith(letter);
//            e.Use();
//        }
//    }

//    private static void CycleThroughSiblingsStartingWith(string letter)
//    {
//        GameObject[] selectedObjects = Selection.gameObjects;
//        if (selectedObjects.Length == 0)
//            return;

//        Transform parent = selectedObjects[0].transform.parent;
//        if (parent == null)
//            return;

//        var siblings = parent.GetComponentsInChildren<Transform>(true);
//        var matchingSiblings = new List<GameObject>();

//        foreach (var sibling in siblings)
//        {
//            if (sibling.parent == parent && sibling.name.StartsWith(letter, System.StringComparison.OrdinalIgnoreCase))
//            {
//                matchingSiblings.Add(sibling.gameObject);
//            }
//        }

//        if (matchingSiblings.Count == 0)
//            return;

//        if (!letterIndices.ContainsKey(letter))
//        {
//            letterIndices[letter] = 0;
//        }
//        else
//        {
//            letterIndices[letter] = (letterIndices[letter] + 1) % matchingSiblings.Count;
//        }

//        Selection.activeGameObject = matchingSiblings[letterIndices[letter]];
//    }
//}