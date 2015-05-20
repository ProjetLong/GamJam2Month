using UnityEngine;
using System.Collections;

public class bulletHole : MonoBehaviour {
    public Material[] materials;
    public float life = 4.0f;
    public float size = 1.0f;

    private float destroyTime;
    private float startTime;

    void Start(){
	    startTime = Time.time;
	    destroyTime = Time.time + life;
	    int chooseId = Mathf.RoundToInt(Random.Range(0,materials.Length));
	    renderer.material = materials[chooseId];
        Transform parent = transform.parent;
	    transform.parent = null;
        transform.localRotation = Quaternion.Euler (transform.localRotation.x, transform.localRotation.y, Random.value * 360);
	    //transform.localRotation.eulerAngles.z = Random.value * 360;
	    transform.localScale = Vector3.one * (0.5f + Random.value * 0.5f) * size;
	    transform.parent = parent;
    }

    void Update () {
	    if (Time.time > destroyTime){
		    Destroy(gameObject);
	    }
	    //var age = Time.time - startTime;
	    if (Time.time > destroyTime - 1.0){
		    float fadeProgress = destroyTime-Time.time;
            renderer.material.color = new Color (renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, fadeProgress);
		    //renderer.material.color.a = fadeProgress;
	    }
    }
}
