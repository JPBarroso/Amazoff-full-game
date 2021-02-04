using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FlickerMat : MonoBehaviour
{
    public Material material;
    public bool isFlickering = true;
    public float tillFlicker = 2f;
    public float maxFlickerTimer = 0.5f;
    public float flickerTimer = 0.5f;
    public int flickerAmount = 10;
    public int current = 0;
    public bool isOn = true;

    private float time = 2f;
    private bool emit = false; 
    public static bool executedThisFrame = false;
    private Light light;

    void Start()
    {
        light = GetComponentInChildren<Light>();
    }
    void Update()
    {
        if(isFlickering && isOn)
            UpdateLight();
        else if(!isOn)
        {
            material.DisableKeyword("_EMISSION");
            light.enabled = false;
        }
        /*
        if (!isFlickering || executedThisFrame) return;
        if (time <= 0)
        {
            emit = !emit;
            if (emit)
            {
                material.EnableKeyword("_EMISSION");
                light.enabled = true;
                time += Random.value * 3;
            }
            else
            {
                material.DisableKeyword("_EMISSION");
                light.enabled = false;
                time += 0.1f;
            }
        }
        time -= Time.deltaTime;
        executedThisFrame = true;
        */
    }

    private void LateUpdate()
    {
       // executedThisFrame = false;
    }

    void UpdateLight()
    {
        if(tillFlicker <= 0)
        {
            if (current < flickerAmount)
            {
                if (current % 2 == 0 && flickerTimer <= 0)
                {
                    material.DisableKeyword("_EMISSION");
                    light.enabled = false;
                    flickerTimer = maxFlickerTimer;
                    current += 1;
                }
                else if (flickerTimer <= 0)
                {
                    material.EnableKeyword("_EMISSION");
                    light.enabled = true;
                    flickerTimer = maxFlickerTimer;
                    current += 1;
                }
                flickerTimer -= Time.deltaTime;
            }
            else
            {
                tillFlicker = 10f;
                current = 0;
            }
        }
        tillFlicker -= Time.deltaTime;
    }
}
