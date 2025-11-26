using UnityEngine;
using System.Collections;

public class MonsterController : MonoBehaviour
{
    [Header("Monster Actors")]
    public DadMonster dad;
    public DaughterMonster daughter;
    public GiantHeadMonster giantHead;

    [Header("Monster start pos")]
    public Transform dadPos;

    // ============================================================
    // DAUGHTER
    // ============================================================

    public IEnumerator ShowHangingDaughter()
    {
        if (daughter == null) yield break;
        yield return daughter.HangingReveal();
    }

    // ============================================================
    // DAD
    // ============================================================

    public IEnumerator PlayDadAudio(AudioClip clip)
    {
        if (dad == null) yield break;
        yield return dad.Speak(clip);
    }

    public IEnumerator DadAttack()
    {
        if (dad == null) yield break;
        yield return dad.Attack();
    }

    // ============================================================
    // GIANT DAUGHTER HEAD
    // ============================================================

    public IEnumerator ShowGiantHead()
    {
        if (giantHead == null) yield break;
        yield return giantHead.PopIn();
    }

    // ============================================================
    // UTILITY
    // ============================================================

    public void HideAll()
    {
        if (dad) dad.Hide();
        if (daughter) daughter.Hide();
        if (giantHead) giantHead.Hide();
    }
}
