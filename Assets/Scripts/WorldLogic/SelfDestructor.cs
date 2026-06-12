using UnityEngine;

// Unity Events cannot destroy GameObjects. This makes it possible.
public class SelfDestructor : MonoBehaviour {
    public void SelfDestruct() {
        Destroy(gameObject);
    }
}
