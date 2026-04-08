using UnityEngine;

public class EnemyAttackHitbox : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Hit: " + other.name);

        if (other.CompareTag("Player"))
        {
            GameController gc = other.GetComponent<GameController>();

            if (gc != null)
                gc.Die();
        }
    }
}
