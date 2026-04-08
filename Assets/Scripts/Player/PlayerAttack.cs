using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayers = ~0;
    [SerializeField] private float comboResetTime = 1f;

    private Animator animator;
    private int attackStep = 0;
    private float lastAttackTime = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Сброс комбо, если прошло слишком много времени без атаки
        if (Time.time - lastAttackTime > comboResetTime)
        {
            attackStep = 0;
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        lastAttackTime = Time.time;

        int attackNumber = attackStep + 1; // 1, 2, 3
        animator.SetTrigger("Attack" + attackNumber);

        // Подготовка следующего шага комбо
        attackStep = (attackStep + 1) % 3;
    }

    public void DealDamage()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponentInParent<EnemyHealth>();
            if (enemyHealth != null) enemyHealth.TakeDamage(damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}