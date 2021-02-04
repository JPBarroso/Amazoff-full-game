using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class GrabbableObject : MonoBehaviour
{
    Vector3 _initialLocation;
    [SerializeField]
    bool isGrabbed;
    [SerializeField]
    string objName = "test";
    [SerializeField]
    int objType = 0;
    Rigidbody rb;

    FMOD.Studio.EventInstance pickup;
    FMOD.Studio.EventDescription Object;
    FMOD.Studio.PARAMETER_DESCRIPTION obj;
    FMOD.Studio.PARAMETER_ID OID;
    // Start is called before the first frame update
    void Start()
    {
        _initialLocation = transform.position;
        rb = GetComponent<Rigidbody>();
        pickup = FMODUnity.RuntimeManager.CreateInstance("event:/Player/Pickup");
        Object = FMODUnity.RuntimeManager.GetEventDescription("event:/player/Pickup");
        Object.getParameterDescriptionByName("Object", out obj);
        OID = obj.id;


        pickup.setParameterByID(OID, objType);

    }

    // Update is called once per frame
    void Update()
    {
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(pickup, GetComponent<Transform>(), GetComponent<Rigidbody>());
    }
    public void Grab(Transform player)
    {
        isGrabbed = true;
        Debug.Log("Grabbed " + transform.name);
        rb.isKinematic = true;
        transform.SetParent(player);
        pickup.start();
    }
    public void Drop()
    {
        isGrabbed = false;
        Debug.Log(transform.name + " Released");
        rb.isKinematic = false;
        transform.SetParent(null);
    }

    public string GetName()
    {
        return objName;
    }
}
