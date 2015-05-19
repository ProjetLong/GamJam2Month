using UnityEngine;
using System.Collections;

public class health : MonoBehaviour {

	float _health = 300;

    private float lastHitTime;
    private Vector3 hitDirection;
    private Vector3 recoilDirecion;
    private float deathTime;
    private bool alive = true;

    void Update(){
        if (_health <= 0 && alive) {
		    alive = false;
		    deathTime = Time.time;
	    }
        if (_health > 0) {
		    alive = true;
	    }
        _health = Mathf.Max (_health, 0);
    }

    public void SetLastHitTime () {
	    lastHitTime = Time.time;
    }

    public void SetLastHitTime (float setTime) {
	    lastHitTime = setTime;
    }

    public float GetLastHitTime () {
	    return lastHitTime;
    }

    public void SetHitDirection (Vector3 direction) {
	    hitDirection = direction;
    }

    public Vector3 GetHitDirection () {
	    return hitDirection;
    }

    public void SetrecoilDirecion (Vector3 direction) {
	    recoilDirecion = direction;
    }

    public Vector3 GetrecoilDirecion () {
	    return recoilDirecion;
    }

    public float GetDeathTime () {
	    return deathTime;
    }

    public void SetHealth (float newHealth) {
        _health = newHealth;
    }

    public float GetHealth () {
        return _health;
    }
}
