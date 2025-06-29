using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    private EnemyWeapon normalWeapon;
    private SpecialAttackBehaviour specialAttack;
    public float specialAttackInterval = 10f;
    public float phase2HealthPercent = 0.5f; // Cambia a fase 2 al 50% de vida
    private float specialTimer;
    private EnemyStats stats;
    private bool inPhase2 = false;

    public GameObject propPrefab; // Prefab del prop no destructible sin collider
    public int boxSize = 20; // Tamaño del cuadrado (en unidades)
    private bool hasDoneIntroAttack = false;
    private bool isIntroPlaying = false;
    private Vector3 playerPositionReference;


    void Start()
    {
        stats = GetComponent<EnemyStats>();
        specialTimer = specialAttackInterval;
        normalWeapon = GetComponent<EnemyWeapon>();
        specialAttack = GetComponent<SpecialAttackBehaviour>();

        // Empezar intro
        Vector3 playerPos = FindObjectOfType<PlayerStats>().transform.position;
        InitiateIntroAttack(playerPos);
    }

    void Awake()
    {
        isIntroPlaying = true; // Bloquear lógica desde el primer frame
    }

    void Update()
    {
        // ✅ Detener comportamiento normal mientras la intro está activa
        if (isIntroPlaying) return;

        if (stats == null) return;

        // Fase 2
        if (!inPhase2 && stats.currentHealth <= stats.enemyData.MaxHealth * phase2HealthPercent)
        {
            EnterPhase2();
        }

        // Ataque especial
        specialTimer -= Time.deltaTime;
        if (specialTimer <= 0f)
        {
            if (specialAttack != null)
                specialAttack.TriggerSpecial();

            specialTimer = specialAttackInterval;
        }
    }

    void EnterPhase2()
    {
        inPhase2 = true;
        // Ejemplo: aumentar dificultad
        if (normalWeapon != null)
        {
            normalWeapon.fireRate *= 1.5f;
            normalWeapon.bulletCount += 2;
        }
        if (specialAttack != null)
        {
            specialAttack.numberOfProjectiles += 5;
        }
        Debug.Log("¡El jefe ha entrado en la Fase 2!");
    }


    public void OnIntroAnimationComplete()
    {
        GeneratePropCage(playerPositionReference);
        StartCoroutine(DelayBeforeActivation(1f)); // Espera opcional antes de volver a atacar
    }

    public void InitiateIntroAttack(Vector3 playerPos)
    {
        if (hasDoneIntroAttack) return;

        hasDoneIntroAttack = true;
        isIntroPlaying = true; // <- BLOQUEAMOS EL UPDATE NORMAL
        playerPositionReference = playerPos;

        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("IntroAttack");
        }
    }


    void GeneratePropCage(Vector3 center)
    {
        for (int x = -boxSize; x <= boxSize; x++)
        {
            for (int y = -boxSize; y <= boxSize; y++)
            {
                // Solo colocar en bordes (formando la caja)
                if (Mathf.Abs(x) == boxSize || Mathf.Abs(y) == boxSize)
                {
                    Vector3 spawnPos = new Vector3(center.x + x, center.y + y, 0);
                    Instantiate(propPrefab, spawnPos, Quaternion.identity);
                }
            }
        }
    }


    IEnumerator DelayBeforeActivation(float delay)
    {
        yield return new WaitForSeconds(delay);

        // ¡Ahora el jefe puede volver a actuar!
        isIntroPlaying = false;
    }
}
