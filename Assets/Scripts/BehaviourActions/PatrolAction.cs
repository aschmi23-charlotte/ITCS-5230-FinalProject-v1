using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "PatrolAction",
    story: "[gameObject] follows the path [patrolPath] with a speed of [patrolSpeedPercent] starting at node [firstNodeIndex].",
    category: "Action",
    id: "45458bec77ae939313e6e51c32994c91"
)]
public partial class PatrolAction : Action {
    // This action doesn't actually move the agent. The state machine needs to handle that.
    // This action determines how the agent WANTS to move.
    [SerializeReference] public BlackboardVariable<GameObject> gameObject;
    [SerializeReference] public BlackboardVariable<PatrolPath> patrolPath;
    [SerializeReference] public BlackboardVariable<float> patrolSpeedPercent;
    [SerializeReference] public BlackboardVariable<int> firstNodeIndex;

    [SerializeReference] public BlackboardVariable<bool> outWantsToMove;
    [SerializeReference] public BlackboardVariable<Vector2> outMoveTarget;
    [SerializeReference] public BlackboardVariable<float> outSpeedPercent;

    private int targetNodeIndex = 0;
    private float stopTimer = 0f;
    private bool runBackwards = false;

    protected override Status OnStart() {
        targetNodeIndex = firstNodeIndex.Value;
        return Status.Running;
    }

    protected override Status OnUpdate() {
        PatrolPathNode node = GetPathNode(targetNodeIndex);

        // // Stop and wait.
        if ((gameObject.Value.transform.position - node.transform.position).magnitude < node.targetRadius) {
            outWantsToMove.Value = false;
            stopTimer += Time.deltaTime;

            // Time to move onto a new target.
            if (stopTimer > node.waitTime) {
                // At the beginning again. Turning around.
                if (runBackwards && targetNodeIndex == 0) {
                    runBackwards = !runBackwards;
                    targetNodeIndex += runBackwards ? -1 : 1;
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

                node = GetPathNode(targetNodeIndex);
            } 
        }
        else {
            outWantsToMove.Value = true;
            stopTimer = 0;
        }

        outMoveTarget.Value = node.transform.position;
        outSpeedPercent.Value = patrolSpeedPercent.Value;
        return Status.Running;
    }

    protected override void OnEnd() {
    }

    protected PatrolPathNode GetPathNode(int index) {
        return patrolPath.Value.nodes[index];
    }
}

