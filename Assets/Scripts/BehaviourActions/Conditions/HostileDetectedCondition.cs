using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "HostileDetected", story: "[agent] has an enemy targeted", category: "Conditions", id: "ffb264500b6a5dac2ba774bb4611baad")]
public partial class HostileDetectedCondition : Condition
{
    [SerializeReference] public BlackboardVariable<HostileDetector> agent;

    public override bool IsTrue()
    {
        return agent.Value.HasTarget();
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
