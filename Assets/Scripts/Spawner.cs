﻿using UnityEngine;
using System.Collections;

public abstract class Spawner : MonoBehaviour
{

    public GameObject toSpawn;
    // Use this for initialization
    void Start()
    {
        if (this.toSpawn == null)
        {
            this.enabled = false;
            Debug.Log("Nothing to spawn !!!");
        }
    }

    void Update()
    {
    }

    protected virtual void spawn()
    {
        if (PhotonNetwork.isMasterClient)
        {
            GameObject spawned = PhotonNetwork.Instantiate("Prefabs/" + this.toSpawn.name, this.transform.position + this.transform.up * 2, this.transform.rotation, 0) as GameObject;
            spawned.transform.parent = this.transform;
            spawned.transform.position = this.transform.position;
            spawned.transform.rotation = this.transform.rotation;

            EntitiesManager.Instance.newZombieSpawned(spawned);
        }

    }
}
