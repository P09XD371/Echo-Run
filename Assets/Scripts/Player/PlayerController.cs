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

    private Vector2 particlesStartPos;

    [Header("Player Settings")]
    [SerializeField] float speed;
    [SerializeField] float jumpingPower;

    [Header("Grounding")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform groundCheck;

    private float horizontal;

    private void Start()
    {
        particlesStartPos = dustParticle.transform.localPosition;
    }

    private void Update()
    {
        animator.SetFloat("Speed", Mathf.Abs(horizontal));
        animator.SetBool("IsGrounded", IsGrounded());

        HandleParticles();
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
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    #region PLAYER_CONTROL

    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;

        bool grounded = IsGrounded();

        if (horizontal > 0)
            sprite.flipX = false;
        else if (horizontal < 0)
            sprite.flipX = true;

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
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.3f, groundLayer);
    }

    #endregion
}