using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyStats : MonoBehaviour
{
    public EnemyScriptableObject enemyData;

    [HideInInspector]
    public float currentMoveSpeed;
    [HideInInspector]
    public float currentHealth;
    [HideInInspector]
    public float currentDamage;
    public bool immuneToKnockback = false;

    public float despawnDistance = 20f;
    Transform player;

    [Header("Damage Feedback")]
    public Color damageColor = new Color(1, 0, 0, 1); // What color of the damage flash should be.
    public float damageFlashDuration = 0.2f; // How long the damage flash should last.
    public float deathFadeTime = 0.6f; // How much time it takes for the enemy to fade.
    Color originalColor;
    SpriteRenderer sr;
    EnemyMovement movement;

    void Awake()
    {
        currentMoveSpeed = enemyData.MoveSpeed;
        currentHealth = enemyData.MaxHealth;
        currentDamage = enemyData.Damage;
    }

    void Start()
    {
        player = FindFirstObjectByType<PlayerStats>().transform;
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;

        movement = GetComponent<EnemyMovement>();
    }

    void Update()
    {
        if (Vector2.Distance(transform.position, player.position) >= despawnDistance)
        {
            ReturnEnemy();
        }
    }

    public void TakeDamage(float dmg, Vector2 sourcePosition, float knockbackForce = 5f, float knockbackDuration = 0.2f)
    {
        currentHealth -= dmg;
        StartCoroutine(DamageFlash());

        // Create the text popup when enemy takes damage.
        if (dmg > 0)
            GameManager.GenerateFloatingText(Mathf.FloorToInt(dmg).ToString(), transform);

        // Apply knockback if it is not zero.
        if (knockbackForce > 0 && !immuneToKnockback)
        {
            Vector2 dir = (Vector2)transform.position - sourcePosition;
            movement.Knockback(dir.normalized * knockbackForce, knockbackDuration);
        }

        // Kils the enemy if the health drops below 0.
        if (currentHealth <= 0)
        {
            Kill();
        }

    }

    //This is a Coroutine function that makes the enemy flash red when taking damage.
    IEnumerator DamageFlash()
    {
        sr.color = damageColor;
        yield return new WaitForSeconds(damageFlashDuration);
        sr.color = originalColor;
    }

    public void Kill()
    {
        StartCoroutine(KillFade());
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            PlayerStats player = col.gameObject.GetComponent<PlayerStats>();
            player.TakeDamage(currentDamage);
        }
    }

    // This is a Coroutine function that fades the enemy away slowly
    IEnumerator KillFade()
    {
        // Waits for a single frame.
        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0, origAlpha = sr.color.a;

        // This is a loop that fires every frame.
        while (t < deathFadeTime)
        {
            yield return w;
            t += Time.deltaTime;

            // Set the colour for this frame.
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, (1 - t / deathFadeTime) * origAlpha);
        }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        EnemySpawner es = FindFirstObjectByType<EnemySpawner>();
        es.OnEnemyKilled();
    }

    void ReturnEnemy()
    {
        EnemySpawner es = FindFirstObjectByType<EnemySpawner>();
        transform.position = player.position + es.relativeSpawnPositions[Random.Range(0, es.relativeSpawnPositions.Count)].position;
    }
}
