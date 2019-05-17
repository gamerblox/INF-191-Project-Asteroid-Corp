using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildManager : MonoBehaviour
{
    //UI Elements
    public Text costText;
    public Text timeText;
    public Text marscapText;
    public Text leocapText;
    public InventoryManager inventory;

    //Rocket Info
    public List<RocketInfo> rocketList;
    RocketInfo selectRocket;

    void Start()
    {
        //Reset menu text
        costText.text = "---";
        timeText.text = "---";
        marscapText.text = "---";
        leocapText.text = "---";

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectAtlasRocket()
    {
        //Find the specific rocket info
        foreach (RocketInfo ri in rocketList)
        {
            if (ri.rocketName == "Atlas")
            {
                //Update menu text
                costText.text = ri.costString;
                timeText.text = ri.timeString;
                marscapText.text = ri.marscapString;
                leocapText.text = ri.leocapString;

                selectRocket = ri;

            }

        }

    }

    public void SelectSLSRocket()
    {
        //Find the specific rocket info
        foreach (RocketInfo ri in rocketList)
        {
            if (ri.rocketName == "SLS")
            {
                //Update menu text
                costText.text = ri.costString;
                timeText.text = ri.timeString;
                marscapText.text = ri.marscapString;
                leocapText.text = ri.leocapString;

                selectRocket = ri;

            }

        }

    }

    public void SelectFalconRocket()
    {
        //Find the specific rocket info
        foreach (RocketInfo ri in rocketList)
        {
            if (ri.rocketName == "Falcon")
            {
                //Update menu text
                costText.text = ri.costString;
                timeText.text = ri.timeString;
                marscapText.text = ri.marscapString;
                leocapText.text = ri.leocapString;

                selectRocket = ri;

            }

        }

    }

    public void SelectDeltaIVRocket()
    {
        //Find the specific rocket info
        foreach (RocketInfo ri in rocketList)
        {
            if (ri.rocketName == "DeltaIV")
            {
                //Update menu text
                costText.text = ri.costString;
                timeText.text = ri.timeString;
                marscapText.text = ri.marscapString;
                leocapText.text = ri.leocapString;

                selectRocket = ri;

            }

        }

    }

    public void PurchaseRocket()
    {

        inventory.AddRocket(selectRocket.rocketName);
        Debug.Log(selectRocket.rocketName);

    }

}
