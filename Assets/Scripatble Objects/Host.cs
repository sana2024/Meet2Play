using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Configuration;
using Unity.RemoteConfig;
using Nakama;
using Unity.Services.RemoteConfig;

public class Host : MonoBehaviour
{
    public struct userAttributes { }
    public struct appAttributes { }
    public static Host Instance;
    string Version;
    string IOSVersion;
    string AndroidVersion;
    string host;

    // Start is called before the first frame update

    [System.Obsolete]
    void Start()
    {
         if(Instance == null)
        {
            Instance = this;
        }

        GetVersion();
    }

    [System.Obsolete]
    public void GetVersion()
    {

        RemoteConfigService.Instance.FetchCompleted += SetValues;
        RemoteConfigService.Instance.FetchConfigs<userAttributes, appAttributes>
                 (new userAttributes(), new appAttributes());

    }

    [System.Obsolete]
    void SetValues(ConfigResponse response)
    {


        Version = RemoteConfigService.Instance.appConfig.GetString("Version");
            IOSVersion = RemoteConfigService.Instance.appConfig.GetString("IOSVersion");
        AndroidVersion = RemoteConfigService.Instance.appConfig.GetString("AndroidVersion");
        host = RemoteConfigService.Instance.appConfig.GetString("Host");



    }
}
