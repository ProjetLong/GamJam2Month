using UnityEngine;
using System.Collections;

public class platform : MonoBehaviour
{
    public float maxHeight = 5.0f;
    public float minHeight = 1.0f;
    public float speed = 2.0f;
    private bool rising;
    private float velocity;

    public void Start()
    {
        rising = true;
    }
    public void Update()
    {
        if (rising)
        {
            // Translation add
            Vector3 temp = transform.position;
            temp.y = Mathf.SmoothDamp(transform.position.y, maxHeight, ref velocity, 1.0f / speed);
            transform.position = temp;
            //transform.position.y = Mathf.SmoothDamp(transform.position.y, maxHeight, ref velocity, 1.0f / speed);
            if (Mathf.Abs(transform.position.y - maxHeight) < 0.1)
            {
                rising = false;
            }
        }
        else
        {
            // Translation add
            Vector3 temp = transform.position;
            temp.y = Mathf.SmoothDamp(transform.position.y, minHeight, ref velocity, 1.0f / speed);
            transform.position = temp;
            //transform.position.y = Mathf.SmoothDamp(transform.position.y, minHeight, ref velocity, 1.0f / speed);
            if (Mathf.Abs(transform.position.y - minHeight) < 0.1)
            {
                rising = true;
            }
        }
    }
}
