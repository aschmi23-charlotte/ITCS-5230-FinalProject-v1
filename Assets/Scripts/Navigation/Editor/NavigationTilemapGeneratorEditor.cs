using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.Tilemaps;
using Unity.EditorCoroutines.Editor;

[CustomEditor(typeof(NagivationTilemapGenerator))]
[CanEditMultipleObjects]
public class NavigationTilemapGeneratorEditor : Editor {
    

    public NagivationTilemapGenerator TargetGenerator {get { return (NagivationTilemapGenerator) target;}}

    void OnEnable() {
        if (TargetGenerator.autoToggleRenderer) {
            TargetGenerator.GetComponent<TilemapRenderer>().enabled = true;    
        }
    }

    void OnDisable() {
        if (TargetGenerator.autoToggleRenderer) {
            TargetGenerator.GetComponent<TilemapRenderer>().enabled = false;    
        }
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        if (GUILayout.Button("Generate")) {
            EditorCoroutineUtility.StartCoroutine(GenerateNavigationTilemap(), this);
        }
    }

    IEnumerator GenerateNavigationTilemap() {
        Debug.Log("Started Navigation Tilemap Generation");

        Tilemap manualMap = TargetGenerator.GetComponent<Tilemap>();
        int cellCount = 0;

        foreach (Vector3Int position in TargetGenerator.worldTilemap.cellBounds.allPositionsWithin) {
            // Call at the top. That way, we don't need a bunch of these later.
            cellCount++;
            if (cellCount == TargetGenerator.celsPerFrame) {
                cellCount = 0;
                yield return null;
            }
            
            NavigationTileBase manualTile = manualMap.GetTile<NavigationTileBase>(position);
            if (manualTile != null) {
                TargetGenerator.outputTilemap.SetTile(position, manualTile);
            }

            else if (TargetGenerator.worldTilemap.GetTile(position) != null) {
                TargetGenerator.outputTilemap.SetTile(position, TargetGenerator.obstructedTile);
            }

            else {
                TargetGenerator.outputTilemap.SetTile(position, TargetGenerator.pathableTile);
            }

        }
        Debug.Log("Finished Navigation Tilemap Generation");
    }

}