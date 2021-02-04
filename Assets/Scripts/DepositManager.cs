using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepositManager : MonoBehaviour
{
    public List<GrabbableObject> deposits = new List<GrabbableObject>();
    public UIManager player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision col)
    {
        deposits.Add(col.gameObject.GetComponent<GrabbableObject>());
        player.UpdateList(deposits);
    }
    private void OnCollisionExit(Collision col)
    {
        deposits.Remove(col.gameObject.GetComponent<GrabbableObject>());
        player.UpdateList(deposits);
    }
}
