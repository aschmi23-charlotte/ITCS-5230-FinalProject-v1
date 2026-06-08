using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour {

    public int spawnPointId = 0;

#if UNITY_EDITOR
    void OnDrawGizmos() {
        Gizmos.color = new Color(0f, 1f, 0f, 1f);

        // Convert the local coordinate values into world
        // coordinates for the matrix transformation.
        //Gizmos.matrix = transform.localToWorldMatrix;

        Vector3 topPos = transform.position + Vector3.up * 2f;
        Gizmos.DrawLine(transform.position, topPos);
        UnityEditor.Handles.Label(topPos + Vector3.right * 0.5f, "Player Spawn Point " + spawnPointId);
    }
#endif
}
