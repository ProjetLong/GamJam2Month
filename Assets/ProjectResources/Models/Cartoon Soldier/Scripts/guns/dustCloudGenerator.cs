using UnityEngine;
using System.Collections;

public class dustCloudGenerator : MonoBehaviour
{
    GameObject dustCloudPrefab;
    float rate = 8.0f;
    Material[] materials;
    bool on = true;
    float life = 0.3f;

    private float nextdustCloudTime;
    private float destroyTime;
    Vector3 velocity;

    void Start()
    {
        destroyTime = Time.time + life;
    }

    void Update()
    {
        if (Time.time > destroyTime)
        {
            Destroy(gameObject);
        }
        if (Time.time > nextdustCloudTime)
        {
            nextdustCloudTime = Time.time + (1.0f / rate);
            GameObject newDustCloud = Instantiate(dustCloudPrefab, transform.position, transform.rotation) as GameObject;
            int materialId = Mathf.RoundToInt(Random.Range(0, materials.Length - 1));
            newDustCloud.renderer.material = materials[materialId];
            newDustCloud.GetComponent<dustCloud>().velocity = velocity;
        }
    }
}
