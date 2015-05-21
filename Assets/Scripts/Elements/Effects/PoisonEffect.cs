using UnityEngine;
using System.Collections;

[System.Serializable]
public class PoisonEffect : IShotEffect
{
    #region Members
    public float duration = 0.0f;
    #endregion

    public IEnumerator applyEffect(Enemy enemyScript, Transform callerTransform)
    {
        enemyScript.enterConfusion();
        yield return new WaitForSeconds(duration);
        enemyScript.snapOutOfConfusion();
    }
}
