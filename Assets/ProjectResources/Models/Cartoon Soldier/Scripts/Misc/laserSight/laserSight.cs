using UnityEngine;
using System.Collections;

public class laserSight : MonoBehaviour
{

    GameObject laserLinePrefab;
    bool on;
    bool disableRootCollider = true;

    private Transform laserPointTransform;
    private Transform laserPointOrigin;
    private float laserLineRate = 2;
    private float nextLaserLineTime;
    private float positionBuffer = 2.0f;//Between the ends.

    void Start()
    {
        on = true;
        laserPointTransform = transform.Find("laserPoint");
        laserPointOrigin = transform.Find("laserPointOrigin");
    }

    void Update()
    {
        RaycastHit hit;
        float maxLength = 20.0f;
        if (disableRootCollider)
        {
            transform.root.collider.enabled = false;
        }
        if (Physics.Raycast(transform.position, transform.forward, out hit) && on)
        {
            triggerChildrenCollider triggerChildrenColliderScript = hit.transform.root.GetComponent<triggerChildrenCollider>();//Children collider property.
            bool reCheck = false; //Re-check if there's a hit for children collider.
            Collider mainColliderHit = hit.collider; //Parent collider. (must be re enabled)
            Collider[] childrenColliderList = null;
            if (triggerChildrenColliderScript != null)
            { //Trigger children property. Enable children collider and disable root collider.
                hit.collider.enabled = false;
                childrenColliderList = triggerChildrenColliderScript.childrenColliderList;
                for (var i = 0; i < childrenColliderList.Length; i++)
                {
                    childrenColliderList[i].enabled = true;
                }
                reCheck = Physics.Raycast(transform.position, transform.forward, out hit); //Recheck collision for children collider.
            }
            if (reCheck || triggerChildrenColliderScript == null)
            {
                laserPointTransform.position = hit.point + hit.normal * 0.03f;
                laserPointTransform.GetComponent<laserPoint>().on = true;
                maxLength = Mathf.Min(maxLength, Vector3.Distance(transform.position, hit.point));
            }
            else
            {
                laserPointTransform.GetComponent<laserPoint>().on = false;
            }
            if (triggerChildrenColliderScript != null)
            {//Trigger children property. Disable children collider and enable root collider.
                mainColliderHit.enabled = true;
                for (var n = 0; n < childrenColliderList.Length; n++)
                {
                    childrenColliderList[n].enabled = false;
                }
            }
        }
        else
        {
            laserPointTransform.GetComponent<laserPoint>().on = false;
        }
        if (disableRootCollider)
        {
            transform.root.collider.enabled = true;
        }
        laserLineRate = maxLength * 0.5f;

        if (Time.time > nextLaserLineTime && on)
        {
            nextLaserLineTime = Time.time + (1 / laserLineRate);
            GameObject newLaserLine = Instantiate(laserLinePrefab, transform.position, Quaternion.identity) as GameObject;
            newLaserLine.name = "laserLine";
            newLaserLine.transform.parent = transform;
            newLaserLine.transform.localRotation = Quaternion.identity * Quaternion.Euler(90, 0, 0);
            //newLaserLine.transform.localRotation.eulerAngles.x += 90;
            Vector3 temp = newLaserLine.transform.localPosition;
            if (maxLength < positionBuffer * 2.0f)
            {
                temp.z = positionBuffer;
            }
            else
            {
                temp.z = Random.Range(positionBuffer, maxLength - positionBuffer);
            }
            newLaserLine.transform.localPosition = temp;
        }
        if (on)
        {
            laserPointOrigin.GetComponent<laserPoint>().on = true;
        }
        else
        {
            laserPointTransform.GetComponent<laserPoint>().on = false;
            laserPointOrigin.GetComponent<laserPoint>().on = false;
        }
        //Delete laser lines further than ray cast hit.
        if (maxLength > positionBuffer * 2)
        {
            for (var m = 0; m < transform.childCount; m++)
            {
                Transform child = transform.GetChild(m);
                if (child.localPosition.z > maxLength && child.name == "laserLine")
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }
}
