using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class crouchController : MonoBehaviour
{
    public float crouchSpeedMultiplier = 0.75f;
    public float crouchTogglingTime = 0.1f;
    public float globalCrouchBlend; //0 is standing up, 1 is crouching.

    public float globalCrouchBlendTarget;
    public float globalCrouchBlendVelocity;
    private bool disable;

    public void Update()
    {
        //Crouching.
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (!disable)
            {
                if (globalCrouchBlend < 0.5f)
                {
                    globalCrouchBlendTarget = 1.0f;
                }
                else
                {
                    globalCrouchBlendTarget = 0.0f;
                }
            }
            disable = true;
        }
        else
        {
            disable = false;
        }
        globalCrouchBlend = Mathf.SmoothDamp(globalCrouchBlend, globalCrouchBlendTarget, ref globalCrouchBlendVelocity, crouchTogglingTime);
    }
}
