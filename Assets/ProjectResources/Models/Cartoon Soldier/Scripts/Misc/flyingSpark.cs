using UnityEngine;
using System.Collections;

public class flyingSpark : MonoBehaviour
{
    public float life = 1.5f;
    public float lifeVariation = 0.5f;

    private float startTime;
    private float destroyTime;
    private Vector3 velocity;
    private float gravity = 9.8f;
    private float scale = 1.0f;


    public void Start()
    {
        startTime = Time.time;
        life += Random.Range(-lifeVariation * 0.5f, lifeVariation * 0.5f); //Vary life.
        destroyTime = Time.time + life;
        velocity = Random.insideUnitSphere * 4.0f;
        velocity.y += Random.value * 3.0f;
    }

    public void Update()
    {
        if (Time.time > destroyTime)
        {
            Destroy(gameObject);
        }
        RaycastHit hit;
        if (Physics.Raycast(transform.position, velocity, out hit, velocity.magnitude * Time.deltaTime))
        {
            velocity = Vector3.Reflect(velocity, hit.normal);
        }
        //Velocity.
        velocity.y -= gravity * Time.deltaTime;
        velocity = Vector3.Lerp(velocity, Vector3.zero, Time.deltaTime);
        transform.position += velocity * Time.deltaTime;
        //Rotation.
        transform.LookAt(transform.position + velocity);
        //Scale.
        scale = Mathf.Lerp(0.2f, 0.05f, (Time.time - startTime) / life);

        // Translation add
        Vector3 newLocalScale = Vector3.one * scale;
        newLocalScale.z = (0.2f + velocity.magnitude * 0.6f) * scale;
        transform.localScale = newLocalScale;
        //transform.localScale = Vector3.one * scale;
        //transform.localScale.z = (0.2f + velocity.magnitude * 0.6f) * scale;
    }

}
