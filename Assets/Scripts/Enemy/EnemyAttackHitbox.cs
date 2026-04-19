using UnityEngine;
using System;

public class EnemyAttackHitbox : MonoBehaviour
{
    public event Action<PlayerController> OnPlayerHit;

    bool playerHitAlready = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController pc = other.GetComponent<PlayerController>();

        if (pc != null)
        {
            OnPlayerHit?.Invoke(pc);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController pc = other.GetComponent<PlayerController>();

        if (pc != null)
        {
            OnPlayerHit?.Invoke(pc);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerHitAlready = false;
    }

    public bool CanDealDamage()
    {
        if (playerHitAlready) return false;

        playerHitAlready = true;
        return true;
    }
}