using UnityEngine;

public static class ExplosionForce2D {


    // Explosion Force
    public static Vector2 CalculateLinearExplosionForce(Vector2 pos, Vector2 blastPos, float blastRadius, float blastForce) {
        return CalculateCurvedExplosionForce(pos, blastPos, blastRadius, blastForce, AnimationCurve.Linear(0f, 0f, 1f, 1f));
    }

    public static Vector2 CalculateCurvedExplosionForce(Vector2 pos, Vector2 blastPos, float blastRadius, float blastForce, AnimationCurve blastCurve) {
        Vector2 compVector = pos - blastPos;
        Vector2 blastDirection = compVector.normalized;
        float blastDist = compVector.magnitude;

        if (blastDist > blastRadius) {
            // Out of range. No effect.
            return Vector2.zero;
        }

        float forcePercent = blastCurve.Evaluate(1f - (blastDist / blastRadius));
        Vector2 retVal = blastDirection * blastForce * forcePercent;
        return retVal;
    }


    public static void AddLinearExplosionForce(Rigidbody2D body, Vector2 blastPos, float blastRadius, float blastForce, ForceMode2D forceMode = ForceMode2D.Impulse) {
        AddCurvedExplosionForce(body, blastPos, blastRadius, blastForce, AnimationCurve.Linear(0f, 0f, 1f, 1f), forceMode);
    }

    public static void AddCurvedExplosionForce(Rigidbody2D body, Vector2 blastPos, float blastRadius, float blastForce, AnimationCurve blastCurve, ForceMode2D forceMode = ForceMode2D.Impulse) {
        body.AddForce(CalculateCurvedExplosionForce(body.worldCenterOfMass, blastPos, blastRadius, blastForce * 100f, blastCurve), forceMode);
    }

    public static void PerformLinearExplosionForce(Vector2 blastPos, float blastRadius, float blastForce, LayerMask layerMask, ForceMode2D forceMode = ForceMode2D.Impulse) {
        PerformCurvedExplosionForce(blastPos, blastRadius, blastForce, layerMask, AnimationCurve.Linear(0f, 0f, 1f, 1f), forceMode);
    }
    public static void PerformCurvedExplosionForce(Vector2 blastPos, float blastRadius, float blastForce, LayerMask layerMask, AnimationCurve blastCurve, ForceMode2D forceMode = ForceMode2D.Impulse) {
        Collider2D[] hits = Physics2D.OverlapCircleAll(blastPos, blastRadius, layerMask);

        foreach (Collider2D hit in hits) {
            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
            if (rb != null) {
                AddCurvedExplosionForce(rb, blastPos, blastRadius, blastForce, blastCurve, forceMode);
            }
        }
    }

    // Explosion Damage
    public static float CalculateLinearExplosionDamage(Vector2 pos, Vector2 blastPos, float blastRadius, float blastDamage) {
        return CalculateCurvedExplosionDamage(pos, blastPos, blastRadius, blastDamage, AnimationCurve.Linear(0f, 0f, 1f, 1f));
    }

    public static float CalculateCurvedExplosionDamage(Vector2 pos, Vector2 blastPos, float blastRadius, float blastDamage, AnimationCurve blastCurve) {
        Vector2 compVector = pos - blastPos;
        float blastDist = compVector.magnitude;

        if (blastDist > blastRadius) {
            // Out of range. No effect.
            return 0f;
        }

        float DamagePercent = blastCurve.Evaluate(1f - (blastDist / blastRadius));
        float retVal = blastDamage * DamagePercent;
        return retVal;
    }


    public static void AddLinearExplosionDamage(Hurtbox hurt, Vector2 hurtPos, Vector2 blastPos, float blastRadius, float blastDamage) {
        AddCurvedExplosionDamage(hurt, hurtPos, blastPos, blastRadius, blastDamage, AnimationCurve.Linear(0f, 0f, 1f, 1f));
    }

    public static void AddCurvedExplosionDamage(Hurtbox hurt, Vector2 hurtPos, Vector2 blastPos, float blastRadius, float blastDamage, AnimationCurve blastCurve) {
        hurt.RecieveDamageHit(CalculateCurvedExplosionDamage(hurtPos, blastPos, blastRadius, blastDamage * 100f, blastCurve));
    }

    public static void PerformLinearExplosionDamage(Vector2 blastPos, float blastRadius, float blastDamage, LayerMask layerMask) {
        PerformCurvedExplosionDamage(blastPos, blastRadius, blastDamage, layerMask, AnimationCurve.Linear(0f, 0f, 1f, 1f));
    }
    public static void PerformCurvedExplosionDamage(Vector2 blastPos, float blastRadius, float blastDamage, LayerMask layerMask, AnimationCurve blastCurve) {
        Collider2D[] hits = Physics2D.OverlapCircleAll(blastPos, blastRadius, layerMask);

        foreach (Collider2D hit in hits) {
            Hurtbox hurt = hit.GetComponent<Hurtbox>();
            if (hurt == null) {
                // No hurtbox. Can't damage this.
                continue;
            }

            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
            Vector2 hurtPos = hurt.transform.position;

            if (rb != null) {
                hurtPos = rb.worldCenterOfMass;
            }

            AddCurvedExplosionDamage(hurt, hurtPos, blastPos, blastRadius, blastDamage, blastCurve);
        }
    }
}
