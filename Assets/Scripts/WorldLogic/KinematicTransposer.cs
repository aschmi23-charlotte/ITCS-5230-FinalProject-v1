using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class KinematicTransposer : MonoBehaviour {

    public bool relativeToStart = true;

    public int targetPositionIndex;
    public List<Vector2> positions;
    public float speed;


    public Rigidbody2D RBody { get; private set; }
    public Vector2 StartPos { get; private set; }

    private Vector2 GetPosition() {
        return (Vector2) transform.position;
    }

    private Vector2 RelativeAdd(Vector2 pos) {
        return relativeToStart ? pos + StartPos : pos;
    }

    private void Awake() {
        if (positions.Count == 0) {
            positions.Append(Vector2.zero);
        }

        RBody = GetComponent<Rigidbody2D>();
        if (relativeToStart) {
            StartPos = GetPosition();
        }
    }

    // Update is called once per frame
    void FixedUpdate() {
        Vector2 currentTarget = RelativeAdd(positions[targetPositionIndex]);

        if (currentTarget == GetPosition()) {
            return;
        }

        Vector2 navVector = currentTarget - GetPosition();
        Vector2 moveDirection = (navVector).normalized;
        float remainingDistance = navVector.magnitude;

        float speedThisTick = Mathf.Min(remainingDistance, speed * Time.fixedDeltaTime);

        RBody.MovePosition(GetPosition() + moveDirection * speedThisTick);
    }
}
