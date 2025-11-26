using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class HorrorLightingController : MonoBehaviour
{
    [Header("Lighting Groups")]
    [SerializeField] private List<HorrorLighting> m_playerLightingList = new List<HorrorLighting>();
    [SerializeField] private List<HorrorLighting> m_npcLightingList = new List<HorrorLighting>(); //0 : Dad Light, 1 : Daughter Light

    public void SetAllLightColor(Color setColor)
    {
        foreach (var l in m_playerLightingList) l.SetLightColor(setColor);
        foreach (var l in m_npcLightingList) l.SetLightColor(setColor);
    }

    public void SetAllLightIntensity(float setIntensity)
    {
        foreach (var l in m_playerLightingList) l.SetLightIntensity(setIntensity);
        foreach (var l in m_npcLightingList) l.SetLightIntensity(setIntensity);
    }

    public void SetPlayerLightIntensity(float setIntensity)
    {
        foreach (var l in m_playerLightingList) l.SetLightIntensity(setIntensity);
    }
    public void SetAllPlayerLightColor(Color color)
    {
        foreach (var l in m_playerLightingList) l.SetLightColor(color);
    }

    public void SetPlayerLightSound(bool isOn, AudioClip clip)
    {
        foreach (var l in m_playerLightingList) l.SetLightSoundOn(isOn, clip);
    }

    /// <summary>
    /// Setting light intesnity for NPCs
    /// 0 : Dad
    /// 1 : Daughter
    /// </summary>
    /// <param name="ind"></param>
    /// <param name="setIntensity"></param>
    public void SetNPCLightIntensity(int ind, float setIntensity)
    {
        if (ind > m_npcLightingList.Count)
        {
            Debug.Log($"NPC light ind too large at {ind} with count {m_npcLightingList.Count}");
            return;
        }

        m_npcLightingList[ind].SetLightIntensity(setIntensity);
    }

    public void SetNPCLightColor(int ind, Color color)
    {
        if (ind > m_npcLightingList.Count)
        {
            Debug.Log($"NPC light ind too large at {ind} with count {m_npcLightingList.Count}");
            return;
        }

        m_npcLightingList[ind].SetLightColor(color);
    }

    private IEnumerable<HorrorLighting> AllLights()
    {
        foreach (var l in m_playerLightingList) yield return l;
        foreach (var l in m_npcLightingList) yield return l;
    }

    public IEnumerator FlickerRoutine(float duration = 2f)
    {
        float t = 0f;

        while (t < duration)
        {
            float intensity = Random.value > 0.5f ? 1f : 0f;

            foreach (var l in AllLights())
                l.SetLightIntensity(intensity);

            t += Time.deltaTime;
            yield return null;
        }

        // restore default brightness
        foreach (var l in AllLights())
            l.SetLightIntensity(1f);
    }

    private IEnumerable<HorrorLighting> PlayerLights()
    {
        foreach (var l in m_playerLightingList) yield return l;
    }

    public IEnumerator PlayerFlickerRoutine(float duration = 2f)
    {
        float t = 0f;

        while (t < duration)
        {
            float intensity = Random.value > 0.5f ? 1f : 0f;

            foreach (var l in PlayerLights())
                l.SetLightIntensity(intensity);

            t += Time.deltaTime;
            yield return null;
        }

        // restore default brightness
        foreach (var l in PlayerLights())
            l.SetLightIntensity(1f);
    }

    public IEnumerator SetAllLightIntensityBlackoutRoutine()
    {
        foreach (var l in AllLights())
            l.SetLightIntensity(0f);

        yield return null;
    }

    public IEnumerator RedModeRoutine()
    {
        foreach (var l in AllLights())
        {
            l.SetLightColor(Color.red);
            l.SetLightIntensity(1f);
        }

        yield return null;
    }

    public IEnumerator FadeAllIntensityRoutine(float targetIntensity, float time, Ease ease = Ease.Linear)
    {
        List<Tween> tweens = new List<Tween>();

        foreach (var l in AllLights())
        {
            if (l == null) continue;

            // Tweening intensity through Light component
            Light unityLight = l.GetComponent<Light>();
            if (unityLight != null)
            {
                tweens.Add(unityLight
                    .DOIntensity(targetIntensity, time)
                    .SetEase(ease));
            }
        }

        // Wait for all tweens to finish
        foreach (var t in tweens)
            yield return t.WaitForCompletion();
    }

    public IEnumerator FadeAllColorRoutine(Color targetColor, float time, Ease ease = Ease.Linear)
    {
        List<Tween> tweens = new List<Tween>();

        foreach (var l in AllLights())
        {
            Light unityLight = l.GetComponent<Light>();
            if (unityLight != null)
            {
                tweens.Add(unityLight
                    .DOColor(targetColor, time)
                    .SetEase(ease));
            }
        }

        foreach (var t in tweens)
            yield return t.WaitForCompletion();
    }
}
