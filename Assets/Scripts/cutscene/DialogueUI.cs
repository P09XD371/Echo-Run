using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using TMPro;
using UnityEngine.SceneManagement;

public class DialogUi : MonoBehaviour
{
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text nameLabel;
    [SerializeField] private TMP_Text textLabel;

    [SerializeField] private GameObject imagePlayer;
    [SerializeField] private GameObject imageBoss;

    [SerializeField] private bool playDialogueOnStart = false;
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

        imagePlayer.SetActive(false);
        imageBoss.SetActive(false);

        CloseDialogueBox();

        if (playDialogueOnStart && testDialogue != null)
        {
            ShowDialogue(testDialogue);
        }
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
            string finalText = dialogue;
            bool isActionText = dialogue.StartsWith("*") && dialogue.EndsWith("*");

            if (isActionText)
            {
                imagePlayer.SetActive(false);
                imageBoss.SetActive(false);

                nameLabel.text = "";

                textLabel.alignment = TextAlignmentOptions.Center;
                finalText = "<i>" + dialogue + "</i>";
            }
            else
            {
                textLabel.alignment = TextAlignmentOptions.Left;

                if (dialogue.StartsWith("ГГ:"))
                {
                    nameLabel.text = "Декстер";
                    finalText = dialogue.Replace("ГГ:", "").Trim();

                    imagePlayer.SetActive(true);
                    imageBoss.SetActive(false);
                }
                else if (dialogue.StartsWith("НПС:"))
                {
                    nameLabel.text = "Искандер";
                    finalText = dialogue.Replace("НПС:", "").Trim();

                    imagePlayer.SetActive(false);
                    imageBoss.SetActive(true);
                }
                else
                {
                    nameLabel.text = "";

                    imagePlayer.SetActive(false);
                    imageBoss.SetActive(false);
                }
            }

            Coroutine typing = StartCoroutine(
                typewriterEffect.Run(finalText, textLabel)
            );

            bool lineFinished = false;

            while (!lineFinished)
            {
                if (Keyboard.current.spaceKey.wasPressedThisFrame)
                {
                    if (typing != null)
                    {
                        typewriterEffect.RequestSkip();
                        yield return typing;
                        typing = null;
                    }
                    else
                    {
                        lineFinished = true;
                    }
                }

                yield return null;
            }
        }

        CloseDialogueBox();

        // Если это первый диалог → запускаем timeline
        if (timeline != null)
        {
            if (player != null) player.SetActive(true);

            if (timer != null) timer.SetActive(true);
            if (coinCounter != null) coinCounter.SetActive(true);
            if (coins != null) coins.SetActive(true);
            if (deathText != null) deathText.SetActive(true);

            timeline.Play();
        }
        else
        {
            // Если это финальный диалог → переходим на Level2
            SceneManager.LoadScene("Level2");
        }
    }

    public void CloseDialogueBox()
    {
        dialogueBox.SetActive(false);
        textLabel.text = string.Empty;
        nameLabel.text = string.Empty;

        if (imagePlayer != null) imagePlayer.SetActive(false);
        if (imageBoss != null) imageBoss.SetActive(false);
    }

    public void StartDialogueManually()
    {
        if (testDialogue != null)
        {
            ShowDialogue(testDialogue);
        }
    }
}