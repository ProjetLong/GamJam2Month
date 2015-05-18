using System;
using System.Collections;

public interface IShotEffect
{
    IEnumerator applyEffect(Enemy enemyScript);
}
