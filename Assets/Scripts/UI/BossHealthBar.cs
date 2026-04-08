using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    public EnemyHealth enemy;  // Ссылка на врага
    public Image healthFill;   // Image с Fill для HP

    void Update()
    {
        if (enemy != null)
        {
            float fillAmount = (float)enemy.currentHealth / enemy.maxHealth;
            healthFill.fillAmount = Mathf.Clamp01(fillAmount);
        }
        else
        {
            // Если враг уничтожен, скрываем панель
            gameObject.SetActive(false);
        }
    }
}