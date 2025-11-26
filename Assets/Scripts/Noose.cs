using UnityEngine;
using System.Collections;

public class Noose : MonoBehaviour
{
    [Header("Main Pendulum (X/Z)")]
    [SerializeField] private float minAngle = 3f;
    [SerializeField] private float maxAngle = 7f;
    [SerializeField] private float baseSpeed = 1.2f;

    [Header("Torsion Twist (Y Axis)")]
    [SerializeField] private float twistAmount = 5f;
    [SerializeField] private float twistSpeed = 0.4f;

    [Header("Jitter (Creepy micro-movement)")]
    [SerializeField] private float jitterStrength = 0.3f;
    [SerializeField] private float jitterSpeed = 7f;

    private float xAmp;
    private float zAmp;

    private float xOffset;
    private float zOffset;

    private void Start()
    {
        // random phase offsets so multiple corpses don't sync
        xOffset = Random.Range(0f, 9999f);
        zOffset = Random.Range(0f, 9999f);

        ChooseNewAmplitudes();

        StartCoroutine(ChangeAmplitudeRoutine());
    }

    private void Update()
    {
        // Main pendulum swing (slow)
        float xAngle = Mathf.Sin((Time.time + xOffset) * baseSpeed) * xAmp;
        float zAngle = Mathf.Sin((Time.time + zOffset) * baseSpeed * 1.1f) * zAmp;

        // Rope torsion twist (super slow)
        float yAngle = Mathf.Sin(Time.time * twistSpeed) * twistAmount;

        // Creepy jitter (tiny fast motion)
        float jitterX = Mathf.PerlinNoise(Time.time * jitterSpeed, 0f) - 0.5f;
        float jitterZ = Mathf.PerlinNoise(0f, Time.time * jitterSpeed) - 0.5f;

        jitterX *= jitterStrength;
        jitterZ *= jitterStrength;

        transform.localRotation =
            Quaternion.Euler(xAngle + jitterX, yAngle, zAngle + jitterZ);
    }

    private IEnumerator ChangeAmplitudeRoutine()
    {
        while (true)
        {
            // wait until pendulum crosses center (sin ~ 0)
            yield return new WaitUntil(() =>
                Mathf.Abs(Mathf.Sin(Time.time * baseSpeed)) < 0.04f
            );

            ChooseNewAmplitudes();

            yield return new WaitForSeconds(0.3f);
        }
    }

    private void ChooseNewAmplitudes()
    {
        xAmp = Random.Range(minAngle, maxAngle);
        zAmp = Random.Range(minAngle, maxAngle);
    }
}
