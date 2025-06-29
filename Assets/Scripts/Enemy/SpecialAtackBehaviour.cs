using System.Collections;
using UnityEngine;

public class SpecialAttackBehaviour : MonoBehaviour
{
    public float chargeDuration = 4f;
    public int numberOfProjectiles = 10;

    [Header("Audio")]
    public AudioClip specialAttackSound; // ← Clip específico para ataque especial

    private EnemyMovement enemyMovement;
    private Animator animator;
    //private EnemyWeaponController weaponController;
    private AudioSource audioSource;
    private EnemyWeapon enemyWeapon;

    private bool isCharging = false;

    private void Start()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>(); // Usa el AudioSource existente
        enemyWeapon = GetComponent<EnemyWeapon>();
    }

    public void TriggerSpecial()
    {
        if (!isCharging)
            StartCoroutine(PerformSpecialAttack());
    }

    public bool IsSpecialAttackActive()
    {
        return isCharging;
    }

    private IEnumerator PerformSpecialAttack()
    {
        isCharging = true;

        if (enemyMovement != null)
            enemyMovement.enabled = false;

        if (animator != null)
            animator.SetTrigger("ChargeSpecial");

        yield return new WaitForSeconds(chargeDuration);

        // 🔊 Reproducir audio de ataque especial (sin interrumpir otro sonido)
        if (audioSource != null && specialAttackSound != null)
        {
            audioSource.PlayOneShot(specialAttackSound);
        }

        FireRadialProjectiles();

        if (animator != null)
            animator.SetTrigger("Idle");

        if (enemyMovement != null)
            enemyMovement.enabled = true;

        isCharging = false;
    }

    private void FireRadialProjectiles()
    {
        if (enemyWeapon == null || enemyWeapon.projectilePrefab == null)
            return;

        float damage = enemyWeapon.projectileDamage;
        float speed = enemyWeapon.projectileSpeed;
        GameObject projectilePrefab = enemyWeapon.projectilePrefab;

        for (int i = 0; i < numberOfProjectiles; i++)
        {
            float angle = i * 360f / numberOfProjectiles;
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            Vector2 spawnPos = (Vector2)transform.position + dir * 0.5f; // Ajusta el 0.5f según el radio de tu collider
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
                ep.SetSpeed(speed);
                ep.damage = damage;
            }
        }
    }
}