using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using TMPro;

public class DialogUi : MonoBehaviour
{

    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text textLabel;
    [SerializeField] private DialogueObject testDialogue;

    [SerializeField] private PlayableDirector timeline;

    [SerializeField] private GameObject player;

    [SerializeField] private GameObject timer;
    [SerializeField] private GameObject coinCounter;
    [SerializeField] private GameObject coins;
    [SerializeField] private GameObject deathText;

    private TypewriterEffect typewriterEffect;

    private void Start()
    {
        typewriterEffect = GetComponent<TypewriterEffect>();

        timer.SetActive(false);
        coinCounter.SetActive(false);
        coins.SetActive(false);
        deathText.SetActive(false);

        player.SetActive(false);

        CloseDialogueBox();
        ShowDialogue(testDialogue);
    }
    public void ShowDialogue(DialogueObject dialogueObject)
    {
        dialogueBox.SetActive(true);
        StartCoroutine(StepThroughDialogue(dialogueObject));
    }
    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject)
    {
        foreach (string dialogue in dialogueObject.Dialogues)
        {
            Coroutine typing = StartCoroutine(typewriterEffect.Run(dialogue, textLabel));

            bool lineFinished = false;

            while (!lineFinished)
            {
                if (Keyboard.current.spaceKey.wasPressedThisFrame)
                {
                    // если текст ещё печатается → скипаем
                    if (typing != null)
                    {
                        typewriterEffect.RequestSkip();
                        yield return typing; // дождаться завершения
                        typing = null;
                    }
                    else
                    {
                        // если уже допечатан → идём дальше
                        lineFinished = true;
                    }
                }

                yield return null;
            }
        }

        CloseDialogueBox();

        player.SetActive(true);

        timer.SetActive(true);
        coinCounter.SetActive(true);
        coins.SetActive(true);
        deathText.SetActive(true);

        timeline.Play();
    }
    public void CloseDialogueBox()
    {
        dialogueBox.SetActive(false);
        textLabel.text = string.Empty;
    }
}
