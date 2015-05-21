using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class AirEffect : IShotEffect
{
    #region Members
    public float bumpForce = 0.0f;
    public float explosionRadius = 1.0f;
    #endregion

    public IEnumerator applyEffect(Enemy enemyScript, Transform callerTransform)
    {
        enemyScript.GetComponent<Rigidbody>().AddExplosionForce(
            bumpForce, callerTransform.position, explosionRadius);
        yield return null;
    }
}
