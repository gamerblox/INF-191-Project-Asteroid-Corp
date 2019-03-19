using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameTimeManager : MonoBehaviour
{
    //public int timeSetting;
    //private float[] timeMult = { 0.1f, 0.25f, 0.5f, 1f, 2f };

    //public Text timeStepText;

    public bool DebugEnable;
    private bool timeActivate;

    //void Start()
    //{
    //    UpdateTime(timeSetting);

    //}

    void Update()
    {
        if (DebugEnable)
        {
            //if debug is on space pauses time
            if (Input.GetKeyDown(KeyCode.Space) && !timeActivate)
            {
                Time.timeScale = 0f;
                timeActivate = true;

            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                Time.timeScale = 1f;
                timeActivate = false;

            }

        }

    }

    ////updates everything affected by time by what index for setting inputted
    //void UpdateTime(int setting)
    //{
    //    timeSetting = setting;
    //    Time.timeScale = timeMult[timeSetting];
    //    int tempMult = timeSetting + 1;
    //    timeStepText.text = tempMult.ToString();

    //}

    //public void IncrementTimeSetting()
    //{
    //    Debug.Log(Time.timeScale);
    //    if ((timeSetting + 1) >= timeMult.Length)
    //    {
    //        return;

    //    }
    //    UpdateTime((timeSetting + 1));

    //}

    //public void DecrementTimeSetting()
    //{
    //    Debug.Log(Time.timeScale);
    //    if ((timeSetting - 1) < 0)
    //    {
    //        return;

    //    }
    //    UpdateTime((timeSetting - 1));

    //}

}
