using System.Collections.Generic;
using UnityEngine;

public class CloneController : MonoBehaviour
{
    public List<Frame> frames;
    public GameObject attackHitbox; // HITBOX клона

    private int frameIndex = 0;

    private Rigidbody2D rb;
    private Collider2D coll;
    private SpriteRenderer sr;
    private Animator anim;

    private bool replaying = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        rb.simulated = false;
        coll.enabled = false;

        // Авто-находим хитбокс, если не назначен
        if (attackHitbox == null)
        {
            attackHitbox = transform.Find("AttackHitbox")?.gameObject;
            if (attackHitbox == null)
                Debug.LogWarning("CloneController: AttackHitbox не назначен у клона " + gameObject.name);
        }

        if (attackHitbox != null)
            attackHitbox.SetActive(false);
    }

    void FixedUpdate()
    {
        if (!replaying) return;

        if (frameIndex >= frames.Count)
        {
            StopClone();
            return;
        }

        Frame frame = frames[frameIndex];

        Vector2 oldPos = transform.position;

        transform.position = frame.position;
        sr.flipX = frame.flipX;

        Vector2 velocity = (frame.position - oldPos) / Time.fixedDeltaTime;
        anim.SetFloat("Speed", Mathf.Abs(velocity.x));

        // Управляем хитбоксом
        if (frame.attack && attackHitbox != null)
        {
            if (!attackHitbox.activeSelf)
            {
                attackHitbox.SetActive(true);
                Debug.Log(gameObject.name + " атакует на кадре " + frameIndex);
            }
        }
        else if (attackHitbox != null && attackHitbox.activeSelf)
        {
            attackHitbox.SetActive(false);
        }

        frameIndex++;
    }

    void StopClone()
    {
        replaying = false;

        rb.simulated = true;
        coll.enabled = true;

        rb.bodyType = RigidbodyType2D.Static;
        sr.color = new Color(0.6f, 0.6f, 0.6f, 1f);
        gameObject.tag = "Ground";
        gameObject.layer = LayerMask.NameToLayer("Ground");
    }

    public void Init(List<Frame> newFrames)
    {
        frames = newFrames;
        frameIndex = 0;
        replaying = true;
    }
}