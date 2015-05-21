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
    private Image pattern;
    private GameObject effect;
    private Material elementColor;

    void Start()
    {
        GameObject healthBar = GameObject.Find("HealthBar");
        this.healthBar = healthBar.GetComponent<Slider>();
        this.healthText = healthBar.transform.FindChild("Text").GetComponent<Text>();
        combinaisonState = GameObject.Find("CombinaisonState");
        this.elementColor = this.combinaisonState.transform.FindChild("Element").GetComponent<Renderer>().material;
        this.pattern = this.combinaisonState.transform.FindChild("Pattern").GetComponent<Image>();
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
        if (lvl >= 1)
        {
            switch (combinaison.element)
            {
                case Combinaison.ELEMENTS.AIR:
                    this.elementColor.color = TweakManager.Instance.airCombinaisonState.element;
                    break;
                case Combinaison.ELEMENTS.FIRE:
                    this.elementColor.color = TweakManager.Instance.fireCombinaisonState.element;
                    break;
                case Combinaison.ELEMENTS.ICE:
                    this.elementColor.color = TweakManager.Instance.iceCombinaisonState.element;
                    break;
                case Combinaison.ELEMENTS.POISON:
                    this.elementColor.color = TweakManager.Instance.poisonCombinaisonState.element;
                    break;
                default:
                    this.elementColor.color = Color.gray;
                    break;
            }
        }
        if (lvl >= 2)
        {
            Destroy(this.effect.gameObject);
            switch (combinaison.element)
            {
                case Combinaison.ELEMENTS.AIR:
                    this.effect = Instantiate(TweakManager.Instance.airCombinaisonState.effect, Vector3.zero, Quaternion.identity) as GameObject;
                    break;
                case Combinaison.ELEMENTS.FIRE:
                    this.effect = Instantiate(TweakManager.Instance.fireCombinaisonState.effect, Vector3.zero, Quaternion.identity) as GameObject;
                    break;
                case Combinaison.ELEMENTS.ICE:
                    this.effect = Instantiate(TweakManager.Instance.iceCombinaisonState.effect, Vector3.zero, Quaternion.identity) as GameObject;
                    break;
                case Combinaison.ELEMENTS.POISON:
                    this.effect = Instantiate(TweakManager.Instance.poisonCombinaisonState.effect, Vector3.zero, Quaternion.identity) as GameObject;
                    break;
            }
            if (this.effect)
                this.effect.transform.parent = this.combinaisonState.transform;
        }
        if (lvl >= 3)
        {
            switch (combinaison.element)
            {
                case Combinaison.ELEMENTS.AIR:
                    this.pattern.sprite = TweakManager.Instance.airCombinaisonState.pattern;
                    break;
                case Combinaison.ELEMENTS.FIRE:
                    this.pattern.sprite = TweakManager.Instance.fireCombinaisonState.pattern;
                    break;
                case Combinaison.ELEMENTS.ICE:
                    this.pattern.sprite = TweakManager.Instance.iceCombinaisonState.pattern;
                    break;
                case Combinaison.ELEMENTS.POISON:
                    this.pattern.sprite = TweakManager.Instance.poisonCombinaisonState.pattern;
                    break;
                default:
                    this.pattern.sprite = null;
                    break;
            }
        }
    }
}
