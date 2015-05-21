using System.Collections;
using UnityEngine;

[System.Serializable]
public abstract class IShootPattern
{
    public abstract IEnumerator shoot(Transform canon);
}
