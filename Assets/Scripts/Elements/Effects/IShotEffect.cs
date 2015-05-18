using UnityEngine;
using System.Collections;

public interface IShotEffect
{
    IEnumerator applyEffect(Enemy enemyScript, Transform callerTransform);
}
