using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUIManager_Game : MonoBehaviour
{
    private static GUIManager_Game instance;
    public static GUIManager_Game Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (GUIManager_Game.instance == null)
        {
            GUIManager_Game.instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private Slider healthBar;
    private Text healthText;
    private GameObject combinaisonState;
    private GameObject pattern;
    private GameObject effect;
    private GameObject element;

    void Start()
    {
        GameObject healthBar = GameObject.Find("HealthBar");
        this.healthBar = healthBar.GetComponent<Slider>();
        this.healthText = healthBar.transform.FindChild("Text").GetComponent<Text>();
        combinaisonState = GameObject.Find("CombinaisonState");
    }

    public void setHealth(int currentHealth, int maxHealth)
    {
        float healthProgress = currentHealth / (float)maxHealth;
        this.healthBar.value = healthProgress;
        this.healthText.text = currentHealth + "/" + maxHealth + " " + (int)(healthProgress * 100) + "%";
    }

    public void updateCombinaisonState(Combinaison combinaison)
    {
        int lvl = combinaison.getLevel();
        switch (lvl)
        {
            case 0:
                //not possible
                break;
            case 1:
                /*Destroy(this.effect);
                this.effect = Instantiate(TweakManager.Instance.)*/
                break;
            case 2:
                break;
            case 3:
                break;
        }
    }
}
