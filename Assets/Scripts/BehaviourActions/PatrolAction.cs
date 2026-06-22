using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "PatrolAction",
    story: "Update [outMoveTarget] and [outSpeedPercent] so that [gameObject] follows the path [patrolPath] with a speed of [patrolSpeedPercent] starting at node [firstNodeIndex]",
    category: "Action",
    id: "45458bec77ae939313e6e51c32994c91"
)]
public partial class PatrolAction : Action {
    // This action doesn't actually move the agent. The state machine needs to handle that.
    // This action determines how the agent WANTS to move.
    [SerializeReference] public BlackboardVariable<GameObject> gameObject;
    [SerializeReference] public BlackboardVariable<PatrolPath> patrolPath;
    // [SerializeReference] public BlackboardVariable<float> patrolSpeedPercent;
    [SerializeReference] public BlackboardVariable<float> patrolSpeedPercent;
    [SerializeReference] public BlackboardVariable<int> firstNodeIndex;

    [SerializeReference] public BlackboardVariable<Vector2> outMoveTarget;
    [SerializeReference] public BlackboardVariable<float> outSpeedPercent;

    // public enum PatrolMode {
    //     Stop,
    //     Move    
    // }

    // private PatrolMode patrolMode = PatrolMode.Move;
    private int targetNodeIndex = 0;
    private float stopTimer = 0f;
    private bool runBackwards = false;

    protected override Status OnStart() {
        targetNodeIndex = 0;

        return Status.Running;
    }

    protected override Status OnUpdate() {
        PatrolPathNode node = GetTargetPathNode();

        // // Stop and wait.
        if ((gameObject.Value.transform.position - node.transform.position).magnitude < node.targetRadius) {
            // patrolMode = PatrolMode.Stop;
            // stopTimer += Time.deltaTime;

            // Time to move onto a new target.
            // if (stopTimer > node.waitTime) {
            if (true) {
                // At the beginning again. Turning around.
                if (runBackwards && targetNodeIndex == 0) {
                    runBackwards = !runBackwards;
                    targetNodeIndex += runBackwards ? -1 : 1;
                    node = GetTargetPathNode();
                }

                // Reached the end of the path;
                else if (targetNodeIndex == patrolPath.Value.nodes.Count - 1) {
                    switch (patrolPath.Value.followMode) {
                        case PatrolPath.FollowMode.Loop:
                        default:
                            targetNodeIndex = 0;
                            break;
                        case PatrolPath.FollowMode.PingPong:
                            runBackwards = !runBackwards;
                            targetNodeIndex += runBackwards ? -1 : 1;
                            break;
                        case PatrolPath.FollowMode.OneWay:
                            return Status.Success;
                    }
                    
                    stopTimer = 0;
                } else {
                    targetNodeIndex += runBackwards ? -1 : 1;
                }

                node = GetTargetPathNode();
            }
        }
        outMoveTarget.Value = node.transform.position;
        outSpeedPercent.Value = patrolSpeedPercent.Value;
        return Status.Running;
    }

    protected override void OnEnd() {
    }

    protected PatrolPathNode GetTargetPathNode() {
        return GetPathNode(targetNodeIndex);
    }

    protected PatrolPathNode GetPathNode(int index) {
        return patrolPath.Value.nodes[index];
    }
}

