using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class GameController : MonoBehaviour
{
    Vector2 checkpointPos;

    Rigidbody2D playerRb;
    SpriteRenderer sr;

    public GameObject clonePrefab;

    List<Frame> currentRun = new List<Frame>();
    List<List<Frame>> runs = new List<List<Frame>>();

    List<GameObject> activeClones = new List<GameObject>();

    private int coinCounter = 0;
    public TMP_Text counterText;

    private int deathCounter = 0;
    public TMP_Text deathText;

    void Awake()
    {
        playerRb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
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
        Frame frame = new Frame(transform.position, sr.flipX);
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

        runs.Add(new List<Frame>(currentRun));

        SpawnClones();

        currentRun.Clear();

        StartCoroutine(Respawn(0.5f));
    }

    void SpawnClones()
    {
        foreach (GameObject clone in activeClones)
        {
            Destroy(clone);
        }

        activeClones.Clear();

        foreach (var run in runs)
        {
            GameObject clone = Instantiate(clonePrefab, checkpointPos, Quaternion.identity);

            CloneController cc = clone.GetComponent<CloneController>();
            cc.Init(run);

            activeClones.Add(clone);
        }
    }

    IEnumerator Respawn(float delay)
    {
        playerRb.simulated = false;
        playerRb.velocity = Vector2.zero;

        transform.localScale = Vector3.zero;

        yield return new WaitForSeconds(delay);

        transform.position = checkpointPos;

        transform.localScale = new Vector3(2.247446f, 2.472268f, 1);

        playerRb.simulated = true;
    }
}