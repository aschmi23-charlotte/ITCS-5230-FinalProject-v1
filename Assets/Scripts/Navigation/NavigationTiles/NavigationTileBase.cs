using UnityEngine;
using UnityEngine.Tilemaps;


[CreateAssetMenu(menuName = "Navigation/Navigation Tile")]
public class NavigationTileBase : TileBase {
    [field: SerializeField] public Sprite DrawSprite {get; private set;}
    [field: SerializeField] public bool Obstructed {get; private set;} = false;
    [field: SerializeField] public int BaseCost {get; private set;} = 1;
    
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
        tileData.sprite = DrawSprite;
    }
}