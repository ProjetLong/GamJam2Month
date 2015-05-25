using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class User : Character
{
    #region Public properties

    #endregion

    public int resources = 50;
    public int score = 0;
    public List<Weapon> availableWeapons;
    public Combinaison currentCombinaison;

    protected override void Start()
    {
        base.Start();
        this.updateHealth();
        this.assignElement();
    }

    private void assignElement()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int rng = 0;

        if (players.Length <= 4)
        {
            int[] alreadyAssignedElements = new int[players.Length];

            for (int i = 0; i < players.Length; ++i)
            {
                alreadyAssignedElements[i] = (int)players[i].GetComponent<User>().element;
            }

            while (alreadyAssignedElements.Contains(rng))
            {
                rng = Random.Range(0, (int)Combinaison.ELEMENTS.COUNT - 1);

            }
        }
        else
        {
            rng = Random.Range(0, (int)Combinaison.ELEMENTS.COUNT - 1);
        }

        this.element = (Combinaison.ELEMENTS)rng;
    }

    public void respawn()
    {

    }

    public void reload()
    {

    }

    protected override void modifyHealth(int amount)
    {
        base.modifyHealth(amount);
        this.updateHealth();
    }

    private void updateHealth()
    {
        GUIManager_Game.Instance.setHealth(this.health, this.maxHealth);
    }

    public void changeWeapon(Weapon weapon)
    {
        this.weapon = weapon;
    }

    public void spendResources(int amount)
    {
        this.resources -= amount;
        if (this.resources < 0)
            this.resources = 0;
    }

    public void updateCombinaison(Combinaison newCombinaison)
    {
        serializeForRPCCombinaison(newCombinaison);
        StopCoroutine("combinaisonLifeCoroutine");
        StartCoroutine("combinaisonLifeCoroutine");
        Debug.Log(this.currentCombinaison.ToString());
        GUIManager_Game.Instance.updateCombinaisonState(currentCombinaison);
    }

    public void combinaisonTransfered()
    {
        serializeForRPCCombinaison(null);

        StopCoroutine("combinaisonLifeCoroutine");
    }

    private IEnumerator combinaisonLifeCoroutine()
    {
        Debug.Log("Combinaison will die in " + TweakManager.Instance.combinaisonTimeToLive.ToString());
        yield return new WaitForSeconds(TweakManager.Instance.combinaisonTimeToLive);

        this.serializeForRPCCombinaison(null);
        Debug.Log(this.currentCombinaison);
    }

    public void serializeForRPCCombinaison(Combinaison newCombinaison)
    {
        Combinaison.ELEMENTS first = newCombinaison != null
            ? newCombinaison.element
            : Combinaison.ELEMENTS.COUNT;
        Combinaison.ELEMENTS second = Combinaison.ELEMENTS.COUNT;
        Combinaison.ELEMENTS third = Combinaison.ELEMENTS.COUNT;

        // 2nd element
        if (newCombinaison != null
            && newCombinaison.effect != null)
        {
            if (newCombinaison.effect.GetType() == typeof(AirEffect))
            {
                second = Combinaison.ELEMENTS.AIR;
            }
            else if (newCombinaison.effect.GetType() == typeof(PoisonEffect))
            {
                second = Combinaison.ELEMENTS.POISON;
            }
            else if (newCombinaison.effect.GetType() == typeof(FireEffect))
            {
                second = Combinaison.ELEMENTS.FIRE;
            }
            else if (newCombinaison.effect.GetType() == typeof(IceEffect))
            {
                second = Combinaison.ELEMENTS.ICE;
            }

            // 3rd element
            if (newCombinaison != null
                && newCombinaison.pattern != null)
            {
                if (newCombinaison.pattern.GetType() == typeof(AirPattern))
                {
                    third = Combinaison.ELEMENTS.AIR;
                }
                else if (newCombinaison.pattern.GetType() == typeof(FirePattern))
                {
                    third = Combinaison.ELEMENTS.FIRE;
                }
                else if (newCombinaison.pattern.GetType() == typeof(IcePattern))
                {
                    third = Combinaison.ELEMENTS.ICE;
                }
                else if (newCombinaison.pattern.GetType() == typeof(PoisonPattern))
                {
                    third = Combinaison.ELEMENTS.POISON;
                }
            }
        }

        setCurrentCombinaison((int)first, (int)second, (int)third);
    }

    [RPC]
    public void setCurrentCombinaison(int first, int second, int third)
    {
        this.currentCombinaison = new Combinaison((Combinaison.ELEMENTS)first,
            (Combinaison.ELEMENTS)second, (Combinaison.ELEMENTS)third);
        if (this.photonView.isMine)
        {
            this.photonView.RPC("setCurrentCombinaison", PhotonTargets.Others, first, second, third);
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log(this.name + ": "
                + (this.currentCombinaison != null
                    ? this.currentCombinaison.ToString()
                    : "no combinaison"));
        }
    }
}
