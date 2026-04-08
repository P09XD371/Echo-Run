using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    public EnemyHealth enemy;
    public Image healthFill;   
    void Update()
    {
        if (enemy != null)
        {
            float fillAmount = (float)enemy.currentHealth / enemy.maxHealth;
            healthFill.fillAmount = Mathf.Clamp01(fillAmount);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}