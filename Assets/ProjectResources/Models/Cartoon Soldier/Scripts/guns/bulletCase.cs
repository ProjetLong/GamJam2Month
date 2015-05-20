using UnityEngine;
using System.Collections;

public class bulletCase : MonoBehaviour
{

    float life = 0.5f;

    private float destroyTime;
    Vector3 velocity;
    private float gravity = 9.8f;
    private float turnSpeed;
    private float turnAngle;

    void Start()
    {
        destroyTime = Time.time + life;
        turnAngle = Random.value * 360;
        turnSpeed = Random.Range(-360.0f, 360.0f);
    }

    void Update()
    {
        if (Time.time > destroyTime)
        {
            Destroy(gameObject);
        }
        transform.LookAt(Camera.main.transform.position);
        turnAngle += turnSpeed * Time.deltaTime;
        Vector3 temp = transform.localRotation.eulerAngles;
        temp.z += turnAngle;
        transform.localRotation = Quaternion.Euler(temp);
        velocity.y -= gravity * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime * life * 2.0f);
    }
}
