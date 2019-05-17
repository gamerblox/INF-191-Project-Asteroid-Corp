using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InventoryManager : MonoBehaviour
{
    public GameObject menuRocket;
    public GameObject listRocket;

    void Start()
    {

    }

    public void AddRocket(string data)
    {
        GameObject tempButton = Instantiate(menuRocket);
        tempButton.transform.SetParent(listRocket.transform);
        tempButton.GetComponentInChildren<Text>().text = data;
    }

}

