using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EntitiesManager : Photon.MonoBehaviour
{
    private static EntitiesManager instance;
    public static EntitiesManager Instance
    {
        get { return instance; }
    }

    void Awake()
    {
        if (EntitiesManager.instance == null)
        {
            EntitiesManager.instance = this;
            //DontDestroyOnLoad (this);
        }
        else
        {
            Destroy(this);
        }
    }

    public List<GameObject> spawnedZombies;

    [RPC]
    public void newZombieSpawned(GameObject spawnedZombie)
    {
        spawnedZombies.Add(spawnedZombie);
        if (this.photonView.isMine)
        {
            this.photonView.RPC("newZombieSpawned", PhotonTargets.OthersBuffered, spawnedZombie);
        }
    }

    public void zombieDied(GameObject deadZombie)
    {
        spawnedZombies.Remove(deadZombie);
    }

    public GameObject getNearestZombie(Transform zombieTrans)
    {
        if (spawnedZombies.Count == 0)
        {
            return null;
        }
        else if (spawnedZombies.Count == 1)
        {
            // If no other zombie return zombie
            return zombieTrans.gameObject;
        }

        float smallestDistance = spawnedZombies
            .Select(x => Vector3.SqrMagnitude(x.transform.position - zombieTrans.position))
            .Where(y => y > float.Epsilon)
            .Min<float>();
        GameObject nearestZombie = spawnedZombies.FirstOrDefault(x =>
            Vector3.SqrMagnitude(x.transform.position - zombieTrans.position) == smallestDistance);


        return nearestZombie;
    }
}
