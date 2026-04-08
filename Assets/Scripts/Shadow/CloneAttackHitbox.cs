using UnityEngine;

public class CloneAttackHitbox : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth boss = other.GetComponent<EnemyHealth>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
                Debug.Log(gameObject.name + " нанес урон боссу! HP: " + boss.currentHealth);
            }
            else
            {
                Debug.LogWarning("Boss без EnemyHealth: " + other.name);
            }
        }
    }
}