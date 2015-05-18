﻿using System.Collections;
using UnityEngine;

public class IceEffect : IShotEffect
{
    #region Members
    [Range(0.0f, 100.0f)]
    public float slowRate = 10.0f;
    public float duration = 10.0f;
    public float interval = 1.0f;
    #endregion

    public IEnumerator applyEffect(Enemy enemyScript)
    {
        // Carefu
        slowRate /= 100.0f;
        slowRate = -slowRate;

        enemyScript.alterSpeedRate(slowRate);

        yield return new WaitForSeconds(interval);

        enemyScript.revertSpeedRate(slowRate);
    }
}
