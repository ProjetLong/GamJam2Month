using UnityEngine;
using System.Collections;

public class sentryGun : MonoBehaviour {
    public string[] targetRootName;
    public GameObject explosionPrefab;
    public GameObject blackSmokePrefab;

    private float rotatorAngleScan = 35;
    private float rotatorAcceleration = 5.0f;
    private bool firing = false;
    private bulletTraceGenerator bulletTraceGeneratorScript;
    private muzzleFlashGenerator muzzleFlashGeneratorScript;
    private spinningTurret spinningTurretScript;
    private Transform _base;
    private Transform rotator;
    private Transform pitch;
    private Transform barrel;
    private Transform bulletTraceGenerator;
    private Transform muzzleFlashGenerator;
    private Transform laserSight;
    private laserSight laserSightScript;
    private bool exploded = false;
    //Ai
    private bool foundTarget;
    private float lastTargetfoundTime;
    private float targetBufferTime;
    private Transform targetTransform;
    private float sentryHealth;
    private health healthScript;
    public bool useLaserToFindTargets = true;
    float targetMaxDeltaAngle = 30;
    private GameObject[] targets;
    //Rotator.
    private float rotatorSpeed;
    private float rotatorMaxSpeed = 60.0f;
    private float rotatorAngleTarget;
    private float rotatorDirection = 1;
    private float rotatorChangeDirectionTime;
    private float rotatorChangeDirectionDelay = 1.0f;
    //Pitch.
    private float pitchAngle;
    private float pitchVelocity;
    private float pitchTarget;
    private float deadPitchAngle = 30;

    void Start(){
	    _base = transform.Find("sentryGunBase");
	    rotator = _base.Find("sentryGunRotator");
	    pitch = rotator.Find("sentryGunPitch");
	    barrel = pitch.Find("sentryGunBarrel");
	    bulletTraceGenerator = barrel.Find("bulletTraceGenerator");
	    muzzleFlashGenerator = barrel.Find("muzzleFlashGenerator");
	    spinningTurretScript = barrel.GetComponent<spinningTurret>();
	    bulletTraceGeneratorScript = bulletTraceGenerator.GetComponent<bulletTraceGenerator>();
	    muzzleFlashGeneratorScript = muzzleFlashGenerator.GetComponent<muzzleFlashGenerator>();
	    laserSight = pitch.Find("laserSight");
	    laserSightScript = laserSight.GetComponent("laserSight");
	    targetBufferTime = 2.0f;
	    healthScript = GetComponent<health>();
    }

    void Update (){
	    float sentryHealth = healthScript.GetHealth ();
	    float barrelSpeed = spinningTurretScript.speed;
	    float maxBarrelSpeed = spinningTurretScript.maxSpeed;
	    if(barrelSpeed > maxBarrelSpeed * 0.8f){
		    bulletTraceGeneratorScript.on = firing;
		    muzzleFlashGeneratorScript.on = firing;
	    }
	    else{
		    bulletTraceGeneratorScript.on = false;
		    muzzleFlashGeneratorScript.on = false;	
	    }
	    spinningTurretScript.on = firing;
	    if(bulletTraceGeneratorScript.on){
		    rigidbody.AddForceAtPosition(bulletTraceGenerator.up* Time.deltaTime * Random.value * 1900, bulletTraceGenerator.position) ;
	    }
	    if(sentryHealth <= 0 && exploded == false){//Explosion.
		    exploded = true;
		    Instantiate(explosionPrefab,pitch.position,pitch.rotation);
		    rigidbody.AddForce(Vector3.up*400.0f);
		    GameObject newBlackSmoke = Instantiate(blackSmokePrefab,pitch.position,pitch.rotation) as GameObject;
		    newBlackSmoke.transform.parent = transform;
	    }
	    if(barrel.position.y < transform.position.y + 0.2 || sentryHealth == 0){
		    collider.enabled = false;
		    base.collider.enabled = true;
		    base.collider.isTrigger = false;
		    rotator.collider.enabled = true;
		    rotator.collider.isTrigger = false;
		    pitch.collider.enabled = true;
		    pitch.collider.isTrigger = false;
		    barrel.collider.enabled = true;
		    barrel.collider.isTrigger = false;
		    firing = false; //Deactivate if laying on the ground.
		    laserSightScript.on = false;
		    rotatorSpeed = Mathf.Lerp(rotatorSpeed,0, Time.deltaTime * rotatorAcceleration);
	    }
	    else{
		    sentryAI();
	    }
        rotator.localRotation = Quaternion.Euler (rotator.localRotation.eulerAngles.x, rotator.localRotation.eulerAngles.y, rotator.localRotation.eulerAngles.z + rotatorSpeed * Time.deltaTime);
	    //rotator.localRotation.eulerAngles.z += rotatorSpeed * Time.deltaTime;
	    //Pitch Angle.
	    if(sentryHealth > 0){ //Alive.
		    pitchAngle = 4.0f;
	    }
	    else{
		    pitchTarget = deadPitchAngle; //Dead.
		    if(pitchAngle > pitchTarget + 1){
			    pitchVelocity -= 3.0f * Time.deltaTime;
		    }
		    if(pitchAngle < pitchTarget -1){
			    pitchVelocity += 3.0f * Time.deltaTime;
		    }
		    pitchVelocity = Mathf.Lerp(pitchVelocity,0,Time.deltaTime*5.0f);
		    pitchAngle += pitchVelocity;
	    }
	    if(pitchAngle > deadPitchAngle){
		    pitchVelocity *= -1;
	    }
        pitch.localRotation = Quaternion.Euler (pitchAngle, pitch.localRotation.eulerAngles.y, pitch.localRotation.eulerAngles.z);
	    //pitch.localRotation.eulerAngles.x = pitchAngle;
    }

