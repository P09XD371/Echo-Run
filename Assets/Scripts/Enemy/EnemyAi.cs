using Unity.VisualScripting;
using UnityEngine;

public class EnemyAi : MonoBehaviour
{
    public Transform player;

    public float speed = 3f;
    public float jumpForce = 7f;

    public float detectionRadius = 8f;
    public float attackDistance = 1.5f;

    public Transform groundCheck;
    public Transform edgeCheck;

    public LayerMask groundLayer;

    public GameObject attackHitbox;

    Rigidbody2D rb;

    bool isGrounded;
    bool canAttack = true;

    public Transform wallCheck;
    public float wallCheckDistance = 0.6f;
    public LayerMask obstacleLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        attackHitbox.SetActive(false);
    }

    void Update()
    {
        DetectGround();

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > detectionRadius)
            return;

        if (distance <= attackDistance)
        {
            Attack();
        }
        else
        {
            Chase();
        }
    }

    void OnDrawGizmosSelected()
{
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position, detectionRadius);

    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, attackDistance);
}

    void DetectGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    void Chase()
    {
        float dir = Mathf.Sign(player.position.x - transform.position.x);

        float xDistance = Mathf.Abs(player.position.x - transform.position.x);
        float heightDiff = player.position.y - transform.position.y;

        rb.velocity = new Vector2(dir * speed, rb.velocity.y);


        RaycastHit2D wall = Physics2D.Raycast(transform.position, Vector2.right * dir, 0.7f, groundLayer);

        if ((heightDiff > 1f && xDistance < 3f && isGrounded) || (wall && isGrounded))
        {
            Jump();
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
        if (!canAttack)
            return;

        canAttack = false;

        float dir = Mathf.Sign(player.position.x - transform.position.x);
        Flip(dir);

        Invoke(nameof(DoAttack), 0.5f);
    }

    void DoAttack()
    {
        rb.velocity = Vector2.zero;

        attackHitbox.SetActive(true);

        Invoke(nameof(StopAttack), 0.3f); 
        Invoke(nameof(ResetAttack), 1f);  
    }


    void Flip(float dir)
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * dir;
        transform.localScale = scale;
    }

    void StopAttack()
    {
        attackHitbox.SetActive(false);
    }

    void ResetAttack()
    {
        canAttack = true;
    }

    bool IsWallAhead(float dir)
    {
        RaycastHit2D hit = Physics2D.Raycast(wallCheck.position, Vector2.right * dir, wallCheckDistance, obstacleLayer);

        if (hit)
        {
            if (hit.collider.CompareTag("Ground"))
                return true;
        }

        return false;
    }
}
