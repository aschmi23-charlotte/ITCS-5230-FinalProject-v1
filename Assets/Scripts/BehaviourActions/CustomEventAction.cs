using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CustomEventAction", story: "Trigger VisualScripting CustomEvent [caller] [callerIndex]", category: "Action", id: "d027541c61a8270b23b8f82391bd9991")]
public partial class CustomEventAction : Action
{
    [SerializeReference] public BlackboardVariable<VisualScriptingCustomEventCaller> caller;
    [SerializeReference] public BlackboardVariable<int> callerIndex;

    protected override Status OnStart() {
        caller.Value.TriggerByIndex(callerIndex.Value);

        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