    void sentryAI(){
	    //Check for targets.
	    RaycastHit hit;
	    if (useLaserToFindTargets){ //This checks for targets using raycast (the laser).
		    if (Physics.Raycast(bulletTraceGenerator.position, bulletTraceGenerator.forward, out hit)){
			    bool isTarget = false;
			    for (var i = 0; i < targetRootName.Length; i++){
				    if (hit.transform.root.name == targetRootName[i]){
					    isTarget = true; //Check if what's ahead correspond to the target list.
				    }
			    }
			    if(isTarget){
				    foundTarget = true; //If what's ahead is a target.
				    lastTargetfoundTime = Time.time;
				    if(targetTransform == null){
					    targetTransform = hit.transform; //This is the target.
				    }
			    }
		    }
	    }
	    else{ //This checks for targets using a deltaAngle (Makes the turrent start shooting sooner).
		    targets = GetTargets();
		    for (var n = 0; n < targets.Length; n++){
			    float deltaAngle;
			    deltaAngle = Vector3.Angle(targets[n].transform.position - transform.position, -rotator.up);
			    Debug.DrawRay(transform.position, -rotator.up * 10.0f);
			    if(deltaAngle < targetMaxDeltaAngle){
				    foundTarget = true;
				    lastTargetfoundTime = Time.time;
				    targetTransform = targets[n].transform;
			    }
		    }
	    }
	    //If target has been destroyed.
	    if(targetTransform == null){
		    foundTarget = false;
		    firing = false;
	    }
	    //Target.
	    if(foundTarget){
		    firing = true;
		    Vector3 targetPositionNoHeight = targetTransform.position;
		    targetPositionNoHeight.y = rotator.position.y;
		    var targetDirection = targetPositionNoHeight - rotator.position;
		    float angleToTarget = Vector3.Angle(targetDirection, -rotator.up);
		    float targetSide = rotator.InverseTransformPoint(targetPositionNoHeight).x;
		    if(targetSide > 0){
			    rotatorSpeed = Mathf.Lerp(rotatorSpeed,rotatorMaxSpeed, Time.deltaTime * rotatorAcceleration);
		    }
		    else{
			    rotatorSpeed = Mathf.Lerp(rotatorSpeed,-rotatorMaxSpeed, Time.deltaTime * rotatorAcceleration);
		    }
	    }
	    //No target.
	    if(Time.time > lastTargetfoundTime + targetBufferTime){
		    foundTarget = false;
		    targetTransform = null;
		    firing = false;
		    float getRotatorAngle = rotator.localRotation.eulerAngles.z;
		    if (getRotatorAngle > 180){
			    getRotatorAngle -= 360;
		    }
		    if(rotatorDirection == 0){ //Choose new direction.
			    rotatorChangeDirectionTime = Time.time + rotatorChangeDirectionDelay;
			    if (getRotatorAngle > 0){
				    rotatorDirection = -1;
			    }
			    else{
				    rotatorDirection = 1;
			    }
		    }
		    if(Time.time < rotatorChangeDirectionTime){
			    rotatorSpeed = Mathf.Lerp(rotatorSpeed,0, Time.deltaTime * rotatorAcceleration);//Hold still.
		    }
		    if (rotatorDirection > 0 && Time.time > rotatorChangeDirectionTime){//Scan right.
			    if(getRotatorAngle > rotatorAngleScan * 0.95){
				    rotatorDirection = 0;
			    }
			    rotatorSpeed = Mathf.Lerp(rotatorSpeed,rotatorMaxSpeed, Time.deltaTime * rotatorAcceleration);
		    }
		    if(rotatorDirection < 0 && Time.time > rotatorChangeDirectionTime){//Scan left.
			    if(getRotatorAngle < -rotatorAngleScan * 0.95){
				    rotatorDirection = 0;
			    }
			    rotatorSpeed = Mathf.Lerp(rotatorSpeed,-rotatorMaxSpeed, Time.deltaTime * rotatorAcceleration);			
		    }
	    }
	    else{
		    rotatorSpeed = Mathf.Lerp(rotatorSpeed,0, Time.deltaTime * rotatorAcceleration);
	    }	
    }

    GameObject[] GetTargets() {
        GameObject[] returnList = new GameObject[targetRootName.Length];
	    for (var i = 0; i < targetRootName.Length; i++){
		    returnList[i] = GameObject.Find(targetRootName[i]);
	    }
	    return returnList;
    }
}
