using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackCoordinator : MonoBehaviour
{
    public float attackInterval = 3f; // Tiempo entre ataques normales
    private float timer;

    private PlayerAreaIndicator areaIndicator;
    private Transform player;

    void Start()
    {
        areaIndicator = FindFirstObjectByType<PlayerAreaIndicator>();
        player = FindFirstObjectByType<PlayerMovement>()?.transform;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= attackInterval)
        {
            timer = 0f;
            TryAttackFromMostCrowdedZone();

            // Puedes variar el tiempo entre ataques si quieres algo más dinámico
            attackInterval = Random.Range(1f, 6f);
        }
    }

    void TryAttackFromMostCrowdedZone()
    {
        if (areaIndicator == null) return;

        // Obtener conteo por zona
        Dictionary<PlayerZone, int> counts = new Dictionary<PlayerZone, int>
        {
            { PlayerZone.Northwest, areaIndicator.northwestCount },
            { PlayerZone.Northeast, areaIndicator.northeastCount },
            { PlayerZone.Southwest, areaIndicator.southwestCount },
            { PlayerZone.Southeast, areaIndicator.southeastCount }
        };

        // Buscar la zona más poblada
        PlayerZone maxZone = PlayerZone.Northwest;
        int maxCount = 0;

        foreach (var pair in counts)
        {
            if (pair.Value > maxCount)
            {
                maxZone = pair.Key;
                maxCount = pair.Value;
            }
        }

        if (maxCount == 0) return;

        // Obtener enemigos de esa zona
        List<Transform> enemiesInZone = new List<Transform>();
        foreach (var enemy in GetActiveEnemies())
        {
            if (enemy != null && areaIndicator.GetZone(enemy.position) == maxZone)
            {
                enemiesInZone.Add(enemy);
            }
        }

        if (enemiesInZone.Count == 0) return;

        Transform chosenEnemy = enemiesInZone[Random.Range(0, enemiesInZone.Count)];
        //EnemyWeaponController weaponController = chosenEnemy.GetComponent<EnemyWeaponController>();
        SpecialAttackBehaviour special = chosenEnemy.GetComponent<SpecialAttackBehaviour>();

        //if (weaponController != null)
        {
            // Solo permitir ataque normal si no está en ataque especial
            if (special == null || !special.IsSpecialAttackActive())
            {
                //weaponController.SetTarget(player);
                //weaponController.ManualAttack();
            }
        }
    }

    private List<Transform> GetActiveEnemies()
    {
        var field = areaIndicator.GetType().GetField("activeEnemies", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return field?.GetValue(areaIndicator) as List<Transform> ?? new List<Transform>();
    }
}

