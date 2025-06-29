using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float damage = 10f;
    public float speed = 5f;
    public float lifeTime = 5f;
    private Vector2 direction;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    public void SetSpeed(float s)
    {
        speed = s;
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStats player = other.GetComponent<PlayerStats>();
            if (player != null)
                player.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Prop"))
        {
            // No hacer nada, ignora la colisión con Props
            return;
        }
        else if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
