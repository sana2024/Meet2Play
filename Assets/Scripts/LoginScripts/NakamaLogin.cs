using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ByteBrewSDK;
using UnityEngine.Networking;
#if UNITY_ANDROID
using Unity.Notifications.Android;
using UnityEngine.Android;
#endif
#if UNITY_IOS
using UnityEngine.SocialPlatforms.GameCenter;
#endif

public class NakamaLogin : MonoBehaviour
{
    //--------------
    // Nakama Connections and sessions
    //--------------

    [SerializeField] NakamaConnection Nconnect;
    public IClient iclient;
    public ISession isession;
    public ISocket isocket;
    WebCamDevice[] devices;
    string AvatarUrl;


    //-------------
    // UI
    //-------------

    [SerializeField] Button PlatformBtn;
    [SerializeField] Sprite GoogleIcon;
    [SerializeField] Sprite AppleIcon;
    [SerializeField] Sprite EditorIcon;
    [SerializeField] public GameObject LoadingPanel;
 
    //[SerializeField] GameObject ConnectionPanel;
    [SerializeField] GameObject DiceRotate;
    [SerializeField] GSPManager gspManager;

    [SerializeField] public GameObject LuncherPanel;
    [SerializeField] Image LauncherSlider;

    public static NakamaLogin Instance;

    bool facebookcheck = false;

    [System.Obsolete]
    private void Start()
    {
#if UNITY_ANDROID

        //if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        //    {
        //        Debug.Log("Microphone permission has been granted.");

        //    }

#endif

        devices = WebCamTexture.devices;
        ByteBrew.NewCustomEvent("OpenedApp");
   

        // Initialize ByteBrew
        ByteBrew.InitializeByteBrew();
        ByteBrew.GetUserID();


        var IsTimerIntialized = PlayerPrefs.HasKey("time");

        if (IsTimerIntialized == false)
        {
 
            PlayerPrefs.SetInt("time", 10);
        }


       // iclient = Nconnect.client();
 
 
        if (Instance == null)
        {
            Instance = this;
        }

#if UNITY_ANDROID
        

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        PlatformBtn.image.sprite = GoogleIcon;
        
        PlatformBtn.onClick.AddListener(()=> { gspManager.GoogleSigin(); });


#endif

#if UNITY_IOS

        PlatformBtn.image.sprite = AppleIcon;
        PlatformBtn.onClick.AddListener(AppleLogin.Instance.SigninWithAppleID);


#endif

#if UNITY_EDITOR

        PlatformBtn.image.sprite = EditorIcon;

#endif


    }

 

    public void AutomaticLogins()
    {
 

        if (PlayerPrefs.GetString("login") == "guest")
        {
            OnGuestLogin();

        }
        else if (PlayerPrefs.GetString("login") == "Apple")
        {
#if UNITY_IOS

            AppleLogin.Instance.SigninWithAppleID();


#endif
        }
        else if (PlayerPrefs.GetString("login") == "googlePlay")
        {
#if UNITY_ANDROID


            GSPManager.Instance.GoogleSigin();

#endif
        }


        //if (PlayerPrefs.GetString("login") == "facebook")

        //{

        //    FacebookLogin.Instance.OnfacebookInit();

        //}

    }

