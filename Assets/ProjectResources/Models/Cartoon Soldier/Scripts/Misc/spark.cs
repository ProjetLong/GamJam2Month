using UnityEngine;
using System.Collections;

public class spark : MonoBehaviour
{
    private float life = 0.06f;
    private float destroyTime;
    private float angle;
    public Material[] material;
    public GameObject flyingSparkPrefab;
    private int flyingSparkAmount = 3;
    private int flyingSparkAmountVariation = 2;

    public void Start()
    {
        int materialID = Mathf.FloorToInt(Random.value * material.Length);
        renderer.material = material[materialID];
        destroyTime = Time.time + life;
        angle = Random.value * 360;
        transform.LookAt(Camera.main.transform.position);

        // Translation add
        Vector3 newLocalRotation = transform.localRotation.eulerAngles;
        newLocalRotation.z += angle;
        transform.localRotation = Quaternion.Euler(newLocalRotation);
        transform.localScale = transform.localScale * (0.5f + Random.value);
        //transform.localRotation.eulerAngles.z += angle;
        int flyingSparkAmount = this.flyingSparkAmount + Mathf.RoundToInt(Random.Range(-flyingSparkAmountVariation * 0.5f, flyingSparkAmountVariation * 0.5f));
        for (var i = 0; i < flyingSparkAmount; i++)
        {
            Instantiate(flyingSparkPrefab, transform.position, transform.rotation);
        }
    }

    public void Update()
    {
        if (Time.time > destroyTime)
        {
            Destroy(gameObject);
        }
        transform.LookAt(Camera.main.transform.position);

        // Translation add
        Vector3 newLocalRotation = transform.localRotation.eulerAngles;
        newLocalRotation.z += angle;
        transform.localRotation = Quaternion.Euler(newLocalRotation);
        transform.localScale = transform.localScale * (0.5f + Random.value);
        //transform.localRotation.eulerAngles.z += angle;
        transform.localScale *= 1 + 10 * (Time.deltaTime);
    }
}
