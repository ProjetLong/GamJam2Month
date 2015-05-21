using UnityEngine;

public class PlayerShooting : Photon.MonoBehaviour
{
    public float timeBetweenBullets = 0.15f;
    public float range = 100f;

    float timer;
    Ray shootRay;
    RaycastHit shootHit;
    AudioSource gunAudio;

    public User playerScript;
    public soldierMovement movementScript;
    //Animator anim;

    void Awake()
    {
        gunAudio = GetComponent<AudioSource>();
    }

    void Start()
    {
    }

    void Update()
    {
        if (this.photonView.isMine)
        {
            timer += Time.deltaTime;

            if (timer >= timeBetweenBullets && Time.timeScale != 0)
            {
                if (!movementScript.isRunning)
                {
                    if (Input.GetButton("Fire1"))
                    {
                        this.shoot(false);
                    }
                    else if (Input.GetButtonDown("Fire2"))
                    {
                        if (this.playerScript.currentCombinaison == null)
                        {
                            this.playerScript.serializeForRPCCombinaison(new Combinaison(this.playerScript.element));
                        }

                        if (this.playerScript.currentCombinaison.getLevel() < 3)
                        {
                            this.shoot(true);
                        }
                    }
                }
            }
        }
    }

    [RPC]
    void shoot(bool allyShoot)
    {
        timer = 0f;

        gunAudio.Play();

        shootRay.origin = transform.position;
        shootRay.direction = transform.forward;
        if (Physics.Raycast(shootRay, out shootHit, range))
        {
            //Debug.Log(shootHit.ToString());
            if (allyShoot)
            {
                User user = shootHit.collider.GetComponent<User>();
                if (user)
                {
                    this.shootAlly(user.name);
                }
            }
            else
            {
                this.shootEnemy();
            }
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
            bulletScript.combinaison = new Combinaison(this.playerScript.element);
            this.playerScript.currentCombinaison = bulletScript.combinaison;
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
    private void shootAlly(string username)
    {
        User user = GameObject.Find(username).GetComponent<User>();

        if (this.playerScript.currentCombinaison == null)
        {
            this.playerScript.serializeForRPCCombinaison(new Combinaison(this.playerScript.element));
        }
        else
        {
            Combinaison lvlUpCombinaison = this.playerScript.currentCombinaison;
            lvlUpCombinaison.levelUp(this.playerScript.element);
            this.playerScript.serializeForRPCCombinaison(lvlUpCombinaison);
        }

        user.updateCombinaison(this.playerScript.currentCombinaison);
        this.playerScript.combinaisonTransfered();

        if (this.photonView.isMine)
            this.photonView.RPC("shootAlly", PhotonTargets.Others, username);
    }
}
