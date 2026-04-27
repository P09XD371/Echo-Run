using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneController : MonoBehaviour
{
    public PlayableDirector director;

    public PlayerController playerController;
    public PlayerAttack playerAttack;
    public Rigidbody2D playerRb;

    public GameController gameController;

    public GameObject dialogueBox3;

    public void PlayCutscene()
    {
        StartCoroutine(PlayRoutine());
    }

    IEnumerator PlayRoutine()
    {
        // Timeline запускается сразу
        director.Play();

        // Игрок ещё может двигаться 2 секунды
        yield return new WaitForSeconds(2f);

        // Теперь блокируем управление
        gameController.isInCutscene = true;

        playerController.enabled = false;
        playerAttack.enabled = false;

        playerRb.linearVelocity = Vector2.zero;
        playerRb.simulated = false;

        // Ждём окончания timeline
        float remainingTime = (float)director.duration - 2f;

        if (remainingTime > 0)
        {
            yield return new WaitForSeconds(remainingTime);
        }

        // Возвращаем управление
        playerRb.simulated = true;
        playerController.enabled = true;
        playerAttack.enabled = true;

        gameController.isInCutscene = false;
    }
}