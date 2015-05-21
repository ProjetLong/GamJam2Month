using UnityEngine;
using System.Collections;

[System.Serializable]
class FirePattern : IShootPattern
{
    public override IEnumerator shoot(Transform canon)
    {
        for (int i = 0; i < TweakManager.Instance.fireNbBullets; i++)
        {
            Vector3 direction = canon.forward;
            direction = Quaternion.AngleAxis(Random.Range(-TweakManager.Instance.fireConeAngle, TweakManager.Instance.fireConeAngle), Vector3.up) * direction;
            GameObject bullet = GameObject.Instantiate(TweakManager.Instance.bullet, canon.position, Quaternion.LookRotation(direction)) as GameObject;
            GameObject.Destroy(bullet, TweakManager.Instance.bulletLife);
        }
        yield return null;
    }
}
