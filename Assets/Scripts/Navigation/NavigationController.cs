using System.Collections.Generic;
using Priority_Queue;
using UnityEngine;

public class NavigationController : MonoBehaviour {

    public static NavigationController Instance { get; private set; }

    public NavigationTilemap NavMap {get; private set;}

    void Awake() {
        Debug.Assert(Instance == null, "Multiple instances of NavigationController present!");
        if (Instance == null) {
            Instance = this;
            Debug.Log("NavigationController instance awakened.");
        }

        NavMap = GetComponentInChildren<NavigationTilemap>();
    }

    public NavigationSearcher GetNewSearcher() {
        return new NavigationSearcher(this);
    }


}
