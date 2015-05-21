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
        setCurrentCombinaison(newCombinaison);
        StopCoroutine("combinaisonLifeCoroutine");
        StartCoroutine("combinaisonLifeCoroutine");
        Debug.Log(this.currentCombinaison.ToString());
    }

    public void combinaisonTransfered()
    {
        setCurrentCombinaison(new Combinaison(this.element));

        StopCoroutine("combinaisonLifeCoroutine");
    }

    private IEnumerator combinaisonLifeCoroutine()
    {
        yield return new WaitForSeconds(TweakManager.Instance.combinaisonTimeToLive);

        this.setCurrentCombinaison(null);
    }

    [RPC]
    public void setCurrentCombinaison(Combinaison newCombinaison)
    {
        this.currentCombinaison = newCombinaison;
        if (this.photonView.isMine)
        {
            this.photonView.RPC("setCurrentCombinaison", PhotonTargets.Others, newCombinaison);
        }
    }
}
