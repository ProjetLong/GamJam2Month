using UnityEngine;
using System.Collections;

public class laserLine : MonoBehaviour
{

    public GameObject laserDustPrefab;

    private float startTime;
    private float life = 1.0f;
    private float lifeVariation = 1.0f;
    private float endTime;
    private float length;
    private float laserDustRate = 12.0f;
    private float nextLaserDustTime;
    private Color laserColor;
    private float curveProgress;

    void Start()
    {
        startTime = Time.time;
        life = life + lifeVariation * Random.value;
        endTime = Time.time + life;
        length = Random.Range(1, 3);
        laserColor = new Color(0, 0, 0);
        for (var i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            child.renderer.material.SetColor("_TintColor", laserColor);
        }
    }

    void Update()
    {
        if (Time.time > endTime)
        {
            Destroy(gameObject);
        }
        float age = Time.time - startTime;
        float progress = age / life;
        curveProgress = -4 * Mathf.Pow(progress, 2) + progress * 4;
        laserColor = new Color(curveProgress * 0.10f, 0, 0);
        for (var i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.name == "visual")
            {
                child.renderer.material.SetColor("_TintColor", laserColor);
                Vector3 temp = Vector3.one * 0.1f;
                temp.y = length + 2.0f * curveProgress + Random.value * 1.0f;
                child.localScale = temp;
            }
        }
        transform.localScale = Vector3.one;
        if (Time.time > nextLaserDustTime)
        {
            nextLaserDustTime = Time.time + (1 / laserDustRate);
            GameObject newLaserDust = Instantiate(laserDustPrefab, transform.position, Quaternion.identity) as GameObject;
            newLaserDust.transform.parent = transform;
            float getPosition = (transform.localScale.y * 0.5f) / newLaserDust.transform.localScale.y;
            Vector3 temp = new Vector3(0, Random.Range(-getPosition * 0.5f, getPosition * 0.5f), 0);
            newLaserDust.transform.localPosition = temp;

        }
    }

    public float GetCurveProgress()
    {
        return curveProgress; //Red is the only color used on the laser color. Black is transparent because of particle additive material.
    }
}
