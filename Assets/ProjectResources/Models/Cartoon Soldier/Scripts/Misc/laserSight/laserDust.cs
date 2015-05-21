using UnityEngine;
using System.Collections;

public class laserDust : MonoBehaviour
{

    private float startTime;
    private float life = 0.5f;
    private float lifeVariation = 1.0f;
    private float endTime;
    //private float length;
    private float scale;
    private float maxAlpha;

    void Start()
    {
        startTime = Time.time;
        life = life + lifeVariation * Random.value;
        endTime = Time.time + life;
        //length = Random.Range(6, 8);
        scale = Random.Range(0.11f, 0.14f);
        var laserColor = new Color(0, 0, 0);
        renderer.material.SetColor("_TintColor", laserColor);
        maxAlpha = Random.Range(0.1f, 0.3f);
    }

    void Update()
    {
        if (Time.time > endTime)
        {
            Destroy(gameObject);
        }
        var age = Time.time - startTime;
        var progress = age / life;
        var curveProgress = -4 * Mathf.Pow(progress, 2) + progress * 4;
        float parentAlpha = 1.0f;
        if (transform.parent != null)
        {
            parentAlpha = transform.parent.GetComponent<laserLine>().GetCurveProgress();
        }
        var laserColor = new Color(curveProgress * maxAlpha * parentAlpha, 0, 0);
        renderer.material.SetColor("_TintColor", laserColor);
        transform.LookAt(Camera.main.transform.position);
        transform.localScale = Vector3.one * (scale + curveProgress * scale * 0.2f);
    }
}
