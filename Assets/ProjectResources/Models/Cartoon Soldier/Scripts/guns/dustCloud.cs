using UnityEngine;
using System.Collections;

public class dustCloud : MonoBehaviour
{

    float life = 2.0f;
    private float startTime;
    private float destroyTime;
    public Vector3 velocity;
    private float gravity = 9.8f;
    private float angle;
    private float startSturnSpeed;
    private float turnSpeed;
    private float startScale;
    private float endScale;

    void Start()
    {
        startTime = Time.time;
        destroyTime = Time.time + life;
        velocity += Random.insideUnitSphere * .5f;
        velocity.y += Random.value * 0.6f;
        turnSpeed = Random.Range(-360, 360);
        startSturnSpeed = turnSpeed;
        angle = Random.value * 360;
        startScale = Random.Range(0.05f, 0.01f);
        transform.localScale = Vector3.one * startScale;
        endScale = 1.0f + Random.value * 2.0f;
    }

    void Update()
    {
        if (Time.time > destroyTime)
        {
            Destroy(gameObject);
        }
        float age = Time.time - startTime;
        float falloffProgress = Mathf.Pow(age / life, 0.2f);
        turnSpeed = Mathf.Lerp(startSturnSpeed, 0, falloffProgress);
        velocity.y -= gravity * Time.deltaTime;
        velocity = Vector3.Lerp(velocity, Vector3.zero, Time.deltaTime * 5.0f);
        transform.position += velocity * Time.deltaTime;
        transform.LookAt(Camera.main.transform.position);
        angle += turnSpeed * Time.deltaTime;
        Vector3 temp = transform.localRotation.eulerAngles;
        temp.z = angle;
        transform.localRotation = Quaternion.Euler(temp);
        transform.localScale = Vector3.Lerp(Vector3.one * startScale, Vector3.one * endScale, falloffProgress);
        Color cloudColor = renderer.material.GetColor("_Color");
        cloudColor.a = Mathf.Lerp(cloudColor.a, 0, Time.deltaTime * life);
        renderer.material.SetColor("_Color", cloudColor);
    }
}
