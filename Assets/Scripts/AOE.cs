using UnityEngine;
using System.Collections;

public class AOE : MonoBehaviour
{

    public Combinaison combinaison;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            Enemy enemy = other.GetComponent<Enemy>();
            combinaison.effect.applyEffect(enemy, this.transform);
        }
    }
}