    [System.Obsolete]
    public async void OnGuestLogin()
    {

  
            string displayName = "";
            string username = "";
            string avatarUrl = "";

            var vars = new Dictionary<string, string>();
            vars["key"] = "value";
            vars["key2"] = "value2";

 
            LoadingPanel.SetActive(true);
 
            isession = await iclient.AuthenticateDeviceAsync(SystemInfo.deviceUniqueIdentifier, create: true);
 

            var keepAliveIntervalSec = 30;
            isocket = Socket.From(iclient, new WebSocketAdapter(30));
            await isocket.ConnectAsync(isession, true , keepAliveIntervalSec);

        Debug.Log(isession.CreateTime);
   
        if (isession.Created)
            {
                
  
                displayName = "Player" + Random.Range(8000, 500000000);
                username = displayName;
                avatarUrl = "https://i.pinimg.com/564x/70/5e/fb/705efbcfb56e45f52636d4e9f441a369.jpg";
               
                try
                {
                    var retryConfiguration = new Nakama.RetryConfiguration( 1,  5, delegate { System.Console.WriteLine("about to retry."); });

                    iclient.GlobalRetryConfiguration = retryConfiguration;

                    await iclient.UpdateAccountAsync(isession, username, displayName, avatarUrl, null, null, null , retryConfiguration);
                    isession = await iclient.SessionRefreshAsync(isession);
 

            }
                catch (Nakama.ApiResponseException ex)
                {
 
                    if (ex.Message == "Username is already in use.")
                    {
                        Debug.Log("username already in use");
                        displayName = "Player" + Random.Range(8000, 500000);
                        username = displayName;
                        
                        await iclient.UpdateAccountAsync(isession, username, displayName, avatarUrl, null, null);
                        isession = await iclient.SessionRefreshAsync(isession);
                    }
                }
 
                PassData.isocket = isocket;
                PassData.Username = isession.Username;
                PassData.DateAndTime = " " +System.DateTime.Now.Year.ToString() + "-" + System.DateTime.Now.Month.ToString() + "-" + System.DateTime.Now.Day.ToString();
                PassData.MyURL = avatarUrl;
                PassData.iClient = iclient;
                PassData.isession = isession;
                PassData.ImageURL = avatarUrl;

            ByteBrew.NewCustomEvent("Registered", "type=Guest; Username=" + isession.Username + ";");
            AppFlyer.Instance.AppsFlyerEvent("Registered" , isession.Username);



            }
            else
            {
                var account = await iclient.GetAccountAsync(isession);
                Debug.Log(account.User.Id);
                var user = account.User;
                displayName = user.DisplayName;
                username = displayName;

                PassData.DateAndTime = " " + user.CreateTime.ToString().Substring(0, 10);
                PassData.isocket = isocket;
                PassData.Username = username;
                PassData.iClient = iclient;
                PassData.isession = isession;
                PassData.MyURL = user.AvatarUrl;

 

            ByteBrew.NewCustomEvent("LoggedIn", "type=Guest; Username=" + isession.Username + ";");
            AppFlyer.Instance.AppsFlyerEvent("login", isession.Username);

        }

  
            PlayerPrefs.SetString("login", "guest");
            ChangeScene();
            LoadingPanel.SetActive(false);
 

        
    }



#if UNITY_IOS
    public void GamecenterLogin()
    {

            LoadingPanel.SetActive(true);
            GameCenterPlatform.ShowDefaultAchievementCompletionBanner(true);
            Social.localUser.Authenticate(success =>
            {
                if (success)
                {
                    PlayerPrefs.SetString("login", "gameCenter");
                    var avatarUrl = "https://upload.wikimedia.org/wikipedia/de/8/83/Game_Center_Logo_iOS_7.png";
                    EmailLogin(Social.localUser.userName + "@gmail.com", Social.localUser.id, Social.localUser.userName, avatarUrl);
                }


                else
                {
                    Debug.Log("Failed to authenticate");
                }
                    
            });

        
    }

#endif

    public async void signinApple(string token)
    {
        try
            {
                isession = await iclient.AuthenticateCustomAsync(token);


                var keepAliveIntervalSec = 30;
                isocket = Socket.From(iclient, new WebSocketAdapter(30));
                await isocket.ConnectAsync(isession, true, keepAliveIntervalSec);
                var account = await iclient.GetAccountAsync(isession);

                if (isession.Created)
                {

                var avatarUrl = "https://i.ibb.co/yQTGZvL/apple-logo-icon-149061.png";
                await iclient.UpdateAccountAsync(isession, account.User.Username, account.User.Username, avatarUrl, null, null, null);
                isession = await iclient.SessionRefreshAsync(isession);
                PassData.DateAndTime = " " + System.DateTime.Now.Year.ToString() + "-" + System.DateTime.Now.Month.ToString() + "-" + System.DateTime.Now.Day.ToString();
                AppFlyer.Instance.AppsFlyerEvent("login", isession.Username);
            }
            else
            {
                PassData.DateAndTime = isession.CreateTime.ToString();
            }


                
                    PassData.isocket = isocket;
                    PassData.Username = isession.Username;
                    
                    PassData.MyURL = account.User.AvatarUrl;
                    PassData.iClient = iclient;
                    PassData.isession = isession;
                    PassData.ImageURL = account.User.AvatarUrl;

                    ByteBrew.NewCustomEvent("LoggedIn", "type=Guest; Username=" + isession.Username + ";");
                

                ChangeScene();
                LoadingPanel.SetActive(false);

            }
            catch (Nakama.ApiResponseException ex)
            {
                Debug.Log("exeption --- " + ex);
            }

        
    }

