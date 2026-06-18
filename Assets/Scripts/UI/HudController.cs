using UnityEngine;
using UnityEngine.UIElements;

public class HudController : MonoBehaviour {
    
    [System.Serializable]
    public class HudColors {
        public Color healthColor;
        public Color ammoColor;
    }

    [SerializeField] protected HudColors hudColors;

    public UIDocument Document {get; private set;}
    public Label HealthNumber {get; private set;}
    public ProgressBar HealthBar {get; private set;}
    public Label AmmoCountNumber {get; private set;}
    public ProgressBar AmmoCountBar {get; private set;}
    public Image WeaponIcon {get; private set;}
    
    void Awake() {
        Document = GetComponent<UIDocument>();

    }

    void Start() {
        ReloadUIElements();
    }

    void Update() {
        // Only show the UI when the player is around.
        if (Document.enabled && PlayerBrain.Instance == null) {
            Document.enabled = false;
        } else if (!Document.enabled && PlayerBrain.Instance != null) {
            Document.enabled = true;
            ReloadUIElements();
        }

        // Quit if the Player isn't here.
        if (PlayerBrain.Instance == null) { return; }
        HealthNumber.text = PlayerBrain.Instance.Health.CurrentHealth.ToString();
        HealthBar.value = PlayerBrain.Instance.Health.HealthPercent;
        AmmoCountNumber.text = PlayerBrain.Instance.Weapons.WeaponAmmoCount.ToString();
        AmmoCountBar.value = PlayerBrain.Instance.Weapons.WeaponAmmoPercent;

    }

    private void ConfigureInnerProgressBar(ProgressBar bar, Color color) {
        VisualElement innerBar = bar.Q<VisualElement>(className: "unity-progress-bar__progress");
        innerBar.style.backgroundColor = color;
    }

    private void ReloadUIElements() {
        HealthNumber = Document.rootVisualElement.Q<Label>("HealthNumber");
        HealthBar = Document.rootVisualElement.Q<ProgressBar>("HealthBar");
        AmmoCountNumber = Document.rootVisualElement.Q<Label>("AmmoCountNumber");
        AmmoCountBar = Document.rootVisualElement.Q<ProgressBar>("AmmoCountBar");
        WeaponIcon = Document.rootVisualElement.Q<Image>("WeaponIcon");
        
        ConfigureInnerProgressBar(HealthBar, hudColors.healthColor);
        ConfigureInnerProgressBar(AmmoCountBar, hudColors.ammoColor);
    }

}