using UnityEngine;

public class PlayerShooting : Photon.MonoBehaviour
{

    public float timeBetweenBullets = 0.15f;
    public float range = 100f;


    float timer;
    Ray shootRay;
    RaycastHit shootHit;
    //int shootableMask;
    ParticleSystem gunParticles;
    LineRenderer gunLine;
    AudioSource gunAudio;
    Light gunLight;
    float effectsDisplayTime = 0.2f;

    public GameObject explosion;

    User playerScript;


    void Awake()
    {
        //shootableMask = LayerMask.GetMask("Shootable");
        gunParticles = GetComponent<ParticleSystem>();
        gunLine = GetComponent<LineRenderer>();
        gunAudio = GetComponent<AudioSource>();
        gunLight = GetComponent<Light>();
    }

    void Start()
    {
        playerScript = transform.parent.GetComponent<User>();
    }

    void Update()
    {
        if (this.photonView.isMine)
        {
            timer += Time.deltaTime;

            if (Input.GetButton("Fire1") && timer >= timeBetweenBullets && Time.timeScale != 0)
            {
                shoot();
            }

            if (timer >= timeBetweenBullets * effectsDisplayTime)
            {
                this.disableEffects();
            }
        }
    }

    [RPC]
    public void disableEffects()
    {
        gunLine.enabled = false;
        gunLight.enabled = false;

        if (this.photonView.isMine)
            this.photonView.RPC("disableEffects", PhotonTargets.Others);
    }

    [RPC]
    void shoot()
    {
        timer = 0f;

        transform.parent.GetComponentInChildren<Animator>().SetTrigger("Shoot");
        gunAudio.Play();

        gunLight.enabled = true;

        gunParticles.Stop();
        gunParticles.Play();

        gunLine.enabled = true;
        gunLine.SetPosition(0, transform.position);

        shootRay.origin = transform.position;
        shootRay.direction = transform.forward;
        Debug.DrawRay(transform.position, transform.position + transform.forward * 10, Color.red, 0.5f);
        if (Physics.Raycast(shootRay, out shootHit, range))
        {
            User user = shootHit.collider.GetComponent<User>();
            if (user)
            {
                this.playerScript.currentCombinaison.transfertTo(user);
            }
            else
            {
                if (this.playerScript.currentCombinaison.pattern != null)
                    StartCoroutine(this.playerScript.currentCombinaison.pattern.shoot(this.transform));
            }

            //to remove
            gunLine.SetPosition(1, shootHit.point);
            if (this.explosion)
            {
                GameObject explosion = Instantiate(this.explosion, shootHit.point, Quaternion.identity) as GameObject;
                ParticleSystem emitter = explosion.GetComponent<ParticleSystem>();
                Destroy(explosion, emitter.duration + emitter.startLifetime);
                Light light = explosion.GetComponent<Light>();
                Destroy(light, 0.1f);

            }
        }
        else
        {
            gunLine.SetPosition(1, shootRay.origin + shootRay.direction * range);
        }

        if (this.photonView.isMine)
            this.photonView.RPC("shoot", PhotonTargets.Others);
    }
}
