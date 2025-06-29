using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1f;
    public Transform target;

    private float fireTimer = 0f;
    public bool isFiring = false;

    void Update()
    {
        if (isFiring && target != null)
        {
            fireTimer += Time.deltaTime;

            if (fireTimer >= 1f / fireRate)
            {
                fireTimer = 0f;
                Fire();
            }
        }
    }

    void Fire()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Vector2 direction = target.position - firePoint.position;
        bullet.GetComponent<Bullet>().SetDirection(direction);
    }
}
