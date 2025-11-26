using UnityEngine;
using System.Collections;
using DG.Tweening;

public class GiantHeadMonster : Monster
{
    [Header("Reveal Settings")]
    public float popTime = 0.4f;

    [Header("BlendShapes")]
    public SkinnedMeshRenderer faceRenderer;
    private int happyIndex = -1;

    [Header("Neck Transform")]
    public Transform neck;

    private Coroutine trackPlayerCoroutine;

    private void Awake()
    {
        if (faceRenderer != null && faceRenderer.sharedMesh != null)
            happyIndex = faceRenderer.sharedMesh.GetBlendShapeIndex("Head.Happy");
    }

    public IEnumerator PopIn()
    {
        Show();
        transform.localScale = Vector3.zero;
        Tween t = transform.DOScale(1f, popTime).SetEase(Ease.OutBack);
        yield return t.WaitForCompletion();
    }

    public void SetHappy(float weight)
    {
        if (happyIndex < 0) return;
        faceRenderer.SetBlendShapeWeight(happyIndex, weight);
    }

    public IEnumerator SlideHappy(float targetWeight, float duration)
    {
        if (happyIndex < 0) yield break;

        float start = faceRenderer.GetBlendShapeWeight(happyIndex);

        Tween t = DOTween.To(
            () => start,
            v => faceRenderer.SetBlendShapeWeight(happyIndex, v),
            targetWeight,
            duration
        );

        yield return t.WaitForCompletion();
    }

    public void StartTrackPlayer(Transform playerTransform)
    {
        if (trackPlayerCoroutine != null)
            StopCoroutine(trackPlayerCoroutine);

        trackPlayerCoroutine = StartCoroutine(TrackPlayer(playerTransform));
    }

    public void StopTrackPlayer()
    {
        if (trackPlayerCoroutine != null)
        {
            StopCoroutine(trackPlayerCoroutine);
            trackPlayerCoroutine = null;
        }
    }

    private IEnumerator TrackPlayer(Transform playerTransform)
    {
        // Rotate bone so +Z aligns with +Y
        Quaternion axisFix = Quaternion.Euler(0f, 90f, -90f);

        while (true)
        {
            Vector3 target = playerTransform.position;
            target.y = neck.position.y;

            // direction from neck to player
            Vector3 dir = (target - neck.position).normalized;

            // apply correction so Y behaves as forward
            Quaternion lookRot = Quaternion.LookRotation(dir, Vector3.up) * axisFix;

            neck.rotation = lookRot;

            yield return null;
        }
    }
}
