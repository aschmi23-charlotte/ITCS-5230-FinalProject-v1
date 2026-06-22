using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "PatrolAction",
    story: "[agentBrain] wants to follow the path [patrolPath] starting at node [firstNodeIndex]",
    category: "Action",
    id: "45458bec77ae939313e6e51c32994c91"
)]
public partial class PatrolAction : Action {
    // This action doesn't actually move the agent. The state machine needs to handle that.
    // This action determines how the agent WANTS to move.
    [SerializeReference] public BlackboardVariable<PatrolPath> patrolPath;
    [SerializeReference] public BlackboardVariable<MovementAgentBrain> agentBrain;
    [SerializeReference] public BlackboardVariable<int> firstNodeIndex;

    private int targetNodeIndex = 0;
    private float stopTimer = 0f;
    private bool runBackwards = false;

    protected override Status OnStart() {
        targetNodeIndex = 0;
        PatrolPathNode node = GetTargetPathNode();

        agentBrain.Value.Destination = node.transform.position;
        agentBrain.Value.Desire = MovementAgentBrain.AgentDesire.Move;

        return Status.Running;
    }

    protected override Status OnUpdate() {
        PatrolPathNode node = GetTargetPathNode();

        // Stop and wait.
        if ((agentBrain.Value.transform.position - node.transform.position).magnitude < node.targetRadius) {
            agentBrain.Value.Desire = MovementAgentBrain.AgentDesire.Move;
            stopTimer = Time.deltaTime;

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
                }
            }
        }

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

