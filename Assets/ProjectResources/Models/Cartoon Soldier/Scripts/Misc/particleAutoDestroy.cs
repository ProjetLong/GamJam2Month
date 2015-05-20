using UnityEngine;
using System.Collections;

public class particleAutoDestroy : MonoBehaviour
{
    public float life;
    private float destroyTime;

    public void Start()
    {
        destroyTime = Time.time + life;
    }

    public void Update()
    {
        if (Time.time > destroyTime)
        {
            Destroy(gameObject);
        }
        if (Time.time > destroyTime - particleEmitter.maxEnergy)
        {
            particleEmitter.emit = false;
        }
    }
}
