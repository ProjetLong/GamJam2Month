using UnityEngine;

public class soldierAnimation : MonoBehaviour {
    //Curves.
    public AnimationCurve idleBlendCurve;
    public AnimationCurve walkBlendCurve;
    public AnimationCurve runBlendCurve;
    public AnimationCurve sprintBlendCurve;
    public AnimationCurve strafeBlendCurve;
    public AnimationCurve turnBlendCurve;
    public AnimationCurve animationSpeedCurve;
    public AnimationCurve turnAnimationSpeedCurve;
    public AnimationCurve fallingBlendCurve;
    public AnimationCurve landingDurationCurve;
    public AnimationCurve hitBlendCurve;
    public float tiltMultiplier = 1.0f;

    //Animation blend values.
    public float idleBlend;
    public float walkBlend;
    public float runBlend;
    public float sprintBlend;
    public float strafeBlend;
    public float turnBlend;
    public float animationSpeed;
    public float turnAnimationSpeed;
    public float fallingBlend;
    public float landingDuration;
    public float landingInhibit;
    public float landingBlend;
    public float crouchIdleBlend;
    public float crouchRunBlend;
    public float crouchSprintBlend;
    public float crouchStrafeBlend;
    public float crouchTurnBlend;
    public float hitBlend;
    public float dieBlend;
    //---
    private Quaternion soldierRotation;
    private float verticalSpeed;
    private Vector3 lastPosition;
    private float lastYRotation;
    private float tilt;
    private bool backward; //Switch when strafing backwards.
    private float backwardBuffer = 0.5f; //So it doesn switches too fast.
    private float lastLandingTime; //Last time soldier landed after a fall.
    private bool isFalling;
    private float startedFallingTime;
    private bool isGrounded;
    private float hitStartTime ; //Tme in which hit animation should start playing.
    private Vector3 getHitDirection;
    //External scripts.
    public crouchController crouchControllerScript;
    private weaponController weaponControllerScript;
    private health healthScript;
    //
    void Start(){
	    soldierRotation = Quaternion.identity;
	    crouchControllerScript.globalCrouchBlend = 0.0f;
	    crouchControllerScript.globalCrouchBlendTarget = 0.0f;
	    crouchControllerScript.globalCrouchBlendVelocity = 0.0f;
	    crouchControllerScript = transform.root.GetComponent<crouchController>();
	    weaponControllerScript = transform.root.GetComponent<weaponController>();
	    healthScript = transform.root.GetComponent<health>();
    }

