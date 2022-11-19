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
    Ping p;

    bool MyFunctionCalled = false;



    void Update()
    {
        StartCoroutine(CheckRoutine());

    }

    public void onclick()
    {
 
        
    }

    IEnumerator CheckRoutine()
    {
        UnityWebRequest request = new UnityWebRequest("https://www.google.com/");
        yield return request.SendWebRequest();

        Debug.Log(request.result);
    }


}
