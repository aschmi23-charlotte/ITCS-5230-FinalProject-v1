using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ChaseHostile", story: "[agent] moves with a speed of [chaseSpeedPercent] to be within [successDistance] units of target.", category: "Action", id: "21413d0f6f6ae1bfc1b30e9e8334eb2f")]
public partial class ChaseHostileAction : Action
{
    [SerializeReference] public BlackboardVariable<HostileDetector> agent;
    [SerializeReference] public BlackboardVariable<float> successDistance;
    [SerializeReference] public BlackboardVariable<float> chaseSpeedPercent;

    [SerializeReference] public BlackboardVariable<bool> outWantsToMove;
    [SerializeReference] public BlackboardVariable<Vector2> outMoveTarget;
    [SerializeReference] public BlackboardVariable<float> outSpeedPercent;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate() {
        // Reached the last place the target was detected.
        if (Vector2.Distance((Vector2)agent.Value.transform.position, agent.Value.TargetLastKnownLocation) <= successDistance) {

            // The last known location is updated as long as the target is detected.
            // If we've arrives and can't detect the target, then we've lost the target.
            if (agent.Value.CanStillDetectTarget()) {
                outWantsToMove.Value = false;
                return Status.Success;
            } else {
                outWantsToMove.Value = false;
                agent.Value.ClearTarget();
                return Status.Failure;
            }
        }

        outWantsToMove.Value = true;
        outMoveTarget.Value = agent.Value.TargetLastKnownLocation;
        outSpeedPercent.Value = chaseSpeedPercent.Value;

        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

