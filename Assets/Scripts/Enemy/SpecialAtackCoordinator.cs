using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAttackCoordinator : MonoBehaviour
{
    public float specialAttackInterval = 30f;
    private float timer;

    private PlayerAreaIndicator areaIndicator;

    void Start()
    {
        areaIndicator = FindFirstObjectByType<PlayerAreaIndicator>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= specialAttackInterval)
        {
            timer = 0f;
            TriggerSpecialAttack();
        }
    }

    private void TriggerSpecialAttack()
    {
        if (areaIndicator == null) return;

        var enemies = GetActiveEnemies(areaIndicator);

        foreach (var enemy in enemies)
        {
            if (enemy == null || !enemy.gameObject.activeInHierarchy) continue;

            if (enemy.TryGetComponent(out SpecialAttackBehaviour specialAttack))
            {
                if (!specialAttack.IsSpecialAttackActive())
                    specialAttack.TriggerSpecial();
            }
        }
    }

    private List<Transform> GetActiveEnemies(PlayerAreaIndicator indicator)
    {
        return indicator.GetType()
            .GetField("activeEnemies", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.GetValue(indicator) as List<Transform>;
    }
}
