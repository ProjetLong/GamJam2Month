using System.Collections;
using UnityEngine;

public class FireEffect : IShotEffect
{
    #region Members
    public int power = 1;
    public float duration = 10.0f;
    public float interval = 1.0f;
    #endregion

    public IEnumerator applyEffect(Enemy enemyScript)
    {
        float endtime = Time.time + duration;

        while (Time.time <= endtime)
        {
            enemyScript.takeDamage(Combinaison.ELEMENTS.FIRE, power);

            yield return new WaitForSeconds(interval);
        }
    }
}
