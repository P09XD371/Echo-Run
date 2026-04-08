using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Target & Movement")]
    public Transform player;
    public float speed = 3f;
    public float jumpForce = 7f;
    public float detectionRadius = 8f;
    public float attackDistance = 1.5f;

    [Header("Ground & Obstacles")]
    public Transform groundCheck;
    public Transform wallCheck;
    public float wallCheckDistance = 0.6f;
    public LayerMask groundLayer;
    public LayerMask obstacleLayer;

    [Header("Attack")]
    public GameObject attackHitbox;

    [Header("UI")]
    public BossHealthBar healthBar;

    Rigidbody2D rb;

    bool isGrounded;

    bool playerInside = false;
    float attackTimer = 0f;
    float attackDelay = 3f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        attackHitbox.SetActive(true);

        EnemyAttackHitbox hitbox = attackHitbox.GetComponent<EnemyAttackHitbox>();

        if (hitbox != null)
        {
            hitbox.OnPlayerEnter += OnPlayerEnter;
            hitbox.OnPlayerExit += OnPlayerExit;
        }
    }

    void Update()
    {
        DetectGround();

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > detectionRadius) return;


        if (distance <= attackDistance)
        {
            Attack();
        }
        else
        {
            Chase();
        }

        if (playerInside)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= attackDelay)
            {
                KillPlayer();
            }
        }
    }

    void DetectGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    void Chase()
    {
        float dir = Mathf.Sign(player.position.x - transform.position.x);

        // Двигаемся к игроку
        rb.velocity = new Vector2(dir * speed, rb.velocity.y);

        // Прыжок через стену
        if (IsWallAhead(dir) && isGrounded)
        {
            Jump();
        }

        // Прыжок на платформу, если игрок выше
        JumpTowardsPlayer();

        Flip(dir);
    }

    void JumpTowardsPlayer()
    {
        float verticalDiff = player.position.y - transform.position.y;
        float horizontalDiff = player.position.x - transform.position.x;

        // Проверяем только если игрок выше врага
        if (verticalDiff > 0.5f)
        {
            // Луч вперед и вниз под небольшим углом (1 юнит вперед, 2 вверх)
            Vector2 origin = new Vector2(transform.position.x, transform.position.y + 0.1f);
            Vector2 direction = new Vector2(Mathf.Sign(horizontalDiff), 1).normalized;
            float distance = 2f;

            RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, groundLayer);
            Debug.DrawRay(origin, direction * distance, Color.red);

            // Прыгаем, если есть платформа
            if (hit.collider != null && isGrounded)
            {
                Jump();
            }
        }
    }

    void Jump()
    {
        if (isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    void Attack()
    {
        rb.velocity = Vector2.zero;
    }

    void OnPlayerEnter(PlayerController player)
    {
        Debug.Log("PLAYER ENTER HITBOX");

        playerInside = true;
        attackTimer = 0f;
    }

    void OnPlayerExit(PlayerController player)
    {
        playerInside = false;
        attackTimer = 0f;
    }

    void KillPlayer()
    {
        GameController gc = FindObjectOfType<GameController>();

        if (gc != null)
        {
            gc.Die();
        }

        playerInside = false;
        attackTimer = 0f;
    }

    void Flip(float dir)
    {
        Vector3 scale = transform.localScale;

        scale.x = Mathf.Abs(scale.x) * Mathf.Sign(dir);

        transform.localScale = scale;
    }

    bool IsWallAhead(float dir)
    {
        RaycastHit2D hit = Physics2D.Raycast(
            wallCheck.position,
            Vector2.right * dir,
            wallCheckDistance,
            obstacleLayer
        );

        return hit.collider != null && hit.collider.CompareTag("Ground");
    }



    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}