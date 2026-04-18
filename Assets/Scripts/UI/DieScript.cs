using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class DieScript : MonoBehaviour
{
    public static bool firstDeathDone = false;

    [SerializeField] private PlayableDirector cutscene2;

    [SerializeField] private GameObject player;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Rigidbody2D playerRb;

    [SerializeField] private GameController gameController;

    public void OnPlayerDeath()
    {
        if (!firstDeathDone)
        {
            firstDeathDone = true;

            // ❌ выключаем управление
            playerController.enabled = false;

            // ❌ выключаем физику
            playerRb.simulated = false;

            // 🎬 запускаем катсцену
            cutscene2.Play();

            StartCoroutine(WaitForCutscene());
        }
        else
        {
            // обычная смерть
            StartCoroutine(gameController.Respawn(0.5f));
        }
    }

    private IEnumerator WaitForCutscene()
    {
        yield return new WaitForSeconds((float)cutscene2.duration);

        // ✅ включаем обратно
        playerController.enabled = true;
        playerRb.simulated = true;

        // 🔄 респавн после катсцены
        StartCoroutine(gameController.Respawn(0.5f));
    }
}