using System;
using Unity.Behavior;
using Unity.VisualScripting;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "StateMachineUnityEvent", story: "Trigger [stateMachine] UnityEvent [eventName]", category: "Action", id: "45c8fa9362f83e2a212c5f18aeb62971")]
public partial class StateMachineUnityEventAction : Action {
    [SerializeReference] public BlackboardVariable<StateMachine> stateMachine;
    [SerializeReference] public BlackboardVariable<string> eventName;

    protected override Status OnStart() {
        stateMachine.Value.TriggerUnityEvent(eventName.Value);
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

