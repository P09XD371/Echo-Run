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
            yield return typewriterEffect.Run(dialogue, textLabel);
            yield return new WaitUntil(() => Keyboard.current.spaceKey.wasPressedThisFrame);
        }

        CloseDialogueBox();

        // включаем UI
        timer.SetActive(true);
        coinCounter.SetActive(true);
        coins.SetActive(true);
        deathText.SetActive(true);
        // запуск катсцены
        timeline.Play();
    }
    public void CloseDialogueBox()
    {
        dialogueBox.SetActive(false);
        textLabel.text = string.Empty;
    }
}
