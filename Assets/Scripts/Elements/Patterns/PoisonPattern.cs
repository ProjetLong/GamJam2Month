using UnityEngine;
using System.Collections;

class PoisonPattern : IShootPattern
{
    private RaycastHit hitInfo;
    public IEnumerator shoot(Transform canon)
    {
        if (Physics.Raycast(canon.position, canon.forward, out hitInfo))
        {
            Vector3 spawnPosition = hitInfo.point;
            if (Physics.Raycast(spawnPosition, Vector3.up * -1, out hitInfo))
            {
                spawnPosition.y = hitInfo.point.y;
            }
            //TODO: spawn 
            GameObject poison = GameObject.Instantiate(TweakManager.Instance.poisonEffect, spawnPosition, Quaternion.identity) as GameObject;
            GameObject.Destroy(poison, TweakManager.Instance.poisonDuration);
        }
        yield return null;
    }
}
