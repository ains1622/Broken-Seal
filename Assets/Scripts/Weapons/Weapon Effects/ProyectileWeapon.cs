using UnityEngine;

public class ProyectileWeapon : Weapon
{
    protected float currentAttackInterval;
    protected int currentAttackCount; // Number of times this attack will happen.

    protected override void Update()
    {
        base.Update();

        // Otherwise, if the attack interval goes from above 0 to below, we also call attack.
        if (currentAttackInterval > 0)
        {
            currentAttackInterval -= Time.deltaTime;
            if (currentAttackInterval <= 0) Attack(currentAttackCount);
        }
    }

    public override bool CanAttack()
    {
        if (currentAttackCount > 0) return true;
        return base.CanAttack();
    }

    protected override bool Attack(int attackCount = 1)
    {
        // If no proyectile prefab is assigned, leave a waning message.
        if (!currentStats.projectilePrefab)
        {
            Debug.LogWarning(string.Format("Proyectile prefab has not been set for {0}", name));
            ActiveCooldown();
            return false;
        }

        // Can we attack?
        if (!CanAttack()) return false;

        // Otherwise, calculate the angle and offset of our spawned proyectile.
        float spawnAngle = GetSpawnAngle();

        // If there is a proc effect, play it.
        if (currentStats.procEffect)
        {
            Destroy(Instantiate(currentStats.procEffect, owner.transform), 5f);
        }

        // And spawn a copy of the proyectile.
            Proyectile prefab = Instantiate(
            currentStats.projectilePrefab,
            owner.transform.position + (Vector3)GetSpawnOffset(spawnAngle),
            Quaternion.Euler(0, 0, spawnAngle)
        );

        prefab.weapon = this;
        prefab.owner = owner;

        ActiveCooldown();

        attackCount--;

        // Do we perform another attack?
        if (attackCount > 0)
        {
            currentAttackCount = attackCount;
            currentAttackInterval = currentStats.proyectileInterval;
        }

        return true;
    }

    // Gets wich direction the proyectile should face when spawning.

    protected virtual float GetSpawnAngle()
    {
        return Mathf.Atan2(movement.lastMovedVector.y, movement.lastMovedVector.x) * Mathf.Rad2Deg;
    }

    // Generates a random point to spawn the proyectile on,
    // And rotates the facing of the point by spawnAngle.

    protected virtual Vector2 GetSpawnOffset(float spawnAngle = 0)
    {
        return Quaternion.Euler(0, 0, spawnAngle) * new Vector2(
            Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax),
            Random.Range(currentStats.spawnVariance.yMin, currentStats.spawnVariance.yMax)
        );
    }
}
