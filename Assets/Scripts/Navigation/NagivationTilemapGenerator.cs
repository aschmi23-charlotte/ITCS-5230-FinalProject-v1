using UnityEngine;
using UnityEngine.Tilemaps;

public class NagivationTilemapGenerator : MonoBehaviour {
    [SerializeField] public Tilemap worldTilemap;
    [SerializeField] public Tilemap outputTilemap;
    [SerializeField] public NavigationTileBase pathableTile;
    [SerializeField] public NavigationTileBase obstructedTile;
    [SerializeField] public int celsPerFrame = 200;
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
