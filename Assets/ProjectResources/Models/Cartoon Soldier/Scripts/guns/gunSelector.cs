using UnityEngine;
using System.Collections;

public class gunSelector : MonoBehaviour
{
    void Awake()
    {
        SelectWeapon(0);
    }

    void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            SelectWeapon(0);
        }
        if (Input.GetKeyDown("2"))
        {
            SelectWeapon(1);
        }
    }

    void SelectWeapon(int index)
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            if (i == index)
            {
                transform.GetChild(i).gameObject.SetActiveRecursively(true);
            }
            else
            {
                transform.GetChild(i).gameObject.SetActiveRecursively(false);
            }
        }
    }
}
