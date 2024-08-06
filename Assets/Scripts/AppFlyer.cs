using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppsFlyerSDK;

public class AppFlyer : MonoBehaviour
{
    public static AppFlyer Instance;

    private void Awake()
    {

        if(Instance == null)
        {
            Instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        AppsFlyer.initSDK("U9qyA6wjziN8RRQ9JbCb9P", "1594208918");
        AppsFlyer.startSDK();

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void AppsFlyerEvent(string eventName , string eventValue)
    {
        Dictionary<string, string> eventValues = new Dictionary<string, string>();
        eventValues.Add(eventName, eventValue);
        AppsFlyer.sendEvent(eventName, eventValues);
    }
}
