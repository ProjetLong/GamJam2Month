using System.Collections;
using UnityEngine;

public interface IShootPattern
{
    IEnumerator shoot(Transform canon);
}
