using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

    public void resetCombinaison()
    {
        pattern = null;
        effect = null;
        element = ELEMENTS.COUNT;
    }

    public void transfertTo(User user)
    {
    }
}
