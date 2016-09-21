using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.IO;

public class GeneratorOverseer : EditorWindow
{
    private bool _generated = false;
    private bool _closeAfterSaving = false;
    public static bool stopPlayingAfterGeneration = false; 

    [MenuItem("Window/GeneratorOverseer")]
    static void Init()
    {
        GeneratorOverseer window = GetWindow<GeneratorOverseer>();
        window.Show();
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Generated", _generated ? "YES" : "NO");
        GUILayout.Label("Close after saving scene", EditorStyles.boldLabel);
        _closeAfterSaving = EditorGUILayout.Toggle("Enable:", _closeAfterSaving);

        GUILayout.Label("Stop playmode after generation", EditorStyles.boldLabel);
        stopPlayingAfterGeneration = EditorGUILayout.Toggle("Enable:", stopPlayingAfterGeneration);

        if (EditorApplication.isPlaying == true)
        {
            _generated = true;
        }

        if (_generated && EditorApplication.isPlaying == false)
        {
            SaveScene();
            _generated = false;
            EditorApplication.isPlaying = false;

            if (_closeAfterSaving)
            {
                EditorApplication.Exit(0);
            }
        }
    }

    private void SaveScene()
    {
        int index = GetGighestScenePrefabIndex();
        string[] possiblePrefabs = AssetDatabase.FindAssets(string.Format("ScenePrefab#{0}", index), new string[]{ "Assets/Scenes" });
        string prefabPath = AssetDatabase.GUIDToAssetPath(possiblePrefabs[0]);
        GameObject scenePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        foreach (GameObject o in Object.FindObjectsOfType<GameObject>())
        {
            DestroyImmediate(o);
        }

        GameObject prefabInstance =  Instantiate(scenePrefab, Vector3.zero, Quaternion.identity) as GameObject;
       
        string sceneName = string.Format("Assets/Scenes/Scene#{0}.unity", index);
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), sceneName, true);        
        EditorSceneManager.OpenScene(sceneName);        
        NavMeshBuilder.BuildNavMesh();       
        DestroyImmediate(prefabInstance);
       // EditorSceneManager.OpenScene("Assets/_Scenes/main.unity");
    }

    private int GetGighestScenePrefabIndex()
    {
        string path = string.Format("{0}//{1}", Application.dataPath, "Scenes");
        string[] scenePrefabs = Directory.GetFiles(path, "*.prefab");
        int highestIndex = 0;
        foreach (var scene in scenePrefabs)
        {
            string fileName = Path.GetFileNameWithoutExtension(scene);
            int index = 0;
            int.TryParse(fileName.Split('#')[1], out index);

            if (index > highestIndex)
            {
                highestIndex = index;
            }
        }

        return highestIndex;
    }

    //void CloaseAfterGeneration()
    //{
    //    if (_generated && EditorApplication.isPlaying == false)
    //    {
    //        EditorApplication.Exit(0);
    //    }
    //}

}
