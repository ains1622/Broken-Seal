using UnityEngine;

public class EnemyAttackTrigger : MonoBehaviour
{
    private EnemyWeapon enemyWeapon; // Referencia al EnemyWeapon
    public Animator enemyAnimator;
    private float fireTimer = 0f;

    private Transform target;                  // Jugador
    private bool isAttacking = false;

    [Header("Bullet Pattern Settings")]
    public float spreadAngle = 45f;       // Ángulo total de dispersión (en grados)

    [Header("Fase 1 (Normal)")]
    public float fireRate = 1f;
    public int bulletCount = 3;
    public float animSpeed = 1f;

    [Header("Fase 2 (Especial)")]
    public float fireRatePhase2 = 0.5f;
    public int bulletCountPhase2 = 7;
    public float animSpeedPhase2 = 1.5f;

    [Header("Tiempo de cambio de fase")]
    public float timeBeforePhase2 = 5f;
    private float attackTime = 0f;

    private bool inPhase2 = false;

    [Header("Velocidad de Balas")]
    public float bulletSpeed = 5f;
    public float bulletSpeedPhase2 = 10f;


    void Start()
    {
        enemyAnimator = GetComponent<Animator>();
        enemyAnimator.speed = animSpeed;
        enemyWeapon = GetComponent<EnemyWeapon>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (isAttacking && target != null)
        {
            attackTime += Time.deltaTime;
            if (!inPhase2 && attackTime >= timeBeforePhase2)
            {
                EnterPhase2();
            }
            fireTimer += Time.deltaTime;
            float currentFireRate = inPhase2 ? fireRatePhase2 : fireRate;
            int currentBulletCount = inPhase2 ? bulletCountPhase2 : bulletCount;
            float currentBulletSpeed = inPhase2 ? bulletSpeedPhase2 : bulletSpeed;
            float currentAnimSpeed = inPhase2 ? animSpeedPhase2 : animSpeed;
            enemyAnimator.speed = currentAnimSpeed;
            if (fireTimer >= 1f / currentFireRate)
            {
                fireTimer = 0f;
                if (enemyWeapon != null)
                {
                    enemyWeapon.bulletCount = currentBulletCount;
                    enemyWeapon.spreadAngle = spreadAngle;
                    enemyWeapon.projectileSpeed = currentBulletSpeed;
                    enemyWeapon.fireRate = currentFireRate;
                    enemyWeapon.Fire();
                }
            }
        }
    }

    private void StartAttackingImmediately()
    {
        isAttacking = true;
        fireTimer = 0f;
    }


    private void EnterPhase2()
    {
        inPhase2 = true;
        enemyAnimator.speed = animSpeedPhase2;
        Debug.Log("¡Fase 2 activada!");
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            enemyAnimator.SetBool("isAttacking", true);
            StartAttackingImmediately();
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StopAllCoroutines();
            isAttacking = false;
            attackTime = 0f;
            inPhase2 = false;
            enemyAnimator.speed = animSpeed;

            enemyAnimator.SetBool("isAttacking", false);
        }
    }
}
