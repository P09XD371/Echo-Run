using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class Level2CutsceneController : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private DialogUi dialogueUi;

    public void PlayCutscene()
    {
        StartCoroutine(PlayRoutine());
    }

    private IEnumerator PlayRoutine()
    {
        // отключаем управление
        playerController.enabled = false;

        if (playerAttack != null)
            playerAttack.enabled = false;

        Debug.Log("Timeline started");

        director.Play();

        // ждем полного завершения timeline
        while (director.state == PlayState.Playing)
        {
            yield return null;
        }

        Debug.Log("Timeline finished");

        // запускаем диалог
        if (dialogueUi != null)
        {
            dialogueUi.StartDialogueManually();
        }
    }
}