﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alert : MonoBehaviour
{

    public GameObject alertimage;
    
    public void RemoveAlert()
    {

        Destroy(this.gameObject);

    }

}