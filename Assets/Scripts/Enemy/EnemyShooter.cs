using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float shootInterval = 2f;
    public float projectileSpeed = 5f;
    public Transform firePoint;
    private Transform player;
    private float shootTimer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        shootTimer = shootInterval;
        if (firePoint == null) firePoint = transform;
    }

    void Update()
    {
        if (player == null) return;
        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0f)
        {
            ShootAtPlayer();
            shootTimer = shootInterval;
        }
    }

    void ShootAtPlayer()
    {
        Vector2 direction = (player.position - firePoint.position).normalized;
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = direction * projectileSpeed;
    }
}
