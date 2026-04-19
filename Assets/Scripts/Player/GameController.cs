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
    public InputAction reset;

    [Header("UI")]
    public TMP_Text counterText;
    public TMP_Text deathText;

    public CutsceneController cutscene;
    private bool firstDeathCutscenePlayed = false;
    public bool isInCutscene = false;

    private bool isDead = false;

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

        if (fire != null) fire.Enable();
        if (reset.triggered)
        {
            Debug.Log("R нажата");
        }
        if (reset != null) reset.Enable();
    }

    void Start()
    {
        checkpointPos = transform.position;
        playerRb.simulated = true;
    }

    void Update()
    {
        if (reset != null && reset.triggered)
        {
            ForceReset();
        }
    }

    void FixedUpdate()
    {
        RecordFrame();
    }

    void RecordFrame()
    {
        if (isInCutscene || isDead) return;

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
        isDead = true;
        playerRb.linearVelocity = Vector2.zero;
        playerRb.simulated = false;

        deathCounter++;

        if (deathText != null)
            deathText.text = "Death: " + deathCounter;

        if (bossHealth != null)
            bossHealth.ResetHealth();

        runs.Add(new List<Frame>(currentRun));

        if (firstDeathCutscenePlayed)
        {
            ClearClones();
            SpawnClones();
        }

        currentRun.Clear();

        if (!firstDeathCutscenePlayed)
        {
            firstDeathCutscenePlayed = true;
            StartCoroutine(FirstDeathRoutine());
        }
        else
        {
            StartCoroutine(Respawn(0.5f));
        }
    }

    void SpawnClones()
    {
        foreach (var run in runs)
        {
            GameObject clone = Instantiate(clonePrefab, checkpointPos, Quaternion.identity);

            CloneController cc = clone.GetComponent<CloneController>();
            if (cc != null)
                cc.Init(run);

            activeClones.Add(clone);
        }
    }

    public IEnumerator Respawn(float delay)
    {
        playerRb.simulated = false;
        playerRb.linearVelocity = Vector2.zero;
        transform.localScale = Vector3.zero;

        yield return new WaitForSeconds(delay);

        transform.position = checkpointPos;
        transform.localScale = new Vector3(2.247446f, 2.472268f, 1);
        playerRb.simulated = true;

        var pc = GetComponent<PlayerController>();
        var pa = GetComponent<PlayerAttack>();

        pc.enabled = true;
        pa.enabled = true;

        isDead = false;
    }

    IEnumerator FirstDeathRoutine()
    {
        if (cutscene != null)
            cutscene.PlayCutscene();

        yield return new WaitForSeconds((float)cutscene.director.duration);

        ClearClones();
        SpawnClones();

        yield return StartCoroutine(Respawn(0.5f));
    }

    void ClearClones()
    {
        foreach (GameObject clone in activeClones)
        {
            if (clone != null)
                Destroy(clone);
        }

        activeClones.Clear();
    }

    void ForceReset()
    {
        StopAllCoroutines();

        isDead = false;

        ClearClones();

        runs.Clear();
        currentRun.Clear();

        if (bossHealth != null)
            bossHealth.ResetHealth();

        transform.position = checkpointPos;
        playerRb.linearVelocity = Vector2.zero;
        playerRb.simulated = true;

        var pc = GetComponent<PlayerController>();
        var pa = GetComponent<PlayerAttack>();

        pc.enabled = true;
        pa.enabled = true;

        transform.localScale = new Vector3(2.247446f, 2.472268f, 1);
    }

}