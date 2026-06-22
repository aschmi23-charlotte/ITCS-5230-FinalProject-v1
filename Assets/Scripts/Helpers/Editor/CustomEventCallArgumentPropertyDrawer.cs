using UnityEditor;
using UnityEngine;

// IngredientDrawerUIE
[CustomPropertyDrawer(typeof(CustomEventCall.Argument))]
public class CustomEventCallArgumentProperyDrawer : PropertyDrawer {
    // Needs to match the order of CustomEventCall.Argument.ArgumentType.
    
    public static float typeDropdownWidthPercent = 0.2f;
    public static float typeDropdownWidthMax = 90f;
    public static int argPropertyFirstIndex = 1;
    public static string[] argPropertyNames = {
        "boolArg",
        "intArg",
        "floatArg",
        "stringArg",
        "vector2Arg",
        "vector3Arg",
        "gameObjectArg",
        "gameObjectComponentArg",
        "unityEngineObjectArg",
        "visualScriptingVariableName",
    };

    // IMGUI Implementation:
    // Draw the property inside the given rect
    public override void OnGUI(Rect parentPosition, SerializedProperty property, GUIContent label) {
        // Using BeginProperty / EndProperty on the parent property means that

        EditorGUI.BeginProperty(parentPosition, label, property);

        // Draw label
        Rect position = EditorGUI.PrefixLabel(parentPosition, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        SerializedProperty argumentTypeProperty = property.FindPropertyRelative("argumentType");
        
        // Drawing editor based on type:
        int argTypeIndex = argumentTypeProperty.enumValueIndex;
        // Draw the property, IF it's on that we CAN draw an editor for.
        if (argPropertyFirstIndex <= argTypeIndex && argTypeIndex <= argPropertyNames.Length - 1 + argPropertyFirstIndex) {
            // Draw Type
            float typeSelectorWidth = Mathf.Max(position.width * typeDropdownWidthPercent, typeDropdownWidthMax);
            Rect argumentTypeRect = new Rect(position.x, position.y, typeSelectorWidth, position.height);
            EditorGUI.PropertyField(argumentTypeRect, argumentTypeProperty, GUIContent.none);

            // Draw value:
            SerializedProperty argumentValueProperty = property.FindPropertyRelative(argPropertyNames[argTypeIndex - argPropertyFirstIndex]);
            Rect argumentValueRect = new Rect(position.x + typeSelectorWidth, position.y, position.width - typeSelectorWidth, position.height);
            EditorGUI.PropertyField(argumentValueRect, argumentValueProperty, GUIContent.none);
        } else {
            // Just draw the type selection.
            Rect argumentTypeRect = new Rect(position.x, position.y, position.width, position.height);
            EditorGUI.PropertyField(argumentTypeRect, argumentTypeProperty, GUIContent.none);
        }


        // Set indent back to what it was
        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }
}
