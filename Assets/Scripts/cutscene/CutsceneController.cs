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

    public void PlayCutscene()
    {
        StartCoroutine(PlayRoutine());
    }

    IEnumerator PlayRoutine()
    {
        // ❌ Отключаем управление
        gameController.isInCutscene = true;

        playerController.enabled = false;
        playerAttack.enabled = false;

        playerRb.linearVelocity = Vector2.zero;
        playerRb.simulated = false;

        // ▶️ Запуск Timeline
        director.Play();

        // ⏳ Ждём окончания
        yield return new WaitForSeconds((float)director.duration);

        // ✅ Возвращаем управление
        playerRb.simulated = true;
        playerController.enabled = true;
        playerAttack.enabled = true;

        gameController.isInCutscene = false;
    }
}