using UnityEngine;
using DG.Tweening;
using System.Collections;

public class HorrorDoor : MonoBehaviour
{
    [Header("Door Transform")]
    [SerializeField] private Transform doorTransform;

    [Header("Door Angles")]
    [SerializeField] private float m_closed = 0f;
    [SerializeField] private float m_halfOpen = 45f;
    [SerializeField] private float m_open = 90f;

    [Header("Audio")]
    [SerializeField] private AudioSource m_source;

    public Coroutine activeRattleRoutine;

    private void RotateTo(float angle, float time, Ease ease)
    {
        doorTransform
            .DOLocalRotate(new Vector3(0, angle, 0), time)
            .SetEase(ease);
    }

    public void CloseDoor(float t, Ease ease) => RotateTo(m_closed, t, ease);
    public void HalfOpenDoor(float t, Ease ease) => RotateTo(m_halfOpen, t, ease);
    public void OpenDoor(float t, Ease ease) => RotateTo(m_open, t, ease);

    public void PlayOneShot(AudioClip audioClip)
    {
        m_source.PlayOneShot(audioClip);
    }
    public void StartAudioLoop(AudioClip clip, float volume = 1f)
    {
        if (m_source == null || clip == null)
            return;

        m_source.loop = true;
        m_source.clip = clip;
        m_source.volume = volume;
        m_source.Play();
    }

    public void StopAudioLoop()
    {
        if (m_source == null)
            return;

        m_source.loop = false;
        m_source.Stop();
    }

    private IEnumerator RotateRoutine(float angle, float time, Ease ease)
    {
        Tween t = doorTransform
            .DOLocalRotate(new Vector3(0, angle, 0), time)
            .SetEase(ease);

        yield return t.WaitForCompletion();
    }

    public IEnumerator HalfOpenRoutine(float time, Ease ease)
    {
        yield return RotateRoutine(m_halfOpen, time, ease);
    }

    public IEnumerator OpenRoutine(float time, Ease ease)
    {
        yield return RotateRoutine(m_open, time, ease);
    }

    public IEnumerator CloseRoutine(float time, Ease ease)
    {
        yield return RotateRoutine(m_closed, time, ease);
    }

    public IEnumerator SlamRoutine(float time, Ease ease)
    {
        // You can layer a sound here too if you want
        // PlayOneShot(slamClip);

        yield return RotateRoutine(m_closed, time, ease);
    }
    public IEnumerator RattleRoutine(float strength = 5f, float duration = 6f)
    {
        Vector3 originalRot = doorTransform.localEulerAngles;

        Tween t = doorTransform.DOPunchRotation(
            new Vector3(0, strength, 0),
            duration,
            vibrato: 12,
            elasticity: 1f
        );

        yield return t.WaitForCompletion();

        // ensure it resets perfectly
        doorTransform.localEulerAngles = originalRot;
    }

    public void StartRattleLoop(float strength = 5f, float duration = 0.6f)
    {
        StopRattleLoop(); // in case one was running
        StartAudioLoop(ResourceManager.Instance.doorRattle);
        activeRattleRoutine = StartCoroutine(RattleLoop(strength, duration));
    }

    public void StopRattleLoop()
    {
        if (activeRattleRoutine != null)
            StopCoroutine(activeRattleRoutine);

        activeRattleRoutine = null;

        StopAudioLoop();
    }

    private IEnumerator RattleLoop(float strength = 5f, float duration = 0.6f)
    {
        while (true)
        {
            Tween t = doorTransform.DOPunchRotation(
                new Vector3(0, strength, 0),
                duration,
                vibrato: 12,
                elasticity: 1f
            );

            yield return t.WaitForCompletion();
            yield return null;
        }
    }
}
