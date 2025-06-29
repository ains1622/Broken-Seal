using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float fireRate = 1f;
    public int bulletCount = 1;
    public float spreadAngle = 0f;
    public float projectileSpeed = 5f;
    public float projectileDamage = 10f;
    private float fireTimer = 0f;
    private Transform target;
    private Collider2D col;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (target == null) return;
        fireTimer += Time.deltaTime;
        if (fireTimer >= 1f / fireRate)
        {
            fireTimer = 0f;
            Fire();
        }
    }

    public void Fire()
    {
        if (target == null) return;
        Vector2 center = (Vector2)transform.position;
        Vector2 directionToPlayer = ((Vector2)target.position - center).normalized;
        float baseAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        float startAngle = baseAngle - (spreadAngle / 2f);
        float angleStep = bulletCount > 1 ? spreadAngle / (bulletCount - 1) : 0f;

        // Calcula el radio del collider
        float colliderRadius = 0.5f;
        if (col is CircleCollider2D circle)
            colliderRadius = circle.radius * Mathf.Max(transform.localScale.x, transform.localScale.y);
        else if (col is CapsuleCollider2D capsule)
            colliderRadius = Mathf.Max(capsule.size.x, capsule.size.y) * 0.5f * Mathf.Max(transform.localScale.x, transform.localScale.y);
        else if (col is BoxCollider2D box)
            colliderRadius = Mathf.Max(box.size.x, box.size.y) * 0.5f * Mathf.Max(transform.localScale.x, transform.localScale.y);

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = startAngle + angleStep * i;
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            Vector2 spawnPos = center + dir * colliderRadius;
            GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            EnemyProjectile ep = proj.GetComponent<EnemyProjectile>();
            Collider2D projCol = proj.GetComponent<Collider2D>();
            if (projCol != null)
            {
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (GameObject enemy in enemies)
                {
                    Collider2D enemyCol = enemy.GetComponent<Collider2D>();
                    if (enemyCol != null)
                        Physics2D.IgnoreCollision(projCol, enemyCol);
                }
            }
            if (ep != null)
            {
                ep.SetDirection(dir);
                ep.SetSpeed(projectileSpeed);
                ep.damage = projectileDamage;
            }
        }
    }
}
