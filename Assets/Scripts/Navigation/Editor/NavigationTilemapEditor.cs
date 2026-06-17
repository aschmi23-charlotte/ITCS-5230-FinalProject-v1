using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.Tilemaps;
using Unity.EditorCoroutines.Editor;

[CustomEditor(typeof(NavigationTilemap))]
[CanEditMultipleObjects]
public class NavigationTilemapEditor : Editor {
    
    public NavigationTilemap TargetNav {get { return (NavigationTilemap) target;}}

    void OnEnable() {
        if (TargetNav.autoToggleRenderer) {
            TargetNav.GetComponent<TilemapRenderer>().enabled = true;    
        }
    }

    void OnDisable() {
        if (TargetNav.autoToggleRenderer) {
            TargetNav.GetComponent<TilemapRenderer>().enabled = false;    
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }
}