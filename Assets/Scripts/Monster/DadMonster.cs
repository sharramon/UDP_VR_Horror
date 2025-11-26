using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DadMonster : Monster
{

    [Header("Dad Attack Settings")]
    public float rushDistance = 3f;
    public float rushTime = 0.7f;

    [SerializeField] private Animator dadAnimator;

    public override IEnumerator Attack()
    {
        // Example rush-forward scare
        Vector3 start = transform.position;
        Vector3 end = start + transform.forward * rushDistance;

        Tween t = transform.DOMove(end, rushTime).SetEase(Ease.InQuad);
        yield return t.WaitForCompletion();
    }

    public void SetAnimTrigger(string trigger)
    {
        if(dadAnimator == null)
        {
            return;
        }

        ClearAllTriggers();
        dadAnimator.SetTrigger(trigger);
    }

    private void ClearAllTriggers()
    {
        if (dadAnimator == null) return;

        for (int i = 0; i < dadAnimator.parameterCount; i++)
        {
            AnimatorControllerParameter p = dadAnimator.GetParameter(i);
            if (p.type == AnimatorControllerParameterType.Trigger)
            {
                dadAnimator.ResetTrigger(p.name);
            }
        }
    }
}
