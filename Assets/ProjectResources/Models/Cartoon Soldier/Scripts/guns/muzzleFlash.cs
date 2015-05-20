using UnityEngine;
using System.Collections;

public class muzzleFlash : MonoBehaviour
{

    float minLife = 0.01f;
    float maxLife = 0.02f;

    private float destroyTime;
    private float angle;

    void Start()
    {
        destroyTime = Time.time + Random.Range(minLife, maxLife);
        angle = 90 * Mathf.Round(Random.Range(0, 3));
    }

    void Update()
    {
        if (Time.time > destroyTime)
        {
            Destroy(gameObject);
        }
        transform.LookAt(Camera.main.transform.position);
        Vector3 temp = transform.localRotation.eulerAngles;
        temp.z += angle;
        transform.localRotation = Quaternion.Euler(temp);
    }
}
