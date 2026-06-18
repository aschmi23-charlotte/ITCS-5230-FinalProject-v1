using UnityEngine;

public class ItemUnlock : MonoBehaviour {
    [SerializeField] protected PlayerBrain.PlayerUpgrades.PlayerUpgradeType upgradeType 
        = PlayerBrain.PlayerUpgrades.PlayerUpgradeType.PulseJump;

    public void OnItemPickup(PlayerBrain player) {
        player.Upgrades.TriggerUnlock(upgradeType);
    }
}