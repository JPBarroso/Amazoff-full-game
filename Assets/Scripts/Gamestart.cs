using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamestart : MonoBehaviour
{
    FMOD.Studio.EventInstance gamestart; 

    // Start is called before the first frame update
    void Start()
    {
        gamestart = FMODUnity.RuntimeManager.CreateInstance("Event:/Beff Jayzos/Game start"); 
        gamestart.start();
    }

    // Update is called once per frame
    void Update()
    {
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(gamestart, GetComponent<Transform>(), GetComponent<Rigidbody>());
    }
}
