using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabManager : MonoBehaviour
{
    public float grabDistance = 2f;
    public LayerMask toIgnore;
    GameObject grabbed;
    UIManager ui;
    // Start is called before the first frame update
    void Start()
    {
        ui = GetComponentInParent<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * grabDistance, Color.red);
        if (Input.GetMouseButtonDown(0) && grabbed == null)
        {
            RaycastHit hit;
            Ray forwardRay = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(forwardRay, out hit, grabDistance))
            {
                if (hit.transform.GetComponent<GrabbableObject>())
                {
                    ui.ShowName(hit.transform.GetComponent<GrabbableObject>().GetName());
                    grabbed = hit.transform.gameObject;
                    grabbed.GetComponent<GrabbableObject>().Grab(this.transform);
                }
            }
        }
        else if (Input.GetMouseButtonDown(0) && grabbed != null)
        {
            grabbed.GetComponent<GrabbableObject>().Drop();
            ui.HideName();
            grabbed = null;
        }
        else if(grabbed == null)
        {
            RaycastHit hit;
            Ray forwardRay = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(forwardRay, out hit, grabDistance,~toIgnore))
            {
                if (hit.transform.GetComponent<GrabbableObject>())
                {
                    ui.ShowGrabItem(hit.transform.GetComponent<GrabbableObject>().GetName());
                }
                else
                {
                    ui.HideName();
                }
            }
            else
            {
                ui.HideName();
            }
        }

    }
    private void FixedUpdate()
    {
       
    }
}
