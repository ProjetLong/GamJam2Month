using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class Combinaison
{
    public enum ELEMENTS { FIRE = 0, AIR, POISON, ICE, COUNT };

    #region Members
    public ELEMENTS element = ELEMENTS.COUNT;
    public IShotEffect effect;
    public IShootPattern pattern;
    #endregion

    public int getLevel()
    {
        if (pattern != null)
        {
            return 3;
        }
        else if (effect != null)
        {
            return 2;
        }
        else if (element != ELEMENTS.COUNT)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    public void levelUp(ELEMENTS element)
    {
        int lvl = this.getLevel();
        switch (lvl)
        {
            case 0:
                this.element = element;
                break;
            case 1:
                switch (element)
                {
                    case ELEMENTS.AIR:
                        this.effect = new AirEffect();
                        break;
                    case ELEMENTS.FIRE:
                        this.effect = new FireEffect();
                        break;
                    case ELEMENTS.ICE:
                        this.effect = new IceEffect();
                        break;
                    case ELEMENTS.POISON:
                        this.effect = new PoisonEffect();
                        break;
                }
                break;
            case 2:
                switch (element)
                {
                    case ELEMENTS.AIR:
                        this.pattern = new AirPattern();
                        break;
                    case ELEMENTS.FIRE:
                        this.pattern = new FirePattern();
                        break;
                    case ELEMENTS.ICE:
                        this.pattern = new IcePattern();
                        break;
                    case ELEMENTS.POISON:
                        this.pattern = new PoisonPattern();
                        break;
                }
                break;
            case 3:
                return;
        }
    }

    public void resetCombinaison()
    {
        pattern = null;
        effect = null;
        element = ELEMENTS.COUNT;
    }

    public void transfertTo(User user)
    {
        user.currentCombinaison = this;
    }

}
