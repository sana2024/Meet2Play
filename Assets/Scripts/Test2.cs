using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using UnityEngine.Events;

public class Test2 : MonoBehaviour
{

    [SerializeField] UnityEvent OnSocketDissconected;
    ISocket isocket;
    IClient iclient;
    ISession isession;

    public void Start()
    {
        isocket = PassData.isocket;
        isession = PassData.isession;
        iclient = PassData.iClient;

    }

    public void Update()
    {
        if(!isocket.IsConnected || isocket == null)
        {
             OnSocketDissconected.Invoke();
        }

    }

}
