using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemSpawner : MonoBehaviour
{
    public GameObject[] possibleitems;
    public int numberOfItemsToSpawn = 54;
    public int numberOfOrders = 6;
    public UIManager ui;
    public GameObject roomba;
    public GameObject bigboi;
    public Vector3 enemyTrans;

    FMOD.Studio.EventInstance amb;
    FMOD.Studio.EventDescription power;
    FMOD.Studio.PARAMETER_DESCRIPTION pow;
    FMOD.Studio.PARAMETER_ID PDW;


    private string itemToGet = "None";
    //public GameObject[] spawnlocations;
    // Start is called before the first frame update
    void Start()
    {
        amb = FMODUnity.RuntimeManager.CreateInstance("event:/warehouse/Warehouse Amb");

        power = FMODUnity.RuntimeManager.GetEventDescription("event:/warehouse/Warehouse Amb");
        power.getParameterDescriptionByName("Danger", out pow);
        PDW = pow.id;

        amb.setParameterByID(PDW, 0f);
        amb.start();
        GameObject[] locationarray = GameObject.FindGameObjectsWithTag("Slot");
        List<GameObject> locationlist = new List<GameObject>();
        foreach (GameObject item in locationarray)
        {
            locationlist.Add(item);
        }
        for (int i = 0; i < 45; i++)
        {
            int x = i % possibleitems.Length;
            int r = Random.Range(0, locationlist.Count);
            Instantiate(possibleitems[x], locationlist[r].transform.position, Quaternion.identity);
            locationlist.RemoveAt(r);
        }
        /*
        foreach(GameObject item in locationarray)
        {
            Instantiate(possibleitems[Random.Range(0, possibleitems.Length)], item.transform.position,Quaternion.identity);
            //Destroy(item);
        }
        */

        SendOrder();
    }

    public void RemoveObject(GameObject obj)
    {
        Destroy(obj);
    }
    public void SendOrder()
    {
        int r = Random.Range(0, possibleitems.Length);
        itemToGet = possibleitems[r].GetComponent<GrabbableObject>().GetName();
        ui.itemList.text = "Orders Left: " + numberOfOrders + "\n" + "Find a " + itemToGet;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Item")
        {
            Debug.Log("destroyed " + collision.gameObject.name);
            if(collision.gameObject.GetComponent<GrabbableObject>().GetName() == itemToGet)
            {
                numberOfOrders--;
                SendOrder();
                TurnOffLights();
                SpawnEnemy();
            }
            Destroy(collision.gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {
        //increase();
        if(numberOfOrders == 0)
        {
            SceneManager.LoadScene(2);
        }
    }
    
    /*void increase()
    {
        if (Input.GetKey("l"))
        {
            numberOfOrders = 4;
            SpawnEnemy();
        }
    }*/

    void TurnOffLights()
    {
        GameObject[] lightsarray = GameObject.FindGameObjectsWithTag("Light");
        foreach(GameObject light in lightsarray)
        {
            light.GetComponent<FlickerMat>().isOn = false;
        }
        if (numberOfOrders == 5)
        {
            amb.setParameterByID(PDW, 1f);
        }
    }

    void SpawnEnemy()
    {
        if(numberOfOrders == 1)
        {
            return;
        }
        else if(numberOfOrders == 2)
        {
            Instantiate(bigboi, enemyTrans, Quaternion.identity);
        }
        else if(numberOfOrders <= 4)
        {
            Instantiate(roomba, enemyTrans, Quaternion.identity);
        }
    }
    private void OnDestroy()
    {
        amb.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
