using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Accent", menuName = "Voice/Accent")]
public class Accent : ScriptableObject
{
    [Serializable]
    private class Phoneme
    {
        public char Character = '?';
        public AudioClip Clip;
    }

    [SerializeField]
    [Tooltip("Phonemes for each character.")]
    private List<Phoneme> phonemes = new();
    private readonly Lazy<IReadOnlyDictionary<char, Phoneme>> lookup;

    public Accent()
    {
        lookup = new Lazy<IReadOnlyDictionary<char, Phoneme>>(() => phonemes.ToDictionary(c => c.Character));
    }

    private Phoneme Find(char character)
    {
        if (!lookup.Value.TryGetValue(character, out var resource))
        {
            // Try to find the character in the opposite case.
            if (!lookup.Value.TryGetValue(char.IsLower(character) ? char.ToUpper(character) : char.ToLower(character), out resource)) {
                Debug.LogWarning($"No phoneme found for '{character}'.");
            }
        }
        return resource;
    }

    public void Pronounce(AudioSource source, char character)
    {
        var phoneme = Find(character);
        if (phoneme?.Clip != null)
        {
            Debug.Log($"Pronouncing '{character}'.");
            source.PlayOneShot(phoneme.Clip);
        }
    }
}
