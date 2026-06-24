using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class HudEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("Window/UI Toolkit/HudEditor")]
    public static void ShowExample()
    {
        HudEditor wnd = GetWindow<HudEditor>();
        wnd.titleContent = new GUIContent("HudEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Instantiate UXML
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);
    }
}
