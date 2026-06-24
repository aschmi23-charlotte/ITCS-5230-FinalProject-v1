using UnityEngine;

public class HealthPickup : ItemPickupBase {
    [Header("Health Pickup Info")]
    [SerializeField] protected float healAmount = 10f;

    public override void OnItemPickup(PlayerBrain player) {
        player.Health.RecieveHealing(healAmount);
    }
    
}