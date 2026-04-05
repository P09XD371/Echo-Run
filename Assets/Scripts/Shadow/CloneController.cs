using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class CloneController : MonoBehaviour
{
    public List<Frame> frames;

    int frameIndex = 0;

    Rigidbody2D rb;
    Collider2D coll;
    SpriteRenderer sr;
    Animator anim;

    bool replaying = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        rb.simulated = false;
        coll.enabled = false;
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
    }
}