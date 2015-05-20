using UnityEngine;
using System.Collections;

public class bulletTraceGenerator : MonoBehaviour
{
    GameObject bulletTracePrefab;
    float rate = 8.0f;
    Vector3 velocity;
    public bool on = false;
    public float accuracy = 1.0f; //1.0 to 0.0;

    private float nextbulletTraceTime;

    void Update()
    {
        accuracy = Mathf.Clamp01(accuracy);
        if (on)
        {
            if (Time.time > nextbulletTraceTime)
            {
                rate = Mathf.Max(rate, 1.0f);
                nextbulletTraceTime = Time.time + (1.0f / rate);
                GameObject newBulletTrace = Instantiate(bulletTracePrefab, transform.position, transform.rotation) as GameObject;
                Vector3 bulletVelocity = newBulletTrace.GetComponent<bulletTrace>().velocity;
                float badAim = (1 - accuracy);
                badAim *= newBulletTrace.GetComponent<bulletTrace>().bulletSpeed * 0.05f;
                bulletVelocity += newBulletTrace.transform.right * Random.Range(-badAim, badAim);
                bulletVelocity += newBulletTrace.transform.up * Random.Range(-badAim, badAim);
                newBulletTrace.GetComponent<bulletTrace>().velocity = bulletVelocity;
            }
        }
    }
}
