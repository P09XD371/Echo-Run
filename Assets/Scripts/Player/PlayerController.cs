using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Component References")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] ParticleSystem dustParticle;
    private bool useMobileInput = false;

    private Vector2 particlesStartPos;

    [Header("Climbing")]
    [SerializeField] float climbSpeed = 3f;
    [SerializeField] LayerMask ladderLayer;     

    private bool isClimbing = false;             
    private float vertical;

    [Header("Player Settings")]
    [SerializeField] float speed;
    [SerializeField] float jumpingPower;
    [SerializeField] float acceleration = 20f;

    [Header("Grounding")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform groundCheck;

    private float horizontal;

    private void Start()
    {
        useMobileInput = Application.isMobilePlatform;
        particlesStartPos = dustParticle.transform.localPosition;
    }

    private void Update()
    {
        animator.SetFloat("Speed", Mathf.Abs(horizontal));
        animator.SetBool("IsGrounded", IsGrounded());

        UpdateFlip();

        HandleParticles();
    }

    private void UpdateFlip()
    {
        if (horizontal > 0)
            sprite.flipX = false;
        else if (horizontal < 0)
            sprite.flipX = true;
    }

    private void HandleParticles()
    {
        bool grounded = IsGrounded();

        var emission = dustParticle.emission;

        if (grounded && Mathf.Abs(horizontal) > 0f)
        {
            Vector2 particlesPos = particlesStartPos;
            if (sprite.flipX) particlesPos.x *= -1f;
            dustParticle.transform.localPosition = particlesPos;
            dustParticle.transform.localRotation = sprite.flipX ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);

            emission.enabled = true;
        }
        else
        {
            emission.enabled = false;
            dustParticle.Clear();
        }
    }

    private void FixedUpdate()
    {
        float targetSpeed = horizontal * speed;
        float newSpeed = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, acceleration * Time.fixedDeltaTime);

        if (isClimbing)
        {
            rb.linearVelocity = new Vector2(newSpeed, vertical * climbSpeed);
            rb.gravityScale = 0f;
        }
        else
        {
            rb.linearVelocity = new Vector2(newSpeed, rb.linearVelocity.y);
            rb.gravityScale = 2f;
        }
    }

    [SerializeField] Transform attackHitbox;
    [SerializeField] Vector2 hitboxOffsetRight;
    [SerializeField] Vector2 hitboxOffsetLeft;

    private void UpdateHitboxPosition()
    {
        if (sprite.flipX)
        {
            attackHitbox.localPosition = hitboxOffsetLeft;
        }
        else
        {
            attackHitbox.localPosition = hitboxOffsetRight;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & ladderLayer) != 0)
        {
            isClimbing = true;
            animator.SetBool("IsClimbing", true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & ladderLayer) != 0)
        {
            isClimbing = false;
            animator.SetBool("IsClimbing", false);
        }
    }

    #region PLAYER_CONTROL

    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
        vertical = context.ReadValue<Vector2>().y;

        bool grounded = IsGrounded();

        if (useMobileInput) return;

        horizontal = context.ReadValue<Vector2>().x;
        vertical = context.ReadValue<Vector2>().y;

        if (horizontal > 0)
        {
            sprite.flipX = false;
            attackHitbox.localPosition = hitboxOffsetRight;
        }
        else if (horizontal < 0)
        {
            sprite.flipX = true;
            attackHitbox.localPosition = hitboxOffsetLeft;
        }

        var emission = dustParticle.emission;

        if (grounded && horizontal != 0)
        {
            Vector2 particlesPos = particlesStartPos;
            if (sprite.flipX) particlesPos.x *= -1f;
            dustParticle.transform.localPosition = particlesPos;
            dustParticle.transform.localRotation = sprite.flipX ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);

            emission.enabled = true;
        }
        else
        {
            emission.enabled = false;
            dustParticle.Clear();
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.3f, groundLayer);
    }

    public void MobileUpDown()
    {
        vertical = 1f;
    }

    public void MobileDownDown()
    {
        vertical = -1f;
    }

    public void MobileVerticalStop()
    {
        vertical = 0f;
    }

    public void MobileMoveLeftDown()
    {
        horizontal = -1f;
    }

    public void MobileMoveRightDown()
    {
        horizontal = 1f;
    }

    public void MobileMoveStop()
    {
        horizontal = 0f;
    }

    public void MobileJump()
    {
        if (IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
        }
    }
    #endregion
}