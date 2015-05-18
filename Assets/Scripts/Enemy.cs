using UnityEngine;
using System.Collections;

public class Enemy : Character
{
    #region Public properties
    [Header("Links")]
    [SerializeField]
    [Tooltip("Death FX")]
    GameObject _deathFXPrefab;
    #endregion

    public enum Type
    {
        DISTANCE,
        CAC
    };

    #region Members
    public Type enemyType = Type.CAC;
    public Transform target;
    public Transform playerTarget;
    private bool isWalking = false;
    private bool canAttack = true;
    public float attackCooldown = 1.5f;

    private NavMeshAgent navAgent;
    #endregion

    protected override void Start()
    {
        base.Start();
        this.navAgent = this.GetComponent<NavMeshAgent>();
        this.navAgent.stoppingDistance = this.range;
        this.navAgent.speed = this.speed * this.speedRate;
        this.navAgent.angularSpeed = this.rotationSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (hasTarget())
        {
            //facing target
            //Vector3 targetPostition = new Vector3(target.position.x, this.transform.position.y, target.position.z);
            //this.transform.LookAt(targetPostition);

            //check range and move if target is too far
            follow();
            //Debug.Log(navAgent.remainingDistance);
            if (navAgent.remainingDistance <= this.range)
            {
                if (this.isWalking)
                {
                    isWalking = false;
                    this.anim.SetBool("isWalking", false);
                }
                attack();
            }
            else
            {
                if (!isWalking)
                {
                    this.isWalking = true;
                    this.anim.SetBool("isWalking", true);
                }
            }
        }
    }

    private bool hasTarget()
    {
        return this.target != null;
    }

    private void follow()
    {
        if (hasTarget())
        {
            //this.moveForward(Time.deltaTime);
            this.navAgent.SetDestination(this.target.position);
            /*if (!isWalking)
            {
                this.isWalking = true;
                this.anim.SetBool("isWalking", true);
            }*/
        }
    }

    private void attack()
    {
        if (hasTarget() && this.canAttack)
        {
            this.anim.SetTrigger("isAttacking");
            Character user = this.target.GetComponent<Character>();
            if (user)
            {
                user.takeDamage(this.element, this.weapon.damageValue);
            }
            StartCoroutine(this.setAttackOnCooldown());
        }
    }

    private IEnumerator setAttackOnCooldown()
    {
        this.canAttack = false;
        yield return new WaitForSeconds(this.attackCooldown);
        this.canAttack = true;
    }

    protected override void death()
    {
        GameObject deathFXObject = GameObject.Instantiate(_deathFXPrefab, transform.position, transform.rotation) as GameObject;
        Destroy(deathFXObject, 1);
        base.death();

        EntitiesManager.Instance.zombieDied(this.gameObject);
        Destroy(this.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (this.target)
        {
            return;
        }
        if (other.tag.Equals("Player"))
        {
            this.target = other.gameObject.transform;
            this.playerTarget = this.target;
        }
    }

    public void enterConfusion()
    {
        if (target == null)
            return;

        GameObject nearestZombie = EntitiesManager.Instance.getNearestZombie(this.transform);
        target = nearestZombie.transform;
    }

    public void snapOutOfConfusion()
    {
        if (target == null
            || playerTarget == null)
            return;

        target = playerTarget;
    }
}
