using UnityEngine;
using System.Collections;

[System.Serializable]
class PoisonPattern : IShootPattern
{
    private RaycastHit hitInfo;
    public override IEnumerator shoot(Transform canon)
    {
        if (Physics.Raycast(canon.position, canon.forward, out hitInfo))
        {
            Vector3 spawnPosition = hitInfo.point;
            /*if (Physics.Raycast(spawnPosition, Vector3.up * -1, out hitInfo))
            {
                spawnPosition.y = hitInfo.point.y;
            }*/
            spawnPosition.y = 0.0f;
            //TODO: spawn 
            GameObject poison = PhotonNetwork.InstantiateSceneObject(TweakManager.Instance.poisonEffect.name, spawnPosition, Quaternion.identity, 0, null) as GameObject;
            PlayerShooting shootScript = canon.GetComponent<PlayerShooting>();
            poison.GetComponent<AOE>().combinaison = shootScript.playerScript.currentCombinaison;
            GameObject.Destroy(poison, TweakManager.Instance.poisonDuration);
        }
        yield return null;
    }
}
