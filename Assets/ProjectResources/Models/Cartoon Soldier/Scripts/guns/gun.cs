using UnityEngine;
using System.Collections;

public class gun : MonoBehaviour
{

    bool firing = false;
    float accuracy;
    private Transform bulletCaseGenerator;
    private Transform bulletTraceGenerator;
    private Transform muzzleFlashGenerator;
    private bulletCaseGenerator bulletCaseGeneratorScript;
    private bulletTraceGenerator bulletTraceGeneratorScript;
    private muzzleFlashGenerator muzzleFlashGeneratorScript;

    void Start()
    {
        Transform bulletCaseGenerator = transform.Find("bulletCaseGenerator");
        Transform bulletTraceGenerator = transform.Find("bulletTraceGenerator");
        Transform muzzleFlashGenerator = transform.Find("muzzleFlashGenerator");
        bulletCaseGeneratorScript = bulletCaseGenerator.GetComponent<bulletCaseGenerator>();
        bulletTraceGeneratorScript = bulletTraceGenerator.GetComponent<bulletTraceGenerator>();
        muzzleFlashGeneratorScript = muzzleFlashGenerator.GetComponent<muzzleFlashGenerator>();
        firing = false;
        //accuracy = 0.9;
    }

    void Update()
    {
        bulletCaseGeneratorScript.on = firing;
        bulletTraceGeneratorScript.on = firing;
        muzzleFlashGeneratorScript.on = firing;
        bulletTraceGeneratorScript.accuracy = accuracy;
        firing = false;
    }

    void Fire()
    {
        firing = true;
    }

    void SetAccuracy(float accuracyValue)
    {
        accuracy = accuracyValue;
    }
}
