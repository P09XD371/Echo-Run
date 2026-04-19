using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class BossAI : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Arena")]
    public Transform arenaCenter;

    [Header("HP")]
    public float maxHealth = 100f;
    float currentHealth;

    [Header("Movement")]
    public float chargeSpeed = 18f;
    public float jumpForce = 12f;
    public float screenAttackSpeed = 20f;

    [Header("Timing")]
    public float attackCooldownTime = 5f;

    [Header("Cinemachine")]
    public CinemachineImpulseSource impulseSource;

    [Header("Hitbox")]
    public EnemyAttackHitbox hitbox;

    [Header("Detection")]
    public float detectionRadius = 10f;

    [Header("UI")]
    public GameObject bossHealthPanel;

    Rigidbody2D rb;

    bool isActive = false;
    bool isAttacking = false;
    bool attackActive = false;
    bool canAttack = true;

    bool phase2 = false;

    float speedMultiplier = 1f;
    float cooldownMultiplier = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        if (hitbox != null)
            hitbox.OnPlayerHit += HandlePlayerHit;

        StartCoroutine(BossLoop());
    }

    void Update()
    {
        CheckPhase();
        CheckPlayerDetection();
    }


    void CheckPlayerDetection()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectionRadius)
        {
            if (!isActive)
            {
                isActive = true;

                if (bossHealthPanel != null)
                    bossHealthPanel.SetActive(true);
            }
        }
        else
        {
            isActive = false;

            if (bossHealthPanel != null)
                bossHealthPanel.SetActive(false);
        }
    }


    void CheckPhase()
    {
        if (!phase2 && currentHealth <= maxHealth * 0.5f)
        {
            phase2 = true;
            speedMultiplier = 1.5f;
            cooldownMultiplier = 0.65f;
        }
    }

    IEnumerator BossLoop()
    {
        while (true)
        {
            if (isActive && !isAttacking && canAttack)
            {
                int attack = Random.Range(0, 3);

                if (attack == 0)
                    yield return StartCoroutine(ChargeAttack());

                if (attack == 1)
                    yield return StartCoroutine(JumpSmash());

                if (attack == 2)
                    yield return StartCoroutine(ScreenAttack());

                StartCoroutine(CooldownRoutine());
            }

            yield return null;
        }
    }


    IEnumerator ChargeAttack()
    {
        isAttacking = true;

        yield return MoveToCenter();

        attackActive = true;

        Shake(1f);

        float dir = Mathf.Sign(player.position.x - transform.position.x);

        float t = 0f;

        while (t < 1.2f)
        {
            t += Time.deltaTime;

            rb.linearVelocity = new Vector2(
                dir * chargeSpeed * speedMultiplier,
                rb.linearVelocity.y
            );

            yield return null;
        }

        attackActive = false;
        rb.linearVelocity = Vector2.zero;

        isAttacking = false;
    }

    IEnumerator JumpSmash()
    {
        isAttacking = true;

        yield return MoveToCenter();

        SlowMo(0.4f, 0.3f);

        attackActive = true;

        rb.linearVelocity = new Vector2(0, jumpForce * speedMultiplier);

        yield return new WaitForSeconds(0.5f);

        rb.linearVelocity = new Vector2(0, -jumpForce * 1.5f * speedMultiplier);

        yield return new WaitForSeconds(0.5f);

        attackActive = false;
        rb.linearVelocity = Vector2.zero;

        isAttacking = false;
    }


    IEnumerator ScreenAttack()
    {
        isAttacking = true;

        float side = Random.value > 0.5f ? 1 : -1;

        transform.position = new Vector2(player.position.x + side * 12f, transform.position.y);

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < 3; i++)
        {
            attackActive = true;

            float targetX = player.position.x;

            while (Mathf.Abs(transform.position.x - targetX) > 0.5f)
            {
                float dir = Mathf.Sign(targetX - transform.position.x);

                rb.linearVelocity = new Vector2(
                    dir * screenAttackSpeed * speedMultiplier,
                    0
                );

                yield return null;
            }

            attackActive = false;
            rb.linearVelocity = Vector2.zero;

            yield return new WaitForSeconds(0.2f);
        }

        isAttacking = false;
    }


    IEnumerator CooldownRoutine()
    {
        canAttack = false;

        yield return MoveToCenter();

        yield return new WaitForSeconds(attackCooldownTime * cooldownMultiplier);

        canAttack = true;
    }


    IEnumerator MoveToCenter()
    {
        while (Vector2.Distance(transform.position, arenaCenter.position) > 0.2f)
        {
            Vector2 dir = (arenaCenter.position - transform.position).normalized;
            rb.linearVelocity = dir * 5f;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
    }


    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
            Destroy(gameObject);
    }

    void HandlePlayerHit(PlayerController player)
    {
        if (!attackActive) return;

        GameController gc = FindObjectOfType<GameController>();

        if (gc != null)
            gc.Die();
    }

    void Shake(float force)
    {
        if (impulseSource != null)
            impulseSource.GenerateImpulse(force);
    }

    void SlowMo(float duration, float scale)
    {
        StartCoroutine(SlowMoRoutine(duration, scale));
    }

    IEnumerator SlowMoRoutine(float duration, float scale)
    {
        Time.timeScale = scale;
        Time.fixedDeltaTime = 0.02f * scale;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }
}