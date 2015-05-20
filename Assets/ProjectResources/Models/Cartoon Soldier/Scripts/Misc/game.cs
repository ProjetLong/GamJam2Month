using UnityEngine;
using System.Collections;

public class game : MonoBehaviour
{
    public GameObject soldier;
    public GameObject soldierPrefab;
    public GameObject sentryGun;
    public GameObject sentryGunPrefab;
    public bool resetMenu = false;
    public GUIText menu1;
    public GUIText menu2;
    public bool lockCursorCheck = true;
    public bool showCursor = false;

    public void Start()
    {
        Screen.lockCursor = lockCursorCheck;
        Screen.showCursor = showCursor;
    }
    public void Update()
    {
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Break();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            resetMenu = true;
        }
        if (resetMenu)
        {
            menu1.text = "Reset menu:";
            menu2.text = "(1) Reset soldier. (2) Reset sentry gun.";
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ResetSoldier();
                resetMenu = false;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ResetSentryGun();
                resetMenu = false;
            }
        }
        else
        {
            menu1.text = "Soldier scripts v0.93 sample scene";
            menu2.text = "(Under development - report bugs: dogzerx@hotmail.com)";
        }
        Screen.lockCursor = true;
    }

    public void ResetSoldier()
    {
        Destroy(soldier);
        soldier = Instantiate(soldierPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        soldier.name = "soldier3rdPerson"; //This is so the sentry gun will recognize & shoot him.
    }

    public void ResetSentryGun()
    {
        Destroy(sentryGun);
        sentryGun = Instantiate(sentryGunPrefab, new Vector3(0.0f, 0.0f, 3.0f), Quaternion.identity) as GameObject;

        // Translation add
        Vector3 oldRotationEulerAngle = sentryGun.transform.rotation.eulerAngles;
        oldRotationEulerAngle.y = 90.0f;
        sentryGun.transform.rotation = Quaternion.Euler(oldRotationEulerAngle);
        //sentryGun.transform.rotation.eulerAngles.y = 90;
        sentryGun.name = "sentryGun"; //This is so the sentry gun will recognize & shoot him.
    }
}
