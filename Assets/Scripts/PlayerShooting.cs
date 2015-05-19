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
    //Animator anim;

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
        //this.anim = this.playerScript.transform.FindChild("Mesh").GetComponent<Animator>();
    }

    void Update()
    {
        if (this.photonView.isMine)
        {
            timer += Time.deltaTime;

            if (timer >= timeBetweenBullets && Time.timeScale != 0)
            {
                if (Input.GetButton("Fire1"))
                {
                    this.shoot(false);
                }
                else if (Input.GetButton("Fire2"))
                {
                    this.shoot(true);
                }
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
    void shoot(bool allyShoot)
    {
        timer = 0f;

        //this.anim.SetTrigger("Shoot");
        gunAudio.Play();

        gunLight.enabled = true;

        gunParticles.Stop();
        gunParticles.Play();

        gunLine.enabled = true;
        gunLine.SetPosition(0, transform.position);

        shootRay.origin = transform.position;
        shootRay.direction = transform.forward;
        if (Physics.Raycast(shootRay, out shootHit, range))
        {
            Debug.Log(shootHit.ToString());
            if (allyShoot)
            {
                User user = shootHit.collider.GetComponent<User>();
                if (user)
                {
                    this.shootAlly(user);
                }
            }
            else
            {
                this.shootEnemy();
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
            this.photonView.RPC("shoot", PhotonTargets.Others, allyShoot);
    }

    [RPC]
    private void shootEnemy()
    {
        if (this.playerScript.currentCombinaison == null)
        {
            GameObject bullet = Instantiate(TweakManager.Instance.bullet, this.transform.position, Quaternion.LookRotation(this.transform.forward)) as GameObject;
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.combinaison = new Combinaison();
            bulletScript.combinaison.levelUp(this.playerScript.element);
        }
        else
        {
            int lvl = this.playerScript.currentCombinaison.getLevel();
            if (lvl >= 3)
            {
                StartCoroutine(this.playerScript.currentCombinaison.pattern.shoot(this.transform));
            }
            else
            {
                GameObject bullet = Instantiate(TweakManager.Instance.bullet, this.transform.position, Quaternion.LookRotation(this.transform.forward)) as GameObject;
                Bullet bulletScript = bullet.GetComponent<Bullet>();
                bulletScript.combinaison = this.playerScript.currentCombinaison;
            }
        }

        if (this.photonView.isMine)
            this.photonView.RPC("shootEnemy", PhotonTargets.Others);
    }

    [RPC]
    private void shootAlly(User user)
    {

        if (this.playerScript.currentCombinaison == null)
        {
            this.playerScript.currentCombinaison = new Combinaison();
        }

        user.updateCombinaison(this.playerScript.currentCombinaison);
        this.playerScript.combinaisonTransfered();

        if (this.photonView.isMine)
            this.photonView.RPC("shootAlly", PhotonTargets.Others, user);
    }
}
