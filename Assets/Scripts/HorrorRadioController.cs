using UnityEngine;
using System.Collections;

public class HorrorRadioController : MonoBehaviour
{
    [Header("Glow / Emission")]
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private Color emissionColor = Color.white;

    [Header("Audio")]
    [SerializeField] private AudioSource radioAudio;

    private Material mat;
    private Coroutine activeRoutine;

    void Awake()
    {
        if (targetRenderer != null)
            mat = targetRenderer.material;
    }

    public void PlayRadioClip(AudioClip clip)
    {
        if (clip == null || radioAudio == null)
            return;

        // If something is already playing, stop its routine
        if (activeRoutine != null)
        {
            StopCoroutine(activeRoutine);
            StopRadioInternal();
        }

        activeRoutine = StartCoroutine(PlayRoutine(clip));
    }

    public void StopRadio()
    {
        if (activeRoutine != null)
        {
            StopCoroutine(activeRoutine);
            activeRoutine = null;
        }

        StopRadioInternal();
    }

    public IEnumerator PlayRoutine(AudioClip clip, float volume = 1f)
    {
        // Turn glow ON
        EnableGlow(true);

        // Play audio
        radioAudio.clip = clip;
        radioAudio.Play();
        radioAudio.volume = volume;

        // Wait exactly for the clip length
        yield return new WaitForSeconds(clip.length);

        // Turn off after the clip finishes
        StopRadioInternal();
        activeRoutine = null;
    }

    private void StopRadioInternal()
    {
        if (radioAudio.isPlaying)
            radioAudio.Stop();

        EnableGlow(false);
    }

    private void EnableGlow(bool on)
    {
        if (mat == null)
            return;

        if (on)
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", emissionColor);
        }
        else
        {
            mat.SetColor("_EmissionColor", Color.black);
            mat.DisableKeyword("_EMISSION");
        }
    }
}
