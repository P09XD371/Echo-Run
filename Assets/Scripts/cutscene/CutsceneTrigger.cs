using UnityEngine;

public class CutsceneTrigger : MonoBehaviour
{
    [SerializeField] private CutsceneController cutscene;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered) return;

        if (collision.CompareTag("Player"))
        {
            triggered = true;

            if (cutscene != null)
            {
                cutscene.PlayCutscene();
            }

            gameObject.SetActive(false);
        }
    }
}