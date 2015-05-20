﻿using UnityEngine;
using System.Collections;

public class laserPoint : MonoBehaviour
{

    public bool on = true;

    void Update()
    {
        if (on)
        {
            transform.LookAt(Camera.main.transform.position);
            transform.localScale = Vector3.one * Random.Range(0.3f, 0.5f);
            Color laserColor = Color.red;
            laserColor.a = Random.Range(0.2f, 1.0f);
            renderer.material.SetColor("_TintColor", laserColor);
            renderer.enabled = true;
        }
        else
        {
            renderer.enabled = false;
        }
    }
}
