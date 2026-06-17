using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class NavigationTilemap : MonoBehaviour {

    [SerializeField] public bool autoToggleRenderer = true;
    
    public Tilemap NavMap {get; private set;}
    public TilemapRenderer NavRenderer {get; private set;}

    void Awake() {
        NavMap = GetComponent<Tilemap>();
        NavRenderer = GetComponent<TilemapRenderer>();
    }

    void Start() {
        NavRenderer.enabled = false;
    }
}
