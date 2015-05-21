using System.Collections;
using UnityEngine;

[System.Serializable]
class AirPattern : IShootPattern
{
    public IEnumerator shoot(Transform canon)
    {
        for (int i = 0; i < TweakManager.Instance.airNbBullets; i++)
        {
            GameObject bullet = GameObject.Instantiate(TweakManager.Instance.bullet, canon.position, Quaternion.LookRotation(canon.forward)) as GameObject;
            GameObject.Destroy(bullet, TweakManager.Instance.bulletLife);
            yield return new WaitForSeconds(TweakManager.Instance.airBulletInterval);
        }
        yield return null;
    }
}
