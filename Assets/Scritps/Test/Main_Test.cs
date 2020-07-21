using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Test : MonoBehaviour
{
    [SerializeField] private GameObject m_MyGameObject = null;

    private const string m_Scene = "Scene_MainMenu";
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(LoadYourAsyncScene());
        }
    }

    IEnumerator LoadYourAsyncScene()
    {
        // Set the current Scene to be able to unload it later
        Scene currentScene = SceneManager.GetActiveScene();

        // The Application loads the Scene in the background at the same time as the current Scene.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(m_Scene, LoadSceneMode.Additive);

        // Wait until the last operation fully loads to return anything
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Move the GameObject (you attach this in the Inspector) to the newly loaded Scene
        SceneManager.MoveGameObjectToScene(m_MyGameObject, SceneManager.GetSceneByName(m_Scene));
        // Unload the previous Scene
        SceneManager.UnloadSceneAsync(currentScene);

    }

    public static List<T> FindObjectsOfTypeAll<T>()
    {
        List<T> results = new List<T>();
        Debug.Log(SceneManager.sceneCount);
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var s = SceneManager.GetSceneAt(i);
            if (s.isLoaded)
            {
                var allGameObjects = s.GetRootGameObjects();
                for (int j = 0; j < allGameObjects.Length; j++)
                {
                    var go = allGameObjects[j];
                    var script = go.GetComponent<T>();
                    if(script != null)
                    {
                        results.Add(script);
                    }

                }
            }
        }
        return results;
    }
}
