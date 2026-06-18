using UnityEngine;

public class ItemUnlock : ItemPickupBase {

    [Header("Upgrade Info")]
    [SerializeField] protected PlayerBrain.PlayerUpgrades.PlayerUpgradeType upgradeType 
        = PlayerBrain.PlayerUpgrades.PlayerUpgradeType.PulseJump;

    public override void OnItemPickup(PlayerBrain player) {
        player.Upgrades.TriggerUnlock(upgradeType);
    }
}