using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Events;
using System;

public class Test : MonoBehaviour
{
    bool ReconnectFlag = false;

    private void Update()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("internet dissconected");
            ReconnectFlag = true;
        }
        else
        {
            if(ReconnectFlag == true)
            {
               
               Debug.Log("reconnected");
                return;

            }
 
        }
    }



}
