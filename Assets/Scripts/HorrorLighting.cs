using UnityEngine;
using DG.Tweening;
using System.Collections;
using Unity.Collections.LowLevel.Unsafe;

public class HorrorLighting : MonoBehaviour
{
    [SerializeField] private Light m_light;
    [SerializeField] private AudioSource m_audioSource;

    public void SetLightColor(Color setColor)
    {
        if (m_light == null)
        {
            Debug.Log("Light isn't set");
            return;
        }

        m_light.color = setColor;
    }

    public void SetLightIntensity(float setIntensity)
    {
        if (m_light == null)
        {
            Debug.Log("Light isn't set");
            return;
        }

        if (setIntensity < 0)
            setIntensity = 0;

        m_light.intensity = setIntensity;
    }

    public void SetLightSoundOn(bool isOn, AudioClip audioClip)
    {
        if(m_audioSource == null)
        {
            Debug.Log("Please set audio source");
            //TODO : I might want to add a spatialized audiosource automatically later
            return;
        }

        m_audioSource.clip = audioClip;

        if (isOn)
        {
            if (!m_audioSource.isPlaying)
                m_audioSource.Play();
        }
        else
        {
            if (m_audioSource.isPlaying)
                m_audioSource.Stop();
        }
    }

    public IEnumerator FadeIntensityRoutine(float target, float time, Ease ease = Ease.Linear)
    {
        if (m_light == null)
            yield break;

        Tween t = m_light
            .DOIntensity(target, time)
            .SetEase(ease);

        yield return t.WaitForCompletion();
    }

    public IEnumerator FadeColorRoutine(Color target, float time, Ease ease = Ease.Linear)
    {
        if (m_light == null)
            yield break;

        Tween t = m_light
            .DOColor(target, time)
            .SetEase(ease);

        yield return t.WaitForCompletion();
    }

    public IEnumerator FlickerRoutine(float duration, float minIntensity = 0f, float maxIntensity = 1f)
    {
        if (m_light == null)
            yield break;

        float t = 0f;

        while (t < duration)
        {
            m_light.intensity = Random.Range(minIntensity, maxIntensity);
            t += Time.deltaTime;
            yield return null;
        }

        m_light.intensity = maxIntensity;
    }

    public void FadeIntensity(float target, float time, Ease ease = Ease.Linear)
    {
        if (m_light == null) return;
        m_light.DOIntensity(target, time).SetEase(ease);
    }

    public void FadeColor(Color target, float time, Ease ease = Ease.Linear)
    {
        if (m_light == null) return;
        m_light.DOColor(target, time).SetEase(ease);
    }
}
