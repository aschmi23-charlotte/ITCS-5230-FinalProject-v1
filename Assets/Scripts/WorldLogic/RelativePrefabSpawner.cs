using System.Collections.Generic;
using UnityEngine;

public class RelativePrefabSpawner : MonoBehaviour {
    [System.Serializable]
    public class SpawnConfig {
        public GameObject prefab;
        public Vector2 offset;
        public float rotation;

        public GameObject Spawn(Transform transform) {
            // This code doesn't take into account the given transform's rotation.
            // But I don't need that behaviour right now.
           return Instantiate(prefab, transform.position + (Vector3)offset, Quaternion.Euler(0f, 0f, rotation));
        }
    }

    [field: SerializeField] public List<SpawnConfig> Configs { get; private set; }

    // Need this so UnityEvents can access this in the editor.
    public void SpawnByIndex(int index) {
        SpawnAndReturnByIndex(index);
    }

    public GameObject SpawnAndReturnByIndex(int index) {
        return Configs[index].Spawn(transform);
    }

}
