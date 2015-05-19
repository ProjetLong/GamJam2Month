using UnityEngine;
using System.Collections;

public class TweakManager : MonoBehaviour
{
    private static TweakManager instance;
    public static TweakManager Instance
    {
        get { return instance; }
    }

    void Awake()
    {
        if (TweakManager.instance == null)
        {
            TweakManager.instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    //base
    public GameObject bullet;
    public float bulletLife = 5.0f;
    //poison
    public GameObject poisonEffect;
    public float poisonDuration = 3.0f;
    //fire
    public float fireConeAngle = 0.0f;
    public int fireNbBullets = 5;
    //air
    public int airNbBullets = 3;
    public float airBulletInterval = 0.25f;
    //ice
    public float iceLaserDuration = 10.0f;

    //combinaison
    public float combinaisonTimeToLive = 15.0f;
}
