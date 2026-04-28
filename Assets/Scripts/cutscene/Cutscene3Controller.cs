using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class Cutscene3Controller : MonoBehaviour
{
    public PlayableDirector director;

    public PlayerController playerController;
    public PlayerAttack playerAttack;
    public Rigidbody2D playerRb;
    public GameController gameController;

    public DialogUi dialogueUi;

    public float delayBeforeLock = 2f;

    public void PlayCutscene()
    {
        StartCoroutine(PlayRoutine());
    }

    IEnumerator PlayRoutine()
    {
        // запускаем timeline сразу
        director.Play();

        // игрок может двигаться 2 секунды
        yield return new WaitForSeconds(delayBeforeLock);

        // блокируем управление
        if (gameController != null)
            gameController.isInCutscene = true;

        if (playerController != null)
            playerController.enabled = false;

        if (playerAttack != null)
            playerAttack.enabled = false;

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
            playerRb.simulated = false;
        }

        // ждём завершения timeline
        float remainingTime = (float)director.duration - delayBeforeLock;

        if (remainingTime > 0)
            yield return new WaitForSeconds(remainingTime);

        // запускаем диалог
        if (dialogueUi != null)
        {
            dialogueUi.StartDialogueManually();
        }
    }
}