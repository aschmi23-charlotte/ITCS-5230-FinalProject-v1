using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineCamera))]
public class PlayerFollowCamAssigner : MonoBehaviour {
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        StartCoroutine(WaitForPlayer());
    }

    IEnumerator WaitForPlayer() {
        while(GlobalController.Instance == null || GlobalController.Instance.Player == null) {
            yield return null;
        }

        CinemachineCamera followCam = GetComponent<CinemachineCamera>();
        Debug.Assert(followCam != null, "PlayerFollowCamAssigner is attached to a CinemachineCamera");

        followCam.Target.TrackingTarget = GlobalController.Instance.Player.transform;
        
        Debug.Log("PlayerFollowCamAssigner finished.");
    }
}
