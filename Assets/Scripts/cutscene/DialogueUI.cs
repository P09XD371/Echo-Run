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

    [Header("Character Images")]
    [SerializeField] private GameObject imagePlayer;   // Декстер
    [SerializeField] private GameObject imageBoss;     // Искандер
    [SerializeField] private GameObject imageBoss2;    // Марк

    [Header("Dialogue Settings")]
    [SerializeField] private bool playDialogueOnStart = false;
    [SerializeField] private DialogueObject testDialogue;
    [SerializeField] private PlayableDirector timeline;
    [SerializeField] private string nextSceneName;

    [Header("Gameplay UI")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject timer;
    [SerializeField] private GameObject coinCounter;
    [SerializeField] private GameObject coins;
    [SerializeField] private GameObject deathText;

    private TypewriterEffect typewriterEffect;

    private void Start()
    {
        typewriterEffect = GetComponent<TypewriterEffect>();

        // Скрываем HUD только на первом уровне
        if (SceneManager.GetActiveScene().name == "Level1")
        {
            if (timer != null) timer.SetActive(false);
            if (coinCounter != null) coinCounter.SetActive(false);
            if (coins != null) coins.SetActive(false);
            if (deathText != null) deathText.SetActive(false);
        }

        HideAllImages();
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

            // действия типа *Марк улыбается*
            if (isActionText)
            {
                HideAllImages();

                nameLabel.text = "";
                textLabel.alignment = TextAlignmentOptions.Center;

                finalText = "<i>" + dialogue + "</i>";
            }
            else
            {
                textLabel.alignment = TextAlignmentOptions.Left;

                // Декстер
                if (dialogue.StartsWith("ГГ:"))
                {
                    nameLabel.text = "Декстер";
                    finalText = dialogue.Replace("ГГ:", "").Trim();

                    imagePlayer.SetActive(true);
                    imageBoss.SetActive(false);
                    imageBoss2.SetActive(false);
                }

                // Искандер
                else if (dialogue.StartsWith("НПС:"))
                {
                    nameLabel.text = "Искандер";
                    finalText = dialogue.Replace("НПС:", "").Trim();

                    imagePlayer.SetActive(false);
                    imageBoss.SetActive(true);
                    imageBoss2.SetActive(false);
                }

                // Марк
                else if (dialogue.StartsWith("НПС2:"))
                {
                    nameLabel.text = "Марк";
                    finalText = dialogue.Replace("НПС2:", "").Trim();

                    imagePlayer.SetActive(false);
                    imageBoss.SetActive(false);
                    imageBoss2.SetActive(true);
                }
                else
                {
                    nameLabel.text = "";
                    HideAllImages();
                }
            }

            Coroutine typing = StartCoroutine(
                typewriterEffect.Run(finalText, textLabel)
            );

            bool lineFinished = false;

            while (!lineFinished)
            {
                bool pressed =
                    (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) ||
                    (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame);

                if (pressed)
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
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }

    private void HideAllImages()
    {
        if (imagePlayer != null) imagePlayer.SetActive(false);
        if (imageBoss != null) imageBoss.SetActive(false);
        if (imageBoss2 != null) imageBoss2.SetActive(false);
    }

    public void CloseDialogueBox()
    {
        dialogueBox.SetActive(false);

        textLabel.text = "";
        nameLabel.text = "";

        HideAllImages();
    }

    public void StartDialogueManually()
    {
        if (testDialogue != null)
        {
            ShowDialogue(testDialogue);
        }
    }
}