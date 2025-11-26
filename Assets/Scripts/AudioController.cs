using UnityEngine;

public class AudioController : MonoBehaviour
{
    private AudioSource _source;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        if (_source == null)
            _source = gameObject.AddComponent<AudioSource>();

        _source.loop = true;
    }

    public void PlayMusic(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;

        _source.clip = clip;
        _source.volume = volume;
        _source.Play();
    }

    public void StopMusic()
    {
        _source.Stop();
    }

    public bool IsPlaying => _source.isPlaying;
}
