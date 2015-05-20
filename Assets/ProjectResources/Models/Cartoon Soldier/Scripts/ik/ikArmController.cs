using UnityEngine;
using System.Collections;

public class ikArmController : MonoBehaviour
{
    public Transform target;
    public Transform elbowTarget;

    private inverseKinematics inverseKinematicsScript;

    public void Start()
    {
        inverseKinematicsScript = GetComponent<inverseKinematics>();
    }

    public void Update()
    {
        inverseKinematicsScript.target = target.position;
        inverseKinematicsScript.elbowTarget = elbowTarget.position;
        inverseKinematicsScript.CalculateIK();
    }
}
