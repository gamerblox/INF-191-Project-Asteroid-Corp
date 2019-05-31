using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertToggle : MonoBehaviour
{

    public void ToggleAlert()
    {

        this.gameObject.SetActive(false);

    }

    public void ActivateAlert()
    {

        this.gameObject.SetActive(true);

    }

}
