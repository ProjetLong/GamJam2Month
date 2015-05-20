using UnityEngine;
using System.Collections;

public class shadow : MonoBehaviour
{
    public string[] ignoreRootName;
    public float distanceTolerance = 0.4f;
    public float maxOpacity = 1.0f;

    private float opacity = 1.0f;
    private Transform castingPoint;
    private float buffer = 0.02f;


    public void Start()
    {
        renderer.enabled = true;
        castingPoint = transform.Find("castingPoint");
        castingPoint.parent = transform.parent;
        transform.parent = transform.root;
    }

    public void LateUpdate()
    {
        //Opacity distance.
        float distanceShadow = Vector3.Distance(transform.position, castingPoint.position);
        opacity = Mathf.Lerp(maxOpacity, 0.0f, distanceShadow * (1 / distanceTolerance));
        //Shadow position.
        transform.position = castingPoint.position;
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position + Vector3.up * 0.5f, -Vector3.up);
        float maxShadowYPosition = -999999.0f;
        for (var i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            string name = hit.transform.root.name;
            bool takeIt = true;
            for (var n = 0; n < ignoreRootName.Length; n++)
            {
                string ignoreName = ignoreRootName[n];
                if (name == ignoreName)
                {
                    takeIt = false;
                }
            }
            if (takeIt)
            {
                if (hit.point.y + buffer > maxShadowYPosition)
                {
                    maxShadowYPosition = hit.point.y + buffer;

                    // Translation add
                    Vector3 newPosition = transform.position;
                    newPosition.y = hit.point.y + buffer;
                    transform.position = newPosition;
                    //transform.position.y = hit.point.y + buffer;
                    transform.LookAt(transform.position + hit.normal);
                }
            }
        }
        if (hits.Length == 0)
        {
            opacity = 0.0f;
        }

        // Translation add
        Color newColor = renderer.material.color;
        newColor.a = opacity;
        renderer.material.color = newColor;
        //renderer.material.color.a = opacity;
    }
}
