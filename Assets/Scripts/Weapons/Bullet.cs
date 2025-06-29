using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletLife = 1f;
    private float speed = 5f;
    private Vector2 direction;
    private float timer = 0f;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > bulletLife)
        {
            Destroy(gameObject);
            return;
        }

        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
