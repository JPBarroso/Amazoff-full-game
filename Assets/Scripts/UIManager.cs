using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text aimingItem;
    public Text itemList;
    string listedItems;
    // Start is called before the first frame update
    void Start()
    {
        aimingItem.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UpdateList(List<GrabbableObject> list)
    {
        foreach(GrabbableObject item in list)
        {
            string tempitem = item.name + "\n";
            listedItems += tempitem;
        }
    }
    public void ShowGrabItem(string name)
    {
        aimingItem.text = "Pickup " + name;
    }
    public void ShowName(string name)
    {
        aimingItem.text = name;
    }
    public void HideName()
    {
        aimingItem.text = "";
    }
}
