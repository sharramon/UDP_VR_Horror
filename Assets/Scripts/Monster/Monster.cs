using UnityEngine;
using System.Collections;

public abstract class Monster : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] protected AudioSource audioSource;

    [Header("Optional Movement")]
    [SerializeField] protected float moveSpeed = 2f;

    // Called by SequenceController
    public virtual IEnumerator Speak(AudioClip line)
    {
        if (audioSource == null || line == null)
            yield break;

        audioSource.clip = line;
        audioSource.Play();

        // Let SequenceController wait for completion
        yield return new WaitWhile(() => audioSource.isPlaying);
    }

    public void PlayLoop(AudioClip clip, float volume = 1f)
    {
        if (audioSource == null || clip == null) return;

        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.volume = volume;
        audioSource.Play();
    }

    public void StopLoop()
    {
        if (audioSource == null) return;

        audioSource.loop = false;
        audioSource.Stop();
    }

    public void PlayOneShot(AudioClip clip, float volume = 1f)
    {
        if (audioSource == null || clip == null) return;
        audioSource.PlayOneShot(clip, volume);
    }

    public virtual IEnumerator WalkTo(Vector3 targetPos)
    {
        // Default: simple linear walk (subclasses override for animations)
        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }
    }

    public virtual IEnumerator Attack()
    {
        // Subclasses override with animations/fx
        yield return null;
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
}
