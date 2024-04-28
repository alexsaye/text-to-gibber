using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Intonation", menuName = "Voice/Intonation")]
public class Intonation : ScriptableObject
{
    public enum Style
    {
        Statement,
        Question,
        Exclamation,
        Command,
        // TODO: more, maybe with emotions: HappyExclamation, SadStatement, etc.
    }

    [Serializable]
    private class Inflection
    {
        public Style Style;

        [Header("Pitch")]

        public AnimationCurve PitchOverTime;
        public float PitchWobble = 0.05f;

        [Header("Volume")]

        public AnimationCurve VolumeOverTime;

        public float VolumeWobble = 0.05f;
    }

    [SerializeField]
    private float pitch = 1f;

    [SerializeField]
    private float volume = 1f;

    [SerializeField]
    [Tooltip("Inflections for each intonation style.")]
    private List<Inflection> inflections = new();
    private readonly Lazy<IReadOnlyDictionary<Style, Inflection>> lookup;

    public Intonation()
    {
        lookup = new Lazy<IReadOnlyDictionary<Style, Inflection>>(() => inflections.ToDictionary(c => c.Style));
    }

    private Inflection Find(Style style)
    {
        if (!lookup.Value.TryGetValue(style, out var curve))
        {
            Debug.LogWarning($"No Pattern found for '{style}'");
        }
        return curve;
    }

    public void Inflect(AudioSource source, Style style, float progress)
    {
        Debug.Log($"Inflecting '{style}' at {progress * 100}% progress");
        var inflection = Find(style);
        if (inflection != null)
        {
            var pitchAtTime = inflection.PitchOverTime?.Evaluate(progress) ?? 1f;
            var pitchWobble = UnityEngine.Random.Range(-inflection.PitchWobble, inflection.PitchWobble);
            source.pitch = pitch * pitchAtTime + pitchWobble;

            var volumeAtTime = inflection.VolumeOverTime?.Evaluate(progress) ?? 1f;
            var volumeWobble = UnityEngine.Random.Range(-inflection.VolumeWobble, inflection.VolumeWobble);
            source.volume = volume * volumeAtTime + volumeWobble;
        }
        else
        {
            source.pitch = pitch;
            source.volume = volume;
        }
    }
}
