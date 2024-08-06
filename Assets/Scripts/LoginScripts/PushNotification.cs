using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OneSignalSDK;

public class PushNotification : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
         
        OneSignal.Initialize("32981917-3b1d-44c1-bc23-d817a1114f02");
        OneSignal.Login(PassData.isession.UserId);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
