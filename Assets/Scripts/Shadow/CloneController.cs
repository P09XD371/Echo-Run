using System.Collections.Generic;
using UnityEngine;

public class CloneController : MonoBehaviour
{
    public List<Frame> frames;
    public GameObject attackHitbox;

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
        if (!replaying)
            return;

        if (frames == null || frames.Count == 0)
            return;

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

        if (anim != null)
            anim.SetFloat("Speed", Mathf.Abs(velocity.x));

        if (frame.attack && attackHitbox != null)
        {
            if (!attackHitbox.activeSelf)
            {
                attackHitbox.SetActive(true);
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

        rb.simulated = false;
        rb.bodyType = RigidbodyType2D.Kinematic;

        coll.enabled = false;

        sr.color = Color.white;

        gameObject.tag = "Clone";
        gameObject.layer = LayerMask.NameToLayer("Clone");

        if (frames != null && frames.Count > 0)
        {
            transform.position = frames[0].position;
        }

        if (attackHitbox != null)
            attackHitbox.SetActive(false);
    }

    public void Restart()
    {
        frameIndex = 0;
        replaying = true;

        rb.simulated = false;
        rb.bodyType = RigidbodyType2D.Kinematic;

        coll.enabled = false;

        sr.color = Color.white;

        gameObject.tag = "Clone";
        gameObject.layer = LayerMask.NameToLayer("Clone");

        if (attackHitbox != null)
            attackHitbox.SetActive(false);
    }
}