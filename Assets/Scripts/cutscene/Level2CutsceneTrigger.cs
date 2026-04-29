using UnityEngine;

public class Level2CutsceneTrigger : MonoBehaviour
{
    [SerializeField] private Level2CutsceneController cutsceneController;
    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (activated) return;

        if (collision.CompareTag("Player"))
        {
            activated = true;

            cutsceneController.PlayCutscene();
        }
    }
}