using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class smoothWorldPosition : MonoBehaviour
{
    //Character Controller clibs stairs' steps in a very snappy manner.
    //The purpose of this script is to smooth this snappy changes of position.
    //This script must be in a game object in between the hierarchy of the character controller and the soldier.
    public Collider[] ignoreColliders;
    public float horizontalSmooth = 2.0f;
    public float verticalSmooth = 10.0f;
    private Vector3 worldPosition;
    private Vector3 platformPosition;
    private Vector3 previousPlatformPosition;
    private Transform platform;
    private bool moveFast;
    private float nothingBelowTime;
    private float nothingBelowBuffer = 0.2f;
    List<string> list = new List<string>();

    public void LateUpdate()
    {
        horizontalSmooth = Mathf.Max(horizontalSmooth, 0);
        verticalSmooth = Mathf.Max(verticalSmooth, 0);
        if (horizontalSmooth == 0 || moveFast)
        {
            worldPosition.x = transform.parent.position.x;
            worldPosition.z = transform.parent.position.z;
        }
        else
        {
            worldPosition.x = Mathf.Lerp(worldPosition.x, transform.parent.position.x, Time.deltaTime * 50.0f / horizontalSmooth);
            worldPosition.z = Mathf.Lerp(worldPosition.z, transform.parent.position.z, Time.deltaTime * 50.0f / horizontalSmooth);
        }

        if (verticalSmooth == 0 || moveFast)
        {
            worldPosition.y = transform.parent.position.y;
        }
        else
        {
            worldPosition.y = Mathf.Lerp(worldPosition.y, transform.parent.position.y, Time.deltaTime * 50.0f / verticalSmooth);
        }
        transform.position = worldPosition;
        moveFast = ShouldMoveFast();
    }

    public bool ShouldMoveFast()
    {
        RaycastHit[] hits = Physics.RaycastAll(worldPosition + Vector3.up * 0.25f, -Vector3.up, 0.5f);
        float closestHitDistance = -1.0f;
        Transform closestTransform = null; ;
        for (var i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            Collider hitCollider = hit.collider;
            bool ignore = false;
            for (int n = 0; n < ignoreColliders.Length; n++)
            {
                Collider ignoreCollider = ignoreColliders[n];
                if (hitCollider == ignoreCollider)
                {
                    ignore = true;
                }
            }
            if (!ignore)
            {
                list.Add(hit.transform.name);
                if (closestHitDistance == -1)
                {
                    closestHitDistance = hit.distance;
                    closestTransform = hit.transform;
                }
                else
                {
                    if (hit.distance < closestHitDistance)
                    {
                        closestHitDistance = hit.distance;
                        closestTransform = hit.transform;
                    }
                }
            }
        }
        if (closestTransform == null)
        {//No platform found.
            platform = null;
            nothingBelowTime += Time.deltaTime; //Start counting time if nothing is below.
            if (nothingBelowTime > nothingBelowBuffer)
            {
                return true; //Nothing below, then it's falling, it should move fast.
            }
            else
            {
                return false; //Nothing is below, but respect buffer. Move smoothly.
            }
        }
        else
        {
            nothingBelowTime = 0; //Reset this value because platorm was found.
            if (platform == closestTransform)
            { //If still at the same platform, compare with previous position.
                platformPosition = platform.position;
                if (platformPosition != previousPlatformPosition)
                {//If position is different, then it's a moving platform, it should move fast.
                    return true;
                }
                else
                {
                    return false; //Platform is at the same position, is on a static floor. Should move smoothly.
                }
            }
            else
            { //Is on a different platform, store the inital position.
                platform = closestTransform;
                previousPlatformPosition = platform.position;
                return false;
            }
        }

    }

}
