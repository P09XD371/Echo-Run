using UnityEngine;
using System;

public class EnemyAttackHitbox : MonoBehaviour
{
    public event Action<PlayerController> OnPlayerEnter;
    public event Action<PlayerController> OnPlayerExit;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();

            if (pc != null)
            {
                OnPlayerEnter?.Invoke(pc);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();

            if (pc != null)
            {
                OnPlayerExit?.Invoke(pc);
            }
        }
    }
}