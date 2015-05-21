using UnityEngine;
using System.Collections;

public class bulletTrace : MonoBehaviour {
    public float life = 0.5f;
    public float bulletSpeed = 1.0f;
    public GameObject dustPrefab;
    public GameObject bulletHolePrefab;

    private float destroyTime;
    public Vector3 velocity;
    private float gravity = 9.8f;

    void Start(){
	    destroyTime = Time.time + life;
	    velocity += transform.forward * bulletSpeed;
    }

    void Update () {
	    if(Time.time > destroyTime){
		    Destroy(gameObject);
	    }
	    Ray ray = new Ray ();
	    ray.origin = transform.position;
	    ray.direction = transform.forward;
	    RaycastHit hit;
        Collider[] childrenColliderList = null;
	    if (Physics.Raycast(ray, out hit,bulletSpeed*Time.deltaTime)){
		    triggerChildrenCollider triggerChildrenColliderScript = hit.transform.root.GetComponent<triggerChildrenCollider>();
		    bool reCheck = false; //Re-check if there's a hit for children collider.
		    Collider mainColliderHit = hit.collider; //Parent collider. (must be re enabled)
            
		    if(triggerChildrenColliderScript != null){ //Trigger children property. Enable children collider and disable root collider.
			    hit.collider.enabled = false;
                childrenColliderList = triggerChildrenColliderScript.childrenColliderList;
			    for (var i = 0; i < childrenColliderList.Length; i++){
				    childrenColliderList[i].enabled = true;
			    }
			    reCheck = Physics.Raycast(ray, out hit,bulletSpeed*Time.deltaTime); //Recheck collision for children collider.
		    }
		    if(reCheck || triggerChildrenColliderScript == null){ //If children collider or root collider are hit, process the bullet hit.
			    Destroy(gameObject);
			    bool makeDust = true;
			    bool makeHole = true;
			    bulletHoleProperty bulletHolePropertyScript =  hit.transform.root.GetComponent<bulletHoleProperty>();
			    GameObject extraPrefab = null;
			    float holeSize = 1.0f;
			    //Material[] differentBulletMaterialArray;
                Material[] bulletMaterials = null;
			    if (bulletHolePropertyScript != null){ //Bullet hole property.
				    makeDust = bulletHolePropertyScript.dust;
				    makeHole = bulletHolePropertyScript.hole;
				    extraPrefab = bulletHolePropertyScript.extraPrefab;
				    holeSize = bulletHolePropertyScript.sizeModifier;
				    bulletMaterials = bulletHolePropertyScript.bulletMaterials;
			    }
			    if (extraPrefab != null){ //Extra prefab.
				    Instantiate(extraPrefab,transform.position,transform.rotation);
			    }
			    if (makeDust){ //Dust.
				    GameObject newdustCloudGenerator = Instantiate(dustPrefab,hit.point - transform.forward*0.1f, Quaternion.identity) as GameObject;
				    newdustCloudGenerator.GetComponent<dustCloudGenerator>().velocity = hit.normal * (2 + Random.value*10);
			    }
			    if (makeHole){ //Bullet hole.
				    GameObject newBulletHole = Instantiate(bulletHolePrefab, hit.point + hit.normal *0.01f, Quaternion.identity) as GameObject;
				    newBulletHole.transform.LookAt(newBulletHole.transform.position + hit.normal);
				    newBulletHole.transform.parent = hit.collider.transform;
				    bulletHole bulletHoleScript = newBulletHole.GetComponent<bulletHole>();
				    bulletHoleScript.size *= holeSize;
				    if (bulletHolePropertyScript != null && bulletMaterials.Length > 0){
					    bulletHoleScript.materials = bulletMaterials;
				    }
			    }
			    if(hit.rigidbody != null){ //Apply force.
				    hit.rigidbody.AddForceAtPosition(velocity* 1.5f, hit.point);
			    }
			    health healthScript = hit.transform.root.GetComponent<health>(); //Health property.
			    if(healthScript != null){
				    healthScript.SetHealth(healthScript.GetHealth() - 20);
				    healthScript.SetLastHitTime();
				    Vector3 hitPointNoHeight = hit.point;//hit.point;
				    hitPointNoHeight.y = hit.transform.position.y;
				    Vector3 hitDirection = hitPointNoHeight - hit.transform.position;
				    hitDirection.Normalize();
				    hitDirection = hit.transform.root.InverseTransformDirection(hitDirection);
				    healthScript.SetHitDirection(hitDirection);
                    Vector3 recoilDirection = hit.transform.position - hitPointNoHeight;
				    recoilDirection.Normalize();
				    healthScript.SetrecoilDirecion(recoilDirection);
			    }
		    }
		    if(triggerChildrenColliderScript != null){//Trigger children property. Disable children collider and enable root collider.
			    mainColliderHit.enabled = true;
			    for (var n = 0; n < childrenColliderList.Length; n++){
				    childrenColliderList[n].enabled = false;
			    }
		    }
	    }
	    velocity.y -= gravity * Time.deltaTime;
	    transform.position += velocity * Time.deltaTime;
	    transform.LookAt(transform.position + velocity);
    }
}
