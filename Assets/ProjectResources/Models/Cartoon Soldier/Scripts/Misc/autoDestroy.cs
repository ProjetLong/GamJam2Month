using UnityEngine;
using System.Collections;

public class autoDestroy : MonoBehaviour
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
    }
}
