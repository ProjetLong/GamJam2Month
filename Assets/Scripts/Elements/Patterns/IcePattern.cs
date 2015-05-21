using System.Collections;
using UnityEngine;

[System.Serializable]
class IcePattern : IShootPattern
{
    private RaycastHit hitInfo;
    public override IEnumerator shoot(Transform canon)
    {
        float duration = TweakManager.Instance.iceLaserDuration;
        GameObject bullet = GameObject.Instantiate(TweakManager.Instance.bullet) as GameObject;
        while (duration >= 0)
        {
            duration -= Time.deltaTime;
            Debug.DrawRay(canon.transform.position, canon.forward);
            if (Physics.Raycast(canon.position, canon.forward, out hitInfo))
            {
                bullet.transform.position = hitInfo.point;
            }
            else
            {
                bullet.transform.position = canon.position + canon.forward * 1000f;
            }
            yield return null;
        }
        GameObject.Destroy(bullet);
    }
}