    public async void signUpApple(string token, string fullName)
    {

            string displayName = "";
            string username = "";
            string avatarUrl = "";
            try
            {
                isession = await iclient.AuthenticateCustomAsync(token);
                Debug.Log(isession);

                var keepAliveIntervalSec = 30;
                isocket = Socket.From(iclient, new WebSocketAdapter(30));
                await isocket.ConnectAsync(isession, true, keepAliveIntervalSec);

                if (isession.Created)
                {
                   AppFlyer.Instance.AppsFlyerEvent("Registered", isession.Username);
                     displayName = fullName;
                    username = fullName;
                    avatarUrl = "https://i.ibb.co/yQTGZvL/apple-logo-icon-149061.png";

                    try
                    {
                        var retryConfiguration = new Nakama.RetryConfiguration(1, 5, delegate { System.Console.WriteLine("about to retry."); });

                        iclient.GlobalRetryConfiguration = retryConfiguration;

                        await iclient.UpdateAccountAsync(isession, username, displayName, avatarUrl, null, null, null, retryConfiguration);
                        isession = await iclient.SessionRefreshAsync(isession);


                    }
                    catch (Nakama.ApiResponseException ex)
                    {

                        if (ex.Message == "Username is already in use.")
                        {
                            Debug.Log("username already in use");

                        }
                    }

                    PassData.isocket = isocket;
                    PassData.Username = isession.Username;
                    PassData.DateAndTime = " " + System.DateTime.Now.Year.ToString() + "-" + System.DateTime.Now.Month.ToString() + "-" + System.DateTime.Now.Day.ToString();
                    PassData.MyURL = avatarUrl;
                    PassData.iClient = iclient;
                    PassData.isession = isession;
                    PassData.ImageURL = avatarUrl;

                    ByteBrew.NewCustomEvent("Registered", "type=Guest; Username=" + isession.Username + ";");
                    ChangeScene();
                    LoadingPanel.SetActive(false);

                }
                else
                {
                    signinApple(token);
                }


            }
            catch (Nakama.ApiResponseException ex)
            {
                Debug.Log("exeption --- " + ex);
            }

        

        }


    public async void EmailLogin(string email, string password , string name , string avatarUrl)
        {
        
        isession = await iclient.AuthenticateEmailAsync(email, password);
        if (isession.Created)
        {
            AppFlyer.Instance.AppsFlyerEvent("Registered", isession.Username);
            ByteBrew.NewCustomEvent("Registered", "type=SocialPlatform; Username=" + isession.Username + ";");
            PassData.DateAndTime = " " + System.DateTime.Now.Year.ToString() + "-" + System.DateTime.Now.Month.ToString() + "-" + System.DateTime.Now.Day.ToString();



            await iclient.UpdateAccountAsync(isession, name, name, avatarUrl, null, null);

        }
        else
        {
            ByteBrew.NewCustomEvent("LoggedIn", "type=SocialPlatform; Username=" + name + ";");
            AppFlyer.Instance.AppsFlyerEvent("login", isession.Username);
            PassData.DateAndTime = isession.CreateTime.ToString();
        }

 
            isession = await iclient.SessionRefreshAsync(isession);
 

            var keepAliveIntervalSec = 30;
            isocket = Socket.From(iclient, new WebSocketAdapter(keepAliveIntervalSec));
            await isocket.ConnectAsync(isession, true, keepAliveIntervalSec);

            var account = await iclient.GetAccountAsync(isession);
        
             PassData.isocket = isocket;
            PassData.Username = isession.Username;
            PassData.MyURL = account.User.AvatarUrl;
            PassData.iClient = iclient;
            PassData.isession = isession;
             
 
            //ByteBrew.NewCustomEvent("Login", "type=GameCenter_GooglePlay; Username=" + isession.Username + ";");
            ChangeScene();
            LoadingPanel.SetActive(false);

    }

  
    private void ChangeScene()
    {
        SceneManager.LoadScene("Menu");
    }


    public void Update()
    {


//#if UNITY_ANDROID
//        PermissionHelper.RequestMicrophontPermission();
//            PermissionHelper.RequestCameraPermission();
//        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
//        {
//            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
//        }
//#endif




        var speed = 1;

 
 
        

    }

    public void FixedUpdate()
    {
        var speed = 3;
        DiceRotate.transform.Rotate(Vector3.forward * speed);
    }

    [System.Obsolete]
    IEnumerator GetTexture()
    {
        Debug.Log(PassData.MyURL);
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(PassData.MyURL);
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {

        }
        else
        {
            Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            PassData.MyURLTexture = CropTexture((Texture2D)myTexture);

        }

    }

    Texture2D CropTexture(Texture2D originalTexture)
    {
        int size = Mathf.Min(originalTexture.width, originalTexture.height);
        int offsetX = (originalTexture.width - size) / 2;
        int offsetY = (originalTexture.height - size) / 2;

        Color[] pixels = originalTexture.GetPixels(offsetX, offsetY, size, size);

        Texture2D squareTexture = new Texture2D(size, size);
        squareTexture.SetPixels(pixels);
        squareTexture.Apply();

        return squareTexture;
    }





}
