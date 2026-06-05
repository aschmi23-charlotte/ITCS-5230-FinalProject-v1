using UnityEngine;
using UnityEngine.SceneManagement;

public static class AppStart {

    // On startup, we want to load the first scene in the build index,
    // since this will contain all of our global controllers, etc.

    // Executes automatically before the very first scene loads
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void OnBeforeSceneLoad() {
        Debug.Log("Engine Started!");

#if UNITY_EDITOR
        // If we're running in the editor, we need to load the global scene if it isn't already.
        // In a build, however, it will be the first scene loaded.
        bool globalSceneLoaded = false;
        for (int i = 0; i < SceneManager.sceneCount; i++) {
            Scene currentScene = SceneManager.GetSceneAt(i);
            if (currentScene.buildIndex == 0) {
                Debug.Log("Global Scene Already Loaded");
                globalSceneLoaded = true;
            }
        }

        if (!globalSceneLoaded) {
            Debug.Log("Loading Global Scene");
            SceneManager.LoadScene(0, LoadSceneMode.Additive);
        }
#endif

    }
}
