using System.Collections;
using UnityEngine;

public class Voice : MonoBehaviour
{
    [SerializeField]
    [Tooltip("AudioSource to play the speech. Will use an attached AudioSource if not set.")]
    private AudioSource audioSource;

    [Header("Language")]

    [SerializeField]
    [Tooltip("Pronunciation to use for speaking.")]
    private Accent accent;

    [SerializeField]
    [Tooltip("Intonation to use for speaking.")]
    private Intonation intonation;

    [Header("Character")]

    [SerializeField]
    [Tooltip("Pitch to use when speaking.")]
    private float pitch = 1f;

    [SerializeField]
    [Range(0f, 1f)]
    [Tooltip("Volume to use when speaking.")]
    private float volume = 1f;

    [Header("Speed")]

    [SerializeField]
    [Tooltip("Delay between characters. (seconds)")]
    private float characterDelay = 0.1f;

    [SerializeField]
    [Tooltip("Delay between words. (seconds)")]
    private float wordDelay = 0.2f;

    private Coroutine speaking;

    [Header("Testing")]
    [SerializeField]
    private string testText = "This is a test.";

    private void Awake()
    {
        if (audioSource == null && !TryGetComponent<AudioSource>(out audioSource))
        {
            Debug.LogError("Missing AudioSource");
        }
    }

    /// <summary>
    /// Says the given text aloud. Interrupts any current speech.
    /// </summary>
    /// <param name="text">The text to say.</param>
    public void Say(string text, Intonation.Style style = Intonation.Style.Statement)
    {
        if (speaking != null)
        {
            Debug.Log($"{name} interrupted itself");
            StopCoroutine(speaking);
        }

        if (audioSource)
        {
            Debug.Log($"{name} says '{text}'");
            speaking = StartCoroutine(SayCoroutine(text, style));
        }
        else
        {
            Debug.LogError($"{name} can't say anything without an AudioSource");
        }
    }

    private IEnumerator SayCoroutine(string text, Intonation.Style style)
    {
        var step = 1f / text.Length;
        for (var index = 0; index < text.Length; index++)
        {
            var character = text[index];
            if (char.IsWhiteSpace(character) || char.IsPunctuation(character))
            {
                yield return new WaitForSeconds(wordDelay);
            }
            else
            {
                audioSource.pitch = pitch;
                audioSource.volume = volume;
                intonation.Inflect(audioSource, style, index * step);
                accent.Pronounce(audioSource, character);
                yield return new WaitForSeconds(characterDelay);
            }
        }
    }

    [ContextMenu("Say Test Statement")]
    private void SayStatement()
    {
        Say(testText, Intonation.Style.Statement);
    }

    [ContextMenu("Say Test Question")]
    private void SayQuestion()
    {
        Say(testText, Intonation.Style.Question);
    }

    [ContextMenu("Say Test Exclamation")]
    private void SayExclamation()
    {
        Say(testText, Intonation.Style.Exclamation);
    }

    [ContextMenu("Say Test Command")]
    private void SayCommand()
    {
        Say(testText, Intonation.Style.Command);
    }


    /// <summary>
    /// Stops saying the current text.
    /// </summary>
    [ContextMenu("Shut Up")]
    public void ShutUp()
    {
        if (speaking != null)
        {
            Debug.Log($"{name} is no longer speaking");
            StopCoroutine(speaking);
        }
        else
        {
            Debug.Log($"{name} is already not speaking");
        }
    }
}
