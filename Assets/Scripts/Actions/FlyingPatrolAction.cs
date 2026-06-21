using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FlyingPatrolAction", story: "Agent patrols the path [given]", category: "Action", id: "45458bec77ae939313e6e51c32994c91")]
public partial class FlyingPatrolAction : Action
{
    [SerializeReference] public BlackboardVariable<PatrolPath> Given;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate() {
        return Status.Success;
    }

    protected override void OnEnd() {
    }
}

