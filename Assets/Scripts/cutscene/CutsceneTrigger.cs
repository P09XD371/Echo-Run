using UnityEngine;

public class CutsceneTrigger : MonoBehaviour
{
    [SerializeField] private Cutscene3Controller cutscene;

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