    void LateUpdate(){
	    //Gather external script info.
	    bool firing = false; //Firing.
	    if (weaponControllerScript != null){
		     firing = weaponControllerScript.isFiring();
	    }
	    float crouchInhibit = 1.0f;//Crouch.
	    float standInhibit = 0.0f;
	    float crouchSpeedMultiplier = 1.0f;
	    if(crouchControllerScript != null){
		    crouchInhibit = 1 - crouchControllerScript.globalCrouchBlend;
		    standInhibit = (1 -crouchInhibit);
		    crouchSpeedMultiplier = crouchControllerScript.crouchSpeedMultiplier;
	    }
	    float lastHitTime = 0;
	    Vector3 hitDirection = Vector3.zero;
	    //float _health;
	    float deathTime = 0;
	    if (healthScript != null){
		    lastHitTime = healthScript.GetLastHitTime();
		    hitDirection = healthScript.GetHitDirection();
		    //_health = healthScript.GetHealth();
		    deathTime = healthScript.GetDeathTime();
	    }
	    //Velocity calculation.
	    Vector3 velocity = (transform.position - lastPosition) / Time.deltaTime; //Units per second.
	    //float previousVerticalSpeed = verticalSpeed;
	    verticalSpeed = (transform.position.y - lastPosition.y) / Time.deltaTime;
	    float overallSpeed = (transform.position - lastPosition).magnitude / Time.deltaTime;
	    lastPosition = transform.position;
	    float forwardSpeed = transform.InverseTransformDirection(velocity).z;
	    float strafeSpeed = transform.InverseTransformDirection(velocity).x;
	    float turnSpeed = Mathf.DeltaAngle(lastYRotation, transform.rotation.eulerAngles.y);
	    lastYRotation = transform.rotation.eulerAngles.y;
	    //Is grounded.
	    float rayHeight = 0.3f;
	    Vector3 rayOrigin = transform.position + Vector3.up * rayHeight;
	    Ray groundRay = new Ray(rayOrigin, Vector3.down);
	    float rayDistance = rayHeight * 2.0f;
	    RaycastHit groundHit;
	    if (Physics.Raycast(groundRay, out groundHit, rayDistance)){
		    isGrounded = true;
	    }
	    else{
		    isGrounded = false;
	    }
	    //Animation blending.
	    float minimumFallSpeed  = -0.5f; //Should be called maximum.
	    if (!isFalling && verticalSpeed < minimumFallSpeed && !isGrounded){ //Start falling time.
		    isFalling = true;
		    startedFallingTime = Time.time;
	    }
	    float totalFallDuration = 0.0f;
	    if(isFalling && verticalSpeed > minimumFallSpeed){ //Land time.
		    isFalling = false;
		    lastLandingTime = Time.time;
		    totalFallDuration = (lastLandingTime - startedFallingTime);
		    landingDuration = landingDurationCurve.Evaluate(totalFallDuration);
	    }
	    float fallDuration;
	    if (Time.time > startedFallingTime && isFalling){//Current fall duration.
		    fallDuration = Time.time - startedFallingTime;
	    }
	    else{
		    fallDuration = 0.0f;
	    }
	    //Animation blending values.
	    float hitInhibit = 1 - hitBlend;//Make other animations inhibit when getting hit.
	    float dieInhibit = 1 - dieBlend;//Don't play animations if dying.
	    float blendSpeed = 0.2f;
	    fallingBlend = fallingBlendCurve.Evaluate(fallDuration); //Falling blend.
	    fallingBlend *= dieInhibit;
	    animation.Blend("soldierFalling",fallingBlend,blendSpeed);
	    float fallingInhibit = Mathf.Pow(Mathf.Abs(1 - fallingBlend),2.0f);//Make other animations inhibit when falling.
	    if(Time.time < lastLandingTime + landingDuration){ //Landing blend.
		    float timeSinceLanding  =  Time.time - lastLandingTime;
		    float landingProgress  = timeSinceLanding / landingDuration; //From 0 to 1.
		    landingProgress = Mathf.Pow(landingProgress, 0.6f);
		    landingBlend = 1 - landingProgress;
		    float landingAnimationStartTime = Mathf.Clamp01(1 - landingDuration);
		    animation["soldierLanding"].time = Mathf.Lerp(landingAnimationStartTime, 1.0f, landingProgress);
		    animation.Blend("soldierLanding",landingBlend,0.05f);
		    landingInhibit = Mathf.Pow(1 - landingBlend,2.0f);
	    }
	    else{
		    landingBlend = 0.0f;
		    landingDuration = 0.0f;
		    //float landingAnimationStartTime = 0.0f;
		    landingInhibit = 1.0f;
	    }
	    var idleBlend = idleBlendCurve.Evaluate(Mathf.Abs(forwardSpeed) + Mathf.Abs(strafeSpeed)); //Idle blend.
	    idleBlend -= Mathf.Abs(turnSpeed) * .8f;
	    idleBlend *= fallingInhibit;
	    idleBlend *= landingInhibit;
	    idleBlend *= crouchInhibit;
	    idleBlend *= hitInhibit;
	    idleBlend *= dieInhibit;
	    //idleBlend = Mathf.Clamp01(idleBlend);
	    animation.Blend("soldierIdle",idleBlend,blendSpeed);
	    walkBlend = walkBlendCurve.Evaluate(Mathf.Abs(forwardSpeed)); //Walk blend.
	    walkBlend *= fallingInhibit;
	    walkBlend *= landingInhibit;
	    walkBlend *= crouchInhibit;
	    walkBlend *= dieInhibit;
	    walkBlend = Mathf.Clamp01(walkBlend);
	    animation.Blend("soldierWalk",walkBlend,blendSpeed);
	    runBlend = runBlendCurve.Evaluate(Mathf.Abs(forwardSpeed)); //Run blend.
	    runBlend *= fallingInhibit;
	    runBlend *= landingInhibit;
	    runBlend  *= crouchInhibit;
	    runBlend *= dieInhibit;
	    //runBlend = Mathf.Clamp01(runBlend);
	    animation.Blend("soldierRun",runBlend,blendSpeed);
	    sprintBlend = sprintBlendCurve.Evaluate(forwardSpeed);//Sprint blend.
	    sprintBlend *= fallingInhibit;
	    sprintBlend *= landingInhibit;
	    sprintBlend *= crouchInhibit;
	    sprintBlend *= dieInhibit;
	    //sprintBlend = Mathf.Clamp01(sprintBlend);
	    animation.Blend("soldierSprint",sprintBlend,blendSpeed);
	    strafeBlend = strafeBlendCurve.Evaluate(Mathf.Abs(strafeSpeed)); //Strafing blend.
	    strafeBlend *= fallingInhibit;
	    strafeBlend *= landingInhibit;
	    strafeBlend *= crouchInhibit;
	    strafeBlend *= dieInhibit;
	    //strafeBlend = Mathf.Clamp01(strafeBlend);
	    if (forwardSpeed > backwardBuffer){
		    backward = false;
	    }
	    if(forwardSpeed < -backwardBuffer){
		    backward = true;
	    }
	    if(!backward){
		    if(strafeSpeed > 0){
			    animation.Blend("soldierStrafeRight",strafeBlend,blendSpeed);
			    animation.Blend("soldierStrafeLeft",0,blendSpeed);
		    }
		    else{
			    animation.Blend("soldierStrafeLeft",strafeBlend,blendSpeed);
			    animation.Blend("soldierStrafeRight",0,blendSpeed);
		    }
	    }
	    else{
		    if(strafeSpeed > 0){
			    animation.Blend("soldierStrafeLeft",strafeBlend,blendSpeed);
			    animation.Blend("soldierStrafeRight",0,blendSpeed);
		    }
		    else{
			    animation.Blend("soldierStrafeRight",strafeBlend,blendSpeed);
			    animation.Blend("soldierStrafeLeft",0,blendSpeed);
		    }	
	    }
	    turnBlend = turnBlendCurve.Evaluate(Mathf.Abs(turnSpeed)); //Turn blend.
	    turnBlend -= overallSpeed;
	    turnBlend = Mathf.Clamp01(turnBlend);
	    turnBlend *= crouchInhibit;
	    turnBlend *= dieInhibit;
	    if(turnSpeed > 0){
		    animation.Blend("soldierSpinRight",turnBlend,blendSpeed);
		    animation.Blend("soldierSpinLeft",0,blendSpeed);
	    }
	    else{
		    animation.Blend("soldierSpinLeft",turnBlend,blendSpeed);
		    animation.Blend("soldierSpinRight",0,blendSpeed);	
	    }
	    //Crouch Idle animation blending. Blend values are calculated above for convenince.
	    if (crouchControllerScript != null){ //Works with a global crouch value that's handled in the crouch controller script.	
		    float inverseCrouchSpeedMultiplier = (1/crouchSpeedMultiplier);
		    crouchIdleBlend = idleBlendCurve.Evaluate((Mathf.Abs(forwardSpeed) + Mathf.Abs(strafeSpeed))* inverseCrouchSpeedMultiplier); //Crouch idle blend.
		    crouchIdleBlend -= Mathf.Abs(turnSpeed) * .8f;
		    crouchIdleBlend *= fallingInhibit;
		    crouchIdleBlend *= landingInhibit;
		    crouchIdleBlend *= standInhibit;
		    crouchIdleBlend *= dieInhibit;
		    animation.Blend("soldierCrouch",crouchIdleBlend,0.05f);
		    crouchRunBlend = runBlendCurve.Evaluate(Mathf.Abs(forwardSpeed) * inverseCrouchSpeedMultiplier);//Crouch run blend.
		    crouchRunBlend *= fallingInhibit;
		    crouchRunBlend *= landingInhibit;
		    crouchRunBlend *= standInhibit;
		    crouchRunBlend *= dieInhibit;
		    animation.Blend("soldierCrouchRun",crouchRunBlend,0.05f);
		    crouchSprintBlend = sprintBlendCurve.Evaluate(forwardSpeed * inverseCrouchSpeedMultiplier);//Crouch sprint blend.
		    crouchSprintBlend *= fallingInhibit;
		    crouchSprintBlend *= landingInhibit;
		    crouchSprintBlend *= standInhibit;
		    crouchSprintBlend *= dieInhibit;
		    animation.Blend("soldierCrouchSprint",crouchSprintBlend,0.05f);
		    crouchStrafeBlend = strafeBlendCurve.Evaluate(Mathf.Abs(strafeSpeed) * inverseCrouchSpeedMultiplier); //Crouch strafe blend.
		    crouchStrafeBlend *= fallingInhibit;
		    crouchStrafeBlend *= landingInhibit; 
		    crouchStrafeBlend *= standInhibit;
		    crouchStrafeBlend *= dieInhibit;
		    if(!backward){
			    if(strafeSpeed > 0){
				    animation.Blend("soldierCrouchStrafeRight",crouchStrafeBlend,blendSpeed*2);
				    animation.Blend("soldierCrouchStrafeLeft",0,blendSpeed*2);
			    }
			    else{
				    animation.Blend("soldierCrouchStrafeLeft",crouchStrafeBlend,blendSpeed*2);
				    animation.Blend("soldierCrouchStrafeRight",0,blendSpeed*2);
			    }
		    }
		    else{
			    if(strafeSpeed > 0){
				    animation.Blend("soldierCrouchStrafeLeft",crouchStrafeBlend,blendSpeed*2);
				    animation.Blend("soldierCrouchStrafeRight",0,blendSpeed*2);			
			    }
			    else{
				    animation.Blend("soldierCrouchStrafeRight",crouchStrafeBlend,blendSpeed*2);
				    animation.Blend("soldierCrouchStrafeLeft",0,blendSpeed*2);			
			    }
		    }
		    crouchTurnBlend = turnBlendCurve.Evaluate(Mathf.Abs(turnSpeed)); //Crouch turn blend.
		    crouchTurnBlend -= overallSpeed;
		    crouchTurnBlend = Mathf.Clamp01(crouchTurnBlend);
		    crouchTurnBlend *= standInhibit;
		    crouchTurnBlend *= dieInhibit;
		    if(turnSpeed > 0){
			    animation.Blend("soldierCrouchSpinRight", crouchTurnBlend, blendSpeed);
			    animation.Blend("soldierCrouchSpinLeft",0, blendSpeed);
		    }
		    else{
			    animation.Blend("soldierCrouchSpinLeft", crouchTurnBlend, blendSpeed);
			    animation.Blend("soldierCrouchSpinRight",0, blendSpeed);			
		    }		
	    }
	    float timeAfterHit = Time.time - lastHitTime; //Hit blend.
	    float getHitBlend = hitBlendCurve.Evaluate(timeAfterHit);
	    hitBlend = getHitBlend;
	    hitBlend *= dieInhibit;
	    float frontHitBlend = hitBlend * Mathf.Max(hitDirection.z,0);
	    animation.Blend("soldierHitFront",frontHitBlend,blendSpeed);
	    float backHitBlend = hitBlend * -Mathf.Min(hitDirection.z,0);
	    animation.Blend("soldierHitBack",backHitBlend,blendSpeed);
	    float rightHitBlend = hitBlend * Mathf.Max(hitDirection.x,0);
	    animation.Blend("soldierHitRight",rightHitBlend,blendSpeed);
	    float leftHitBlend = hitBlend * -Mathf.Min(hitDirection.x,0);
	    animation.Blend("soldierHitLeft",leftHitBlend,blendSpeed);
	    float timeSinceDeath = Time.time - deathTime;
	    bool dying = false;
	    if(timeSinceDeath > 0.1){
		    dying = true;
	    }
	    if(!dying){
		    getHitDirection = hitDirection;
	    }
	    if (healthScript.GetHealth() <= 0){  //Die blend.
		    dieBlend = 1.0f;
		    if (getHitDirection.z > 0){
			    animation["soldierDieFront"].time = timeSinceDeath;
			    if(animation["soldierDieFront"].time > animation["soldierDieFront"].length){
				    animation["soldierDieFront"].time = animation["soldierDieFront"].length;
			    }
			    animation.Blend("soldierDieFront",dieBlend,blendSpeed);
		    }
		    else{
			    animation["soldierDieBack"].time = timeSinceDeath;
			    if(animation["soldierDieBack"].time > animation["soldierDieBack"].length){
				    animation["soldierDieBack"].time = animation["soldierDieBack"].length;
			    }
			    animation.Blend("soldierDieBack",dieBlend,blendSpeed);
		    }
	    }
	    else{
		    dieBlend = 0.0f;
	    }
	    //Animation speed.
	    float animationSpeed;
	    //float strafeSpeedMultiplier = 1.4f; //Speed up strafe animations.
	
	    if(!backward){
		    animationSpeed = animationSpeedCurve.Evaluate(overallSpeed);
	    }
	    else{
		    animationSpeed = -animationSpeedCurve.Evaluate(overallSpeed);
	    }
	    animation["soldierWalk"].speed = animationSpeed;
	    animation["soldierRun"].speed = animationSpeed;
	    animation["soldierSprint"].speed = animationSpeed;
	    animation["soldierStrafeRight"].speed = animationSpeed;
	    animation["soldierStrafeLeft"].speed = animationSpeed;
	    animation["soldierCrouchRun"].speed = animationSpeed;
	    animation["soldierCrouchSprint"].speed = animationSpeed;
	    animation["soldierCrouchStrafeRight"].speed = animationSpeed;
	    animation["soldierCrouchStrafeLeft"].speed = animationSpeed;
	    float turnAnimationSpeed = turnAnimationSpeedCurve.Evaluate(Mathf.Abs(turnSpeed));
	    animation["soldierSpinRight"].speed = turnAnimationSpeed;
	    animation["soldierSpinLeft"].speed = turnAnimationSpeed;
	    animation["soldierCrouchSpinRight"].speed = turnAnimationSpeed;
	    animation["soldierCrouchSpinLeft"].speed = turnAnimationSpeed;
	    //Torso recoil when firing.
	    if (firing){
		    //var spine1 = transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1");
		    //var spine2 = spine1.Find("Bip01 Spine2");
            
		    //spine1.localRotation.eulerAngles.z += Mathf.Sin(Time.time * 50) * 0.5f;
		    //spine2.localRotation.eulerAngles.z += Mathf.Sin(Time.time * 50 - 1.0f) * 0.5f;
	    }
	    //Rotation.
	    float deltaAngle = Mathf.DeltaAngle(transform.rotation.eulerAngles.y, transform.root.rotation.eulerAngles.y);
	    float turnAngle = Mathf.Pow(Mathf.Abs(deltaAngle), 2.5f) * Mathf.Sign(deltaAngle) / 80;
	    turnAngle *= dieInhibit;
	    //soldierRotation.eulerAngles.y += turnAngle * Time.deltaTime;
	    //transform.rotation = soldierRotation;
        transform.Rotate (new Vector3 (0, soldierRotation.eulerAngles.y + turnAngle * Time.deltaTime, 0));
	    //Tilt
	    float tiltTarget = -turnAngle * 0.01f * forwardSpeed * tiltMultiplier;
	    tiltTarget = Mathf.Clamp(tiltTarget,-30f,30f);
	    tilt = Mathf.Lerp(tilt, tiltTarget, Time.deltaTime * 7.0f);
	    if (Mathf.Abs(verticalSpeed) > 1){
		    tilt /= Mathf.Abs(verticalSpeed);
	    }
	    //transform.localRotation.eulerAngles.z = tilt;
    }
}