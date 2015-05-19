using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    public Combinaison combinaison;
    public float speed = 10;
    public int damagePerShot = 20;


    void Update()
    {
        this.transform.Translate(this.transform.forward * this.speed * Time.deltaTime, Space.World);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag == "Enemy")
        {
            Enemy enemyScript = other.collider.GetComponent<Enemy>();
            enemyScript.takeDamage(combinaison.element, this.damagePerShot);
            if (combinaison.effect != null)
                StartCoroutine(combinaison.effect.applyEffect(enemyScript, this.transform));
        }
    }
}
