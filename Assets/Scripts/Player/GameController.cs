using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    Vector2 checkpointPos;
    Rigidbody2D playerRb;

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        checkpointPos = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            Die();
        }
    }

    public void UpdateCheckpoint(Vector2 pos)
    {
               checkpointPos = pos;
    }

    void Die()
    {
        StartCoroutine(Respawn(0.5f));
    }
    
    IEnumerator Respawn(float delay)
    {
        playerRb.simulated = false;
        playerRb.velocity = new Vector2(0, 0);
        transform.localScale = new Vector3(0, 0, 0);
        yield return new WaitForSeconds(delay);
        transform.position = checkpointPos;
        transform.localScale = new Vector3((float)2.247446, (float)2.472268, 1);
        playerRb.simulated = true;
    }

}
