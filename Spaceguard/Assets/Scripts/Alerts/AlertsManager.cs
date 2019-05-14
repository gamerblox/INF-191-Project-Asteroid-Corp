using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertsManager : MonoBehaviour
{
    public GameObject menuAlert;
    public GameObject listAlert;

    void Start()
    {
        AddAlert("New asteroid detected!");
        AddAlert("Test alert.");

    }

    void AddAlert(string data)
    {
        GameObject tempButton = Instantiate(menuAlert);
        tempButton.transform.SetParent(listAlert.transform);
        tempButton.GetComponentInChildren<Text>().text = data;

    }

    void DeleteAlert()
    {


    }

}
