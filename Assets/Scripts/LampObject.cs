using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class LampObject : MonoBehaviour
{
    [SerializeField] private HeartRateMonitor heartRateMonitor;
    public int minHeartRate = 80;
    public int maxHeartRate = 150;
    public int minSpeedMult = 1;
    public int maxSpeedMult = 3;

    [Header("Target Material")]
    [SerializeField] private Renderer targetRenderer;

    [Header("Lights")]
    [SerializeField] private List<Light> lights;

    [Header("Light Intensity Range")]
    public float minIntensity = 0f;
    public float maxIntensity = 2f;

    [Header("Timings")]
    public float rampUpTime = 0.3f;
    public float rampDownTime = 0.3f;
    public float emissionOffWindow = 0.1f;

    [Header("Speed Control")]
    public float speedMultiplier = 1f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip heartBeat;
    public float volume = 0.5f;

    private Material targetMaterial;
    private Coroutine beatRoutine;
    private bool stopRequested = false;



    void Awake()
    {
        if (targetRenderer != null)
        {
            targetMaterial = targetRenderer.material;
        }

        if (heartRateMonitor == null)
            heartRateMonitor = FindFirstObjectByType<HeartRateMonitor>();

        heartRateMonitor.HeartRateUpdated += OnHeartRateUpdate;
    }

    private void Start()
    {
        StartLampBeat();
    }

    private void OnHeartRateUpdate(int heartRate)
    {
        if (heartRate < minHeartRate)
            heartRate = minHeartRate;

        if (heartRate > maxHeartRate)
            heartRate = maxHeartRate;

        float normalized = (float)(heartRate - minHeartRate) / (maxHeartRate - minHeartRate);
        speedMultiplier = Mathf.Lerp(minSpeedMult, maxSpeedMult, normalized);
    }

    public void StartLampBeat()
    {
        if (beatRoutine == null)
            beatRoutine = StartCoroutine(BeatLightsCoroutine());
    }

    public void StopLampBeat()
    {
        if (beatRoutine != null)
        {
            StopCoroutine(beatRoutine);
            beatRoutine = null;
        }

        // Reset everything
        SetAllLights(minIntensity);
        SetEmission(false);
    }

    private IEnumerator BeatLightsCoroutine()
    {
        float scaledUp = rampUpTime / speedMultiplier;
        float scaledDown = rampDownTime / speedMultiplier;

        // PLAY AUDIO ONCE AT START OF CYCLE
        if (audioSource != null && heartBeat != null)
        {
            audioSource.pitch = speedMultiplier;
            audioSource.PlayOneShot(heartBeat, volume);
        }

        //
        // RAMP UP
        //
        float t = 0f;
        while (t < scaledUp)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / scaledUp);

            // Emission OFF first 1/3 of ramp up
            if (lerp < 0.33f) SetEmission(false);
            else SetEmission(true);

            SetAllLights(Mathf.Lerp(minIntensity, maxIntensity, lerp));
            yield return null;
        }

        //
        // RAMP DOWN
        //
        t = 0f;
        while (t < scaledDown)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / scaledDown);

            // Emission OFF last 1/3 of ramp down
            if (lerp > 0.66f) SetEmission(false);
            else SetEmission(true);

            SetAllLights(Mathf.Lerp(maxIntensity, minIntensity, lerp));
            yield return null;
        }

        // At end of the cycle
        SetEmission(false);
        SetAllLights(minIntensity);

        //
        // DECIDE WHETHER TO REPEAT OR STOP
        //
        if (!stopRequested)
        {
            // Fire a new cycle
            beatRoutine = StartCoroutine(BeatLightsCoroutine());
        }
        else
        {
            // Fully stop
            beatRoutine = null;
        }
    }

    private void SetAllLights(float value)
    {
        foreach (var l in lights)
        {
            if (l != null)
                l.intensity = value;
        }
    }

    private void SetEmission(bool on)
    {
        if (targetMaterial == null)
            return;

        if (on)
            targetMaterial.EnableKeyword("_EMISSION");
        else
            targetMaterial.DisableKeyword("_EMISSION");
    }
}
