using UnityEngine;
using System.Collections;

[RequireComponent(typeof(soldierMovement))]
[RequireComponent(typeof(crouchController))]
public class weaponController : MonoBehaviour
{

    public float aimSpeed = 5.0f;
    public float accuracyLossMultiplier = 0.5f;

    private bool firing = false;
    private gunSelector gunSelectorScript;
    private Transform crosshairTransform;
    private float accuracyLoss;
    private float accuracyLossTarget;
    private float shootingAimLoss;
    private float vibratingAimLoss; //shootingAimLoss with firing vibration.
    private bool isSprinting;
    //External scripts.
    private crosshair crosshairScript;
    private soldierMovement soldierMovementScript;
    private crouchController crouchControllerScript;
    private health healthScript;

    // Use this for initialization
    void Start()
    {
        crosshairTransform = this.transform.Find("crosshair");
        //External scripts.
        crosshairScript = crosshairTransform.GetComponent<crosshair>();
        soldierMovementScript = GetComponent<soldierMovement>();
        crouchControllerScript = GetComponent<crouchController>();
        healthScript = GetComponent<health>();
    }

    // Update is called once per frame
    void Update()
    {
        float health = 100;
        if (healthScript != null)
        {
            health = healthScript.GetHealth();
        }
        //Input.
        bool isGrounded = soldierMovementScript.isGrounded;
        if (Input.GetMouseButton(0) && !isSprinting && isGrounded && health > 0)
        {
            firing = true;
            gunSelectorScript.BroadcastMessage("Fire", SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            firing = false;
        }
        //Accuracy.
        float aimCrouchMultiplier = 1 + crouchControllerScript.globalCrouchBlend * 10;
        float turnSpeed = soldierMovementScript.turnSpeed;
        float forwardSpeed = soldierMovementScript.forwardSpeed;
        float strafeSpeed = soldierMovementScript.strafeSpeed;
        accuracyLossTarget = 1.0f;
        if (forwardSpeed > soldierMovementScript.forwardSpeedMultiplier * 1.2)
        {
            isSprinting = true;
            accuracyLossTarget += 1.0f;
        }
        else
        {
            isSprinting = false;
        }

        if (firing)
        {
            shootingAimLoss = Mathf.Lerp(shootingAimLoss, 2.0f, Time.deltaTime * 2.0f);
            crosshairScript.yOffset += Random.Range(0, 0.5f) * Time.deltaTime;
            crosshairScript.xOffset += Random.Range(-0.05f, shootingAimLoss * 0.1f) * Time.deltaTime;
        }
        else
        {
            shootingAimLoss = Mathf.Lerp(shootingAimLoss, 0, Time.deltaTime * 20.0f);
        }
        vibratingAimLoss = shootingAimLoss + Mathf.Sin(Time.time * 80) * shootingAimLoss * 0.5f;
        accuracyLossTarget += vibratingAimLoss;
        accuracyLossTarget += Mathf.Pow(Mathf.Abs(forwardSpeed * 2.0f + strafeSpeed * 2.0f), 0.1f);
        accuracyLossTarget += Mathf.Pow(Mathf.Pow(Mathf.Abs(turnSpeed), 2.3f) / Mathf.Pow(10, 4), 0.35f);
        accuracyLossTarget += (1 - crouchControllerScript.globalCrouchBlend) * 0.5;
        accuracyLossTarget *= accuracyLossMultiplier;
        if (accuracyLoss > accuracyLossTarget)
        {
            accuracyLoss = Mathf.Lerp(accuracyLoss, accuracyLossTarget, Time.deltaTime * aimSpeed * aimCrouchMultiplier * 0.5f);//Gain aim.
        }
        else
        {
            accuracyLoss = Mathf.Lerp(accuracyLoss, accuracyLossTarget, Time.deltaTime * aimSpeed);//Lose aim.
        }
        crosshairScript.accuracyLoss = accuracyLoss;
        accuracyLoss = Mathf.Max(accuracyLoss, 1.0f);
        float accuracy = 1 / accuracyLoss;
        gunSelectorScript.BroadcastMessage("SetAccuracy", accuracy, SendMessageOptions.DontRequireReceiver);
    }

    public bool isFiring()
    {
        return this.firing;
    }
}
