using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Events;
using System;
using UnityEngine.SceneManagement;

public class Test : MonoBehaviour
{
    [SerializeField] Button click;


    bool MyFunctionCalled = false;

    void Update()
    {
        if(MyFunctionCalled == true)
        {
            Debug.Log("called");
            MyFunctionCalled = false;
            
           
     }

    }

    public void onclick()
    {
        MyFunctionCalled = true;
        
    }



}
