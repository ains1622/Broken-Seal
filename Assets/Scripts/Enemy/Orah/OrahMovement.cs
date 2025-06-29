using UnityEngine;

public class OrahMovement : MonoBehaviour
{
    EnemyStats enemy;
    Transform player;
    [SerializeField]
    private Animator animator;

    Vector2 lastPosition;
    Vector3 originalScale;

    void Start()
    {
        enemy = GetComponent<EnemyStats>();
        player = FindFirstObjectByType<PlayerMovement>().transform;
        lastPosition = transform.position;
        originalScale = transform.localScale;
    }

    void Update()
    {
        bool isAttacking = animator.GetBool("isAttacking");

        // Solo moverse si no está atacando
        if (!isAttacking)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, enemy.currentMoveSpeed * Time.deltaTime);
        }

        // Calcular velocidad y actualizar animación
        Vector2 currentPosition = transform.position;
        float speed = (currentPosition - lastPosition).magnitude / Time.deltaTime;
        if (speed > 0f) animator.SetFloat("Speed", 1f);
        else animator.SetFloat("Speed", 0f);
        lastPosition = currentPosition;

        // Opcional: Voltear sprite hacia el jugador
        if (player.position.x < transform.position.x){
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
        else{
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
    }
}
