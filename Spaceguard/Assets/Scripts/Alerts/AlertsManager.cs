using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertsManager : MonoBehaviour
{
    public GameObject menuAlert;
    public GameObject listAlert;
    public GameObject obj;
    public GameObject list;

    void Start()
    {

        //AddAlert("Alert 1\nDecember 23, 2004");
        //AddAlert("Alert 2\nJanuary 4, 2005");
        //AddAlert("Alert 3\nJuly 19, 2005");
        //AddAlert("Alert 4\nMay 6, 2006");
        //AddAlert("Alert 5\nAugust 16, 2008");
        //AddAlert("Alert 6\nSeptember 2, 2008");
        //AddAlert("Alert 7\nJanuary 9, 2013");
        //AddAlert("Alert 8\nDecember 8, 2013");
        //AddAlert("Alert 9\nJuly 29, 2016");
        //AddAlert("Alert 10\nOctober 14, 2016");

    }

    public void AddAlert(string data)
    {
        GameObject tempButton = Instantiate(menuAlert);
        tempButton.transform.SetParent(listAlert.transform);
        tempButton.GetComponentInChildren<Text>().text = data;

    }

    public void DisplayAlert(string data)
    {
        GameObject tempButton = Instantiate(obj);
        tempButton.transform.SetParent(list.transform);
        tempButton.GetComponentInChildren<Text>().text = data;

    }

    void DeleteAlert()
    {


    }

}
