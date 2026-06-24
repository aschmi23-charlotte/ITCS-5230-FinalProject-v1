using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour {
    
    [System.Serializable]
    public class SpawnEntry {
        public GameObject prefab = null;
        public Vector2 offset = Vector2.zero;
        public float chance = 1f;
    }

    [SerializeField] protected List<SpawnEntry> spawnEntries;

    public void SpawnByIndex(int prefabIndex) {
        if (Random.Range(0f, 1f) < spawnEntries[prefabIndex].chance) {
            Instantiate(spawnEntries[prefabIndex].prefab, (Vector2)transform.position + spawnEntries[prefabIndex].offset, Quaternion.Euler(0, 0, 0));
        }
    }
}