using System.Collections;
using Udar.SceneManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalController : MonoBehaviour {
    public static GlobalController Instance { get; private set; }

    // Editor Fields:
    [Header("Runtime Info")]
    public SpawnReference startingSpawn;
    public GameObject mainCamera;

    [Header("Prefabs")]
    public GameObject playerPrefab;

#if UNITY_EDITOR
    [Header("Editor Control")]
    public int spawnPointIdForMapScene = 0;
#endif

    // Attributes
    public Scene CurrentMapScene { get; protected set; }
    public GameObject Player { get; protected set; }

    void Start() {
        Debug.Assert(Instance == null, "Multiple instances of GlobalController present!");
        if (Instance == null) {
            Instance = this;
            Debug.Log("GlobalController instance awakened.");
        }

#if UNITY_EDITOR
        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.buildIndex != 0) {
            Debug.Log("A Map Scene is already loaded.");
            CurrentMapScene = activeScene;
            InitPlayerInCurrentScene(spawnPointIdForMapScene);
        } else {
            InitWithMapScene(startingSpawn);
        }
#else
        InitWithMapScene(startingSpawn);
#endif
    }

    void InitWithMapScene(SpawnReference spawn) {
        StartCoroutine(InitWithMapSceneCoroutine(spawn));
    }

    IEnumerator InitWithMapSceneCoroutine(SpawnReference spawn) {
        AsyncOperation load = SceneManager.LoadSceneAsync(spawn.MapScene.BuildIndex, LoadSceneMode.Additive);
        yield return load;
        yield return null;
        InitPlayerInCurrentScene(spawn.SpawnPointId);
    }

    public void InitPlayerInCurrentScene(int spawnPointId) {
        Player = Instantiate(playerPrefab);

        PlayerSpawnPoint[] spawnPoints = FindObjectsByType<PlayerSpawnPoint>();
        Debug.Assert(spawnPoints.Length > 0, "At least one PlayerSpawnPoint exists");

        foreach (PlayerSpawnPoint spawnPoint in spawnPoints) {
            if (spawnPoint.spawnPointId == spawnPointId) {
                Player.transform.position = spawnPoint.transform.position;
                return;
            }
        }

        Debug.LogErrorFormat("No PlayerSpawnPoint with Id {0} discovered.", spawnPointId);
    }
    /*
    public void PerformMapSceneTransition(SpawnReference spawn) {
        StartCoroutine(MapSceneTransitionCoroutine(spawn));
    }

    IEnumerator MapSceneTransitionCoroutine(SpawnReference spawn) {
        AsyncOperation unload = SceneManager.UnloadSceneAsync(CurrentMapScene);

        yield return unload;
        yield return null;

        AsyncOperation load = SceneManager.LoadSceneAsync(spawn.MapScene.BuildIndex, LoadSceneMode.Additive);
        yield return load;
        yield return null;
    }
    */
}
