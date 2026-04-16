using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
    [Header("Player & Clones")]
    public GameObject clonePrefab;
    public InputAction fire;

    [Header("UI")]
    public TMP_Text counterText;
    public TMP_Text deathText;

    private Vector2 checkpointPos;
    private Rigidbody2D playerRb;
    private SpriteRenderer sr;

    private int coinCounter = 0;
    private int deathCounter = 0;

    private List<Frame> currentRun = new List<Frame>();
    private List<List<Frame>> runs = new List<List<Frame>>();
    private List<GameObject> activeClones = new List<GameObject>();
    public EnemyHealth bossHealth;

    void Awake()
    {
        playerRb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        if (fire != null)
            fire.Enable();
    }

    void Start()
    {
        checkpointPos = transform.position;
    }

    void FixedUpdate()
    {
        RecordFrame();
    }

    void RecordFrame()
    {
        bool isAttacking = fire != null && fire.ReadValue<float>() > 0f;
        Frame frame = new Frame(transform.position, sr.flipX, isAttacking);
        currentRun.Add(frame);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            Die();
        }
        else if (collision.CompareTag("Money") && collision.gameObject.activeSelf)
        {
            collision.gameObject.SetActive(false);
            coinCounter++;
            if (counterText != null)
                counterText.text = "Coins: " + coinCounter;
        }
    }

    public void UpdateCheckpoint(Vector2 pos)
    {
        checkpointPos = pos;
        runs.Clear();
        currentRun.Clear();
    }

    public void Die()
    {
        deathCounter++;

        if (deathText != null)
            deathText.text = "Death: " + deathCounter;

        if (bossHealth != null)
            bossHealth.ResetHealth();

        runs.Add(new List<Frame>(currentRun));

        SpawnClones();

        currentRun.Clear();

        StartCoroutine(Respawn(0.5f));
    }

    void SpawnClones()
    {

        foreach (GameObject clone in activeClones)
            Destroy(clone);
        activeClones.Clear();

        foreach (var run in runs)
        {
            GameObject clone = Instantiate(clonePrefab, checkpointPos, Quaternion.identity);
            CloneController cc = clone.GetComponent<CloneController>();
            if (cc != null)
                cc.Init(run);
            activeClones.Add(clone);
        }
    }

    IEnumerator Respawn(float delay)
    {
        playerRb.simulated = false;
        playerRb.linearVelocity = Vector2.zero;
        transform.localScale = Vector3.zero;

        yield return new WaitForSeconds(delay);

        transform.position = checkpointPos;
        transform.localScale = new Vector3(2.247446f, 2.472268f, 1);
        playerRb.simulated = true;
    }
}