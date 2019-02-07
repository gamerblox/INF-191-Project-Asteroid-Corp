using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public List<GameObject> menus = new List<GameObject>();

    void Start()
    {
        MainSwitchMenu();

    }

    void Update()
    {
        
    }

    public void MainSwitchMenu()
    {
        ClearAllMenus();
        foreach (GameObject menu in menus)
        {
            if (menu.name == "MainMenu")
            {
                menu.SetActive(true);
                return;

            }

        }

    }

    public void SimualtionSwitchMenu()
    {
        ClearAllMenus();
        foreach (GameObject menu in menus)
        {
            if (menu.name == "SimulationMenu")
            {
                menu.SetActive(true);
                return;

            }

        }

    }

    public void InventorySwitchMenu()
    {
        ClearAllMenus();
        foreach (GameObject menu in menus)
        {
            if (menu.name == "InventoryMenu")
            {
                menu.SetActive(true);
                return;

            }

        }

    }

    public void AlertsSwitchMenu()
    {
        ClearAllMenus();
        foreach (GameObject menu in menus)
        {
            if (menu.name == "AlertsMenu")
            {
                menu.SetActive(true);
                return;

            }

        }

    }

    void ClearAllMenus()
    {
        foreach (GameObject menu in menus)
        {
            menu.SetActive(false);

        }

    }

}
