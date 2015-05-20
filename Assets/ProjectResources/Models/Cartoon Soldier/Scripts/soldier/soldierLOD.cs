using UnityEngine;
using System.Collections;

public class soldierLOD : MonoBehaviour
{
    public Transform soldierCamera;
    public GameObject[] lodPrefabs;
    public float[] lodDistances;
    public GameObject soldierCharacter;

    private int currentLod;

    public void Update()
    {
        float lodDistance = Vector3.Distance(soldierCamera.position, soldierCharacter.transform.position);
        int selectLod = lodPrefabs.Length - 1;
        for (var i = 0; i < lodDistances.Length; i++)
        {
            if (i == 0)
            {
                if (lodDistance < lodDistances[i])
                {
                    selectLod = 0;
                }
            }
            else
            {
                if (lodDistance < lodDistances[i] && lodDistance > lodDistances[i - 1])
                {
                    selectLod = i;
                }
            }
        }
        if (selectLod != currentLod)
        {
            SetLod(selectLod);
        }
    }

    public void SetLod(int lod)
    {
        Destroy(soldierCharacter);
        GameObject newLOD = Instantiate(lodPrefabs[lod], transform.position, transform.rotation) as GameObject;
        newLOD.transform.parent = transform;
        newLOD.name = "soldierCharacter";
        soldierCharacter = newLOD;
        currentLod = lod;
    }
}
