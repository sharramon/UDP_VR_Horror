using UnityEngine;
using System.Collections;

public class DaughterMonster : Monster
{

    public IEnumerator HangingReveal()
    {
        Show();
        // optional: fade-in, sway animation, etc.
        yield return new WaitForSeconds(1.0f);
    }
}
