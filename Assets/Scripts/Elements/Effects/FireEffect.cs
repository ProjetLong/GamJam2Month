using System.Collections;
using UnityEngine;

[System.Serializable]
public class FireEffect : IShotEffect
{
    #region Members
    public int power = 1;
    public float duration = 10.0f;
    public float interval = 1.0f;
    #endregion

    public override IEnumerator applyEffect(Enemy enemyScript, Transform callerTransform)
    {
        float endtime = Time.time + duration;

        while (Time.time <= endtime)
        {
            enemyScript.takeDamage(Combinaison.ELEMENTS.FIRE, power);

            yield return new WaitForSeconds(interval);
        }
    }
}
