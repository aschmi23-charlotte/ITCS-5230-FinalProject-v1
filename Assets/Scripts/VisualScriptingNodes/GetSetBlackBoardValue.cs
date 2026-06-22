using UnityEngine;
using Unity.Behavior;
using Unity.VisualScripting;

namespace VisualScripting.Behavior {

    // Getter Nodes Base Class:
    public abstract class GetBlackboardValue<T> : Unit
    {
        [DoNotSerialize]
        public ValueInput agent;

        [DoNotSerialize]
        public ValueInput variableName;

        [DoNotSerialize]
        public ValueOutput value;

        protected override void Definition()
        {
            agent = ValueInput<BehaviorGraphAgent>("agent", null);
            agent.NullMeansSelf();

            variableName = ValueInput<string>("variableName");
            

            value = ValueOutput<T>("value", GetValue);
        }

        private T GetValue(Flow flow)
        {
            BehaviorGraphAgent behaviorAgent =
                flow.GetValue<BehaviorGraphAgent>(agent);

            string name =
                flow.GetValue<string>(variableName);

            if (behaviorAgent == null) {
                return default;
            }

            if (behaviorAgent.GetVariable<T>(name, out BlackboardVariable<T> variable)) {
                return variable.Value;
            }

            return default;
        }
    }


    // Setter Nodes Base Class
    public abstract class SetBlackboardValue<T> : Unit
    {
        [DoNotSerialize]
        public ControlInput enter;

        [DoNotSerialize]
        public ControlOutput exit;

        [DoNotSerialize]
        public ValueInput agent;

        [DoNotSerialize]
        public ValueInput variableName;

        [DoNotSerialize]
        public ValueInput value;

        protected override void Definition()
        {
            enter = ControlInput("enter", Execute);
            exit = ControlOutput("exit");

            agent = ValueInput<BehaviorGraphAgent>("agent");
            agent.NullMeansSelf();
            variableName = ValueInput<string>("variableName");
            value = ValueInput<T>("value");

            Succession(enter, exit);

            Requirement(agent, enter);
            Requirement(variableName, enter);
            Requirement(value, enter);
        }

        private ControlOutput Execute(Flow flow)
        {
            BehaviorGraphAgent behaviorAgent =
                flow.GetValue<BehaviorGraphAgent>(agent);

            string name =
                flow.GetValue<string>(variableName);

            T newValue =
                flow.GetValue<T>(value);

            if (behaviorAgent != null) {
                behaviorAgent.SetVariableValue(name, newValue);
            }

            return exit;
        }
    }

    // Node Definititons:
    [UnitTitle("Get Blackboard Bool")]
    [UnitCategory("Behavior/Blackboard")]
    public sealed class GetBlackboardBool : GetBlackboardValue<bool> {}

    [UnitTitle("Get Blackboard Int")]
    [UnitCategory("Behavior/Blackboard")]
    public sealed class GetBlackboardInt : GetBlackboardValue<int> {}

    [UnitTitle("Set Blackboard Int")]
    [UnitCategory("Behavior/Blackboard")]
    public sealed class SetBlackboardInt : SetBlackboardValue<int> {}

    [UnitTitle("Get Blackboard Float")]
    [UnitCategory("Behavior/Blackboard")]
    public sealed class GetBlackboardFloat : GetBlackboardValue<float> {}

    [UnitTitle("Set Blackboard Float")]
    [UnitCategory("Behavior/Blackboard")]
    public sealed class SetBlackboardFloat : SetBlackboardValue<float> {}

    [UnitTitle("Get Blackboard Vector2")]
    [UnitCategory("Behavior/Blackboard")]
    public sealed class GetBlackboardVector2 : GetBlackboardValue<Vector2> {}

    [UnitTitle("Set Blackboard Vector2")]
    [UnitCategory("Behavior/Blackboard")]
    public sealed class SetBlackboardVector2 : SetBlackboardValue<Vector2> {}
    
    [UnitTitle("Get Blackboard Vector3")]
    [UnitCategory("Behavior/Blackboard")]
    public sealed class GetBlackboardVector3 : GetBlackboardValue<Vector3> {}

    [UnitTitle("Set Blackboard Vector3")]
    [UnitCategory("Behavior/Blackboard")]
    public sealed class SetBlackboardVector3 : SetBlackboardValue<Vector3> {}

    [UnitTitle("Get Blackboard C# Object")]
    [UnitCategory("Behavior/Blackboard")]
    public sealed class GetBlackboardCSharpObject : GetBlackboardValue<object> {}

    [UnitTitle("Set Blackboard C# Object")]
    [UnitCategory("Behavior/Blackboard")]
    public sealed class SetBlackboardCSharpObject : SetBlackboardValue<object> {}
    
    [UnitTitle("Get Blackboard Unity Object")]
    [UnitCategory("Behavior/Blackboard")]
    public sealed class GetBlackboardUnityObject : GetBlackboardValue<Object> {}

    [UnitTitle("Set Blackboard Unity Object")]
    [UnitCategory("Behavior/Blackboard")]
    public sealed class SetBlackboardUnityObject : SetBlackboardValue<Object> {}

    [UnitTitle("Get Blackboard Component")]
    [UnitCategory("Behavior/Blackboard")]
    public sealed class GetBlackboardComponent : GetBlackboardValue<Component> {}

    [UnitTitle("Set Blackboard Component")]
    [UnitCategory("Behavior/Blackboard")]
    public sealed class SetBlackboardComponent : SetBlackboardValue<Component> {}

    [UnitTitle("Get Blackboard GameObject")]
    [UnitCategory("Behavior/Blackboard")]
    public sealed class GetBlackboardGameObject : GetBlackboardValue<GameObject> {}

    [UnitTitle("Set Blackboard GameObject")]
    [UnitCategory("Behavior/Blackboard")]
    public sealed class SetBlackboardGameObject : SetBlackboardValue<GameObject> {}

    [UnitTitle("Get Blackboard ScriptableObject")]
    [UnitCategory("Behavior/Blackboard")]
    public sealed class GetBlackboardScriptableObject : GetBlackboardValue<ScriptableObject> {}

    [UnitTitle("Set Blackboard GameObject")]
    [UnitCategory("Behavior/Blackboard")]
    public sealed class SetBlackboardScriptableObject : SetBlackboardValue<ScriptableObject> {}
}