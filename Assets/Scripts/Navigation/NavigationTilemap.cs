using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class NavigationTilemap : MonoBehaviour {

    [SerializeField] public bool autoToggleRenderer = true;
    
    public Tilemap Map {get; private set;}
    public TilemapRenderer Renderer {get; private set;}

    void Awake() {
        Map = GetComponent<Tilemap>();
        Renderer = GetComponent<TilemapRenderer>();
    }

    void Start() {
        Renderer.enabled = false;
    }
}
