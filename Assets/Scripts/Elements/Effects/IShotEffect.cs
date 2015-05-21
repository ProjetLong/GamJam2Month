using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class IShotEffect
{
    public abstract IEnumerator applyEffect(Enemy enemyScript, Transform callerTransform);
}
