﻿using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class soldierMovement : Photon.MonoBehaviour
{
    //This script controls how the soldier will move.
    public float forwardSpeedMultiplier = 3.5f;
    public float strafeSpeedMultiplier = 2.0f;
    public float turnSpeedMultiplier = 5.0f;
    public float gravity = 9.8f;
    //public String soldierLocation = "smoothWorldPosition/soldierSkeleton";
    public Transform soldier;
    public float turnSpeed = 0.0f;
    public float forwardSpeed = 0.0f; //Speed the character will moved at.
    public float strafeSpeed = 0.0f;
    public bool isGrounded;

    private float stopAfterLanding = 0.0f; //How much time in seconds will the character stop after landing.
    private float fallSpeed = 0.0f;
    private float lastGroundedTime; //Last time since soldier was touching the ground.
    private float lastLandingTime; //Last time since the soldier landed after a fall.
    private float targetForwardSpeed;
    private float targetStrafeSpeed;
    private crouchController crouchControllerScript;
    private bool isFalling;
    private health healthScript;
    private float recoilAmount;
    private float recoilAmountTarget;

    void Start()
    {
        crouchControllerScript = GetComponent<crouchController>();
        healthScript = GetComponent<health>();
        isFalling = false;
        //soldier = transform.Find(soldierLocation);
    }

    void Update()
    {
        //Stick to platforms.
        float rayHeight = 0.5f;
        Vector3 rayOrigin = transform.position + Vector3.up * rayHeight;
        Ray platformRay = new Ray(rayOrigin, Vector3.down);
        float rayDistance = rayHeight * 2.0f;
        RaycastHit platformHit;
        if (Physics.Raycast(platformRay, out platformHit, rayDistance))
        {
            if (platformHit.transform.root != transform)
            {
                isGrounded = true;
                transform.position = new Vector3(transform.position.x, platformHit.point.y, transform.position.z);
                //transform.position.y = platformHit.point.y;
            }
        }
        else
        {
            isGrounded = false;
        }

        CharacterController controller = GetComponent<CharacterController>();
        //Hit recoil.
        Vector3 recoilVector = Vector3.zero;
        float recoilInhibit = 1.0f;
        float deathInhibit = 1.0f;
        if (healthScript != null)
        {
            float lastHitTime = healthScript.GetLastHitTime();
            Vector3 recoilDirecion = healthScript.GetrecoilDirecion();
            float maxRecoil = 0.6f;
            float timeAfterHit = Time.time - lastHitTime;
            float recoilAmountTarget = 0.0f;
            if (timeAfterHit < maxRecoil && lastHitTime != 0)
            {
                recoilAmountTarget = maxRecoil - timeAfterHit;
                recoilAmountTarget *= (1 - Mathf.Abs(Input.GetAxis("Vertical")));
                recoilAmountTarget *= (1 - Mathf.Abs(Input.GetAxis("Horizontal")));
            }
            if (recoilAmount < recoilAmountTarget)
            {
                recoilAmount = Mathf.Lerp(recoilAmount, recoilAmountTarget, Time.deltaTime);
            }
            else
            {
                recoilAmount = Mathf.Lerp(recoilAmount, recoilAmountTarget, Time.deltaTime * 20);
            }
            float biasRecoilAmount = recoilAmount; //Bias to 0.
            biasRecoilAmount /= maxRecoil;
            biasRecoilAmount = Mathf.Pow(biasRecoilAmount, 2.0f);
            biasRecoilAmount *= maxRecoil;
            recoilVector = recoilDirecion * biasRecoilAmount;
            recoilInhibit = 1 - (recoilAmount * 0.65f / maxRecoil);
            float health = healthScript.GetHealth();
            if (health <= 0)
            {
                deathInhibit = 0;
            }
            else
            {
                deathInhibit = 1.0f;
            }
        }
        //Position.
        if (isGrounded)
        {
            if (isFalling)
            {
                isFalling = false;
                lastLandingTime = Time.time;
                stopAfterLanding = (lastLandingTime - lastGroundedTime) * 2.0f;
            }
            fallSpeed = 0.0f;
        }
        else
        {
            if (!isFalling)
            {
                isFalling = true;
                lastGroundedTime = Time.time;
            }
            fallSpeed += gravity * Time.deltaTime;
        }
        Vector3 moveDirection = Vector3.zero;
        moveDirection.y -= fallSpeed;
        if (isGrounded)
        {
            targetForwardSpeed = Input.GetAxis("Vertical");
            targetStrafeSpeed = Input.GetAxis("Horizontal");
            targetForwardSpeed *= forwardSpeedMultiplier;
            targetStrafeSpeed *= strafeSpeedMultiplier;
            if (Input.GetAxis("Vertical") < 0)
            {//Slow down going backwards;
                targetForwardSpeed *= 0.5f;
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {//Sprint with left shift;
                targetForwardSpeed *= 1.5f;
                targetStrafeSpeed *= 1.5f;
            }
        }
        if (crouchControllerScript != null)
        { //Crouch speed multiplier.
            targetForwardSpeed = Mathf.Lerp(targetForwardSpeed, targetForwardSpeed * crouchControllerScript.crouchSpeedMultiplier, crouchControllerScript.globalCrouchBlend);
        }
        if (Time.time <= lastLandingTime + stopAfterLanding && stopAfterLanding > 0.5)
        {
            float timeSinceLanding = Time.time - lastLandingTime;
            float landingSpeedInhibit = Mathf.Pow(timeSinceLanding / stopAfterLanding, 1.5f);
            targetForwardSpeed *= landingSpeedInhibit;
            targetStrafeSpeed *= landingSpeedInhibit;
        }
        forwardSpeed = Mathf.Lerp(forwardSpeed, targetForwardSpeed, Time.deltaTime * 15.0f);
        strafeSpeed = Mathf.Lerp(strafeSpeed, targetStrafeSpeed, Time.deltaTime * 15.0f);
        moveDirection += soldier.forward * forwardSpeed * recoilInhibit * deathInhibit;
        moveDirection += soldier.right * strafeSpeed * recoilInhibit * deathInhibit;
        moveDirection += recoilVector;
        controller.Move(moveDirection * Time.deltaTime);//Move the controller.
        //Rotation.
        float targetTurnSpeed = 0.0f;
        targetTurnSpeed = Input.GetAxis("Mouse X");
        targetTurnSpeed *= Mathf.Pow(turnSpeedMultiplier, 3);
        targetTurnSpeed *= deathInhibit;
        turnSpeed = Mathf.Lerp(turnSpeed, targetTurnSpeed, Time.deltaTime * 25.0f);
        Vector3 temp = transform.rotation.eulerAngles;
        temp.y += turnSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(temp);
        //transform.rotation.eulerAngles.y += turnSpeed * Time.deltaTime;
    }
}