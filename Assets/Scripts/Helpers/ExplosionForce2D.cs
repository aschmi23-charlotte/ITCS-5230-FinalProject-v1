using UnityEngine;

public static class ExplosionForce2D {

    public static Vector2 CalculateExplosionForce(Vector2 pos, Vector2 blastPos, float blastRadius, float blastForce) {
        Vector2 compVector = pos - blastPos;
        Vector2 blastDirection = compVector.normalized;
        float blastDist = compVector.magnitude;

        if (blastDist > blastRadius) {
            // Out of range. No effect.
            return Vector2.zero;
        }

        float forcePercent = 1f - (blastDist / blastRadius);
        Vector2 retVal = blastDirection * blastForce * forcePercent;
        return retVal;
    }

    public static void AddExplosionForce(Rigidbody2D body, Vector2 blastPos, float blastRadius, float blastForce) {
        body.AddForce(CalculateExplosionForce(body.worldCenterOfMass, blastPos, blastRadius, blastForce * 100f));
    }

    public static void PerformExplosion(Vector2 blastPos, float blastRadius, float blastForce, LayerMask layerMask) {
        Collider2D[] hits = Physics2D.OverlapCircleAll(blastPos, blastRadius, layerMask);

        foreach (Collider2D hit in hits) {
            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
            if (rb != null) {
                AddExplosionForce(rb, blastPos, blastRadius, blastForce);
            }
        }
    }
    
}
