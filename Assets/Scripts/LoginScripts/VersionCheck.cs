using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Nakama;
using Unity.Services.RemoteConfig;

public class VersionCheck : MonoBehaviour
{

    public string MyVersion;

    public string MyIOSVersion;
    public string MyAndroidVersion;

    private string IOSVersion;
    private string AndroidVersion;

    public struct userAttributes { }
    public struct appAttributes { }
    private string Version;
    private bool facebookButton;
    private bool googleButton;
    private bool appleButton;
    private bool guestButton;
    private string maintenance;

    float bonusTimer;

    int ios;
    int android;

    [SerializeField] GameObject VersionCheckPanel;
    [SerializeField] Text ThisVersion;
    [SerializeField] Text LatestVersion;
    [SerializeField] Sprite AppStoreSprite;
    [SerializeField] Sprite GoggleSprite;
    [SerializeField] Button RedirectButton;
    [SerializeField] GameObject NoInternetPanel;
    [SerializeField] NakamaConnection Nconnect;
    [SerializeField] GameObject maintenancePanel;

    [SerializeField] GameObject FacebookButton;
    [SerializeField] GameObject SocialButton;
    [SerializeField] GameObject GuestButton;


    public static VersionCheck Instance;


    public bool IsUpToDate = false;


    string AppStoreURL = "https://apps.apple.com/us/app/meet-2-play/id1594208918";
    string GooglePlayStoreURL = "https://play.google.com/store/apps/details?id=com.meet2play.v1.com";

    [System.Obsolete]
    private void Awake()
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
    private void Start()
    {


#if UNITY_IOS

        RedirectButton.image.sprite = AppStoreSprite;

#endif

#if UNITY_ANDROID
        RedirectButton.image.sprite = GoggleSprite;

#endif

    }

    [System.Obsolete]
    void SetValues(ConfigResponse response)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            NoInternetPanel.SetActive(true);
        }
        else
        {
            
            Version = RemoteConfigService.Instance.appConfig.GetString("Version");
            IOSVersion = RemoteConfigService.Instance.appConfig.GetString("IOSVersion");
            AndroidVersion = RemoteConfigService.Instance.appConfig.GetString("AndroidVersion");

            if(IOSVersion != "")
            {
            ios = int.Parse(IOSVersion);
            }

            if(AndroidVersion != "")
            {
            android = int.Parse(AndroidVersion);
            }

            bonusTimer = RemoteConfigService.Instance.appConfig.GetFloat("BonusTimer");
            PassData.BonusTimer = bonusTimer;

            if(RemoteConfigService.Instance.appConfig.GetString("Host") != "")
            {
            // we will create the user client here so we can easly change the IP of the host using remote config
            NakamaLogin.Instance.iclient = new Client("http", RemoteConfigService.Instance.appConfig.GetString("Host"), 7350, "defaultkey", UnityWebRequestAdapter.Instance);
            //FacebookLogin.Instance.iclient = new Client("http", RemoteConfigService.Instance.appConfig.GetString("Host"), 7350, "defaultkey", UnityWebRequestAdapter.Instance);
            }
            

            facebookButton = RemoteConfigService.Instance.appConfig.GetBool("facebookButton");
            googleButton = RemoteConfigService.Instance.appConfig.GetBool("GooglePlayButton");
            appleButton = RemoteConfigService.Instance.appConfig.GetBool("AppleButton");
            guestButton = RemoteConfigService.Instance.appConfig.GetBool("GuestButton");
            maintenance = RemoteConfigService.Instance.appConfig.GetString("maintenance");

          //  Debug.Log("maintenance " + RemoteConfigService.Instance.appConfig.GetBool("maintenance"));



            PassData.DiscordUrl = RemoteConfigService.Instance.appConfig.GetString("discord");

            if (FacebookButton == null)
            {
                FacebookButton = GameObject.FindGameObjectWithTag("Facebook_Button");
            }

            if (GuestButton == null)
            {
                GuestButton = GameObject.FindGameObjectWithTag("Guest_Button");
            }

            if( SocialButton == null)
            {
                SocialButton = GameObject.FindGameObjectWithTag("Social_Button");
            }


            if (facebookButton == true)
            {
                FacebookButton.active = true;
            }
            else
            {
                FacebookButton.active = false;
            }

            

            if (GuestButton != null)
            {



                if (guestButton == true)
                {
                    GuestButton.active = true;
                }
                else
                {
                    GuestButton.active = false;
                }

            }
#if UNITY_IOS

            if (appleButton== true)
            {
                SocialButton.active = true;
            }
            else
            {
                SocialButton.active = false;
            }


            ThisVersion.text = MyIOSVersion;
            LatestVersion.text = IOSVersion;


            if(IOSVersion != "")
            {
            if (int.Parse(MyIOSVersion) >= int.Parse(IOSVersion))
            {
                    if(maintenance != "true")
                    {
                    IsUpToDate = true;
                    VersionCheckPanel.SetActive(false);
 
                    NakamaLogin.Instance.AutomaticLogins();
                    }
                    else
                    {
                        maintenancePanel.SetActive(true);
                    }


            }
            else
            {
                IsUpToDate = false;
                VersionCheckPanel.SetActive(true);
 
            }
            }

#endif


#if UNITY_ANDROID



            if (googleButton == true)
            {
                SocialButton.active = true;
            }
            else
            {
                SocialButton.active = false;
            }

            ThisVersion.text = MyAndroidVersion;
            LatestVersion.text = AndroidVersion;

            if(AndroidVersion != "")
            {
        if (int.Parse(MyAndroidVersion) >= int.Parse(AndroidVersion))
        {
                            if(maintenance != "true")
                    {
                        VersionCheckPanel.SetActive(false);
                        NakamaLogin.Instance.AutomaticLogins();

                    }
                    else
                    {
                        maintenancePanel.SetActive(true);
                    }
        }
        else
        {
            IsUpToDate = false;
            VersionCheckPanel.SetActive(true);
        }
            }


#endif
        }

    }


    public void UpdateClicked()
    {
#if UNITY_IOS

        Application.OpenURL(AppStoreURL);

#endif


#if UNITY_ANDROID

      Application.OpenURL(GooglePlayStoreURL);

#endif
    }

    public void CloseTheApp()
    {
        Application.Quit();
    }
}
