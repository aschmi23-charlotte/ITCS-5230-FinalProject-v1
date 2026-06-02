using UnityEngine;
using UnityEngine.SceneManagement;

public static class AppStart {

    // On startup, we want to load the first scene in the build index,
    // since this will contain all of our global controllers, etc.

    // Executes automatically before the very first scene loads
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void OnBeforeSceneLoad() {
        Debug.Log("Engine Started!");

        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.buildIndex != 0) {
            Debug.Log("Loading Global Scene");
            SceneManager.LoadScene(0, LoadSceneMode.Additive);
        }
        
    }
}
