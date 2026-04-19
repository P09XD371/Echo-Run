using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{
    [SerializeField] private float typewriterSpeed = 50f;

    private bool skipRequested;

    public void RequestSkip()
    {
        skipRequested = true;
    }

    public IEnumerator Run(string textToType, TMP_Text textLabel)
    {
        textLabel.text = string.Empty;

        float t = 0;
        int charIndex = 0;

        skipRequested = false;

        while (charIndex < textToType.Length)
        {
            // если нажали skip — сразу показать весь текст
            if (skipRequested)
            {
                textLabel.text = textToType;
                yield break;
            }

            t += Time.deltaTime * typewriterSpeed;
            charIndex = Mathf.FloorToInt(t);
            charIndex = Mathf.Clamp(charIndex, 0, textToType.Length);

            textLabel.text = textToType.Substring(0, charIndex);
            yield return null;
        }

        textLabel.text = textToType;
    }
}