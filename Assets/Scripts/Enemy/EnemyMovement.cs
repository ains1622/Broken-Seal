using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    EnemyStats enemy;
    Transform player;
    private PlayerAreaIndicator areaIndicator;

    Vector2 knockbackVelocity;
    float knockbackDuration;

    void Start()
    {
        areaIndicator = FindFirstObjectByType<PlayerAreaIndicator>();
        if (areaIndicator != null)
        {
            areaIndicator.RegisterEnemy(transform);
        }

        enemy = GetComponent<EnemyStats>(); // Get the enemy stats component
        player = FindFirstObjectByType<PlayerMovement>().transform; // Find the player object in the scene
    }

    // Update is called once per frame
    void Update()
    {
        // If we are currently being knocked back, then process the knockback
        if (knockbackDuration > 0)
        {
            transform.position += (Vector3)knockbackVelocity * Time.deltaTime; // Apply the knockback velocity
            knockbackDuration -= Time.deltaTime; // Decrease the knockback duration
        }
        else
        {
            // Otherwise, constantly move the enemy towards the player
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, enemy.currentMoveSpeed * Time.deltaTime);
        }
    }

    private void OnDestroy()
    {
        if (areaIndicator != null)
        {
            areaIndicator.UnregisterEnemy(transform);
        }
    }

    // This is meant to be called from other scripts to create knockback

    public void Knockback(Vector2 velocity, float duration)
    {
        // Ignore the knockback if the duration is greater than 0
        if (knockbackDuration > 0) return;

        // Begins the knockback
        knockbackVelocity = velocity;
        knockbackDuration = duration;
    }
}