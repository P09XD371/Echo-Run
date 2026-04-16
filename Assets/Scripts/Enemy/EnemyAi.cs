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

    [Header("Obstacle Jump")]
    public Transform obstacleCheck;
    public float obstacleCheckDistance = 0.5f;
    public float obstacleHeightCheck = 1.2f;
    
    [Header("Jump Detection")]
    public Transform obstacleDetector;
    public Vector2 obstacleSize = new Vector2(0.5f, 0.8f);

    Rigidbody2D rb;

    bool isGrounded;

    bool playerInside = false;
    float attackTimer = 0f;
    float attackDelay = 2f;

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

        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        DetectGround();

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > detectionRadius)
        {
            if (healthBar != null)
                healthBar.gameObject.SetActive(false);

            return;
        }

        if (healthBar != null)
            healthBar.gameObject.SetActive(true);

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

    bool IsObstacleAhead(float dir)
    {
        Vector2 center = obstacleDetector.position + new Vector3(dir * 0.3f, 0);

        Collider2D hit = Physics2D.OverlapBox(
            center,
            obstacleSize,
            0,
            obstacleLayer
        );

        return hit != null;
    }

    bool ShouldJump(float dir)
    {
        Vector2 forwardOrigin = obstacleCheck.position;
        Vector2 upOrigin = obstacleCheck.position + Vector3.up * obstacleHeightCheck;

        Vector2 forwardDir = Vector2.right * dir;

        RaycastHit2D wall = Physics2D.Raycast(
            forwardOrigin,
            forwardDir,
            obstacleCheckDistance,
            obstacleLayer
        );

        RaycastHit2D spaceAbove = Physics2D.Raycast(
            upOrigin,
            forwardDir,
            obstacleCheckDistance,
            obstacleLayer
        );

        Debug.DrawRay(forwardOrigin, forwardDir * obstacleCheckDistance, Color.red);
        Debug.DrawRay(upOrigin, forwardDir * obstacleCheckDistance, Color.green);

        return wall.collider != null && spaceAbove.collider == null;
    }

    void DetectGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    void Chase()
    {
        float dir = Mathf.Sign(player.position.x - transform.position.x);

        rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);

        if (IsObstacleAhead(dir) && isGrounded)
        {
            Jump();
        }

        Flip(dir);
    }

    void JumpTowardsPlayer()
    {
        float verticalDiff = player.position.y - transform.position.y;
        float horizontalDiff = player.position.x - transform.position.x;

        if (verticalDiff > 0.5f)
        {

            Vector2 origin = new Vector2(transform.position.x, transform.position.y + 0.1f);
            Vector2 direction = new Vector2(Mathf.Sign(horizontalDiff), 1).normalized;
            float distance = 2f;

            RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, groundLayer);
            Debug.DrawRay(origin, direction * distance, Color.red);

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
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    void Attack()
    {
        rb.linearVelocity = Vector2.zero;
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
        Vector2 origin = wallCheck.position;
        Vector2 direction = Vector2.right * dir;

        Debug.DrawRay(origin, direction * wallCheckDistance, Color.green);

        RaycastHit2D hit = Physics2D.Raycast(
            origin,
            direction,
            wallCheckDistance,
            obstacleLayer
        );

        return hit.collider != null;
    }

    bool IsStepAhead(float dir)
    {
        Vector2 origin = new Vector2(transform.position.x, transform.position.y - 0.4f);
        Vector2 direction = Vector2.right * dir;

        float distance = 0.6f;

        Debug.DrawRay(origin, direction * distance, Color.blue);

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, obstacleLayer);

        return hit.collider != null;
    }

    void OnDrawGizmos()
    {
        if (obstacleDetector == null) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(obstacleDetector.position, obstacleSize);
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}