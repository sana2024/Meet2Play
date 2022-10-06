using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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


    //-------------
    // UI
    //-------------

    [SerializeField] Button PlatformBtn;
    [SerializeField] Sprite GoogleIcon;
    [SerializeField] Sprite AppleIcon;
    [SerializeField] Sprite EditorIcon;
    [SerializeField] public GameObject LoadingPanel;
 
    [SerializeField] GameObject ConnectionPanel;
    [SerializeField] GameObject DiceRotate;
    [SerializeField] GSPManager gspManager;

    private void Start()
    {


        iclient = Nconnect.client();

#if UNITY_ANDROID
        

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        PlatformBtn.image.sprite = GoogleIcon;
        
        PlatformBtn.onClick.AddListener(()=> { gspManager.GoogleSigin(); });


#endif

#if UNITY_IOS

        PlatformBtn.image.sprite = AppleIcon;
        PlatformBtn.onClick.AddListener(GamecenterLogin);


#endif

#if UNITY_EDITOR

        PlatformBtn.image.sprite = EditorIcon;

#endif


    }

 
    public async void OnGuestLogin()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            ConnectionPanel.SetActive(true);


            }
        else
        {

            ConnectionPanel.SetActive(false);
            string displayName = "";
            string username = "";
            string avatarUrl = "";

            var vars = new Dictionary<string, string>();
            vars["key"] = "value";
            vars["key2"] = "value2";

 
            LoadingPanel.SetActive(true);
 
            isession = await iclient.AuthenticateDeviceAsync(SystemInfo.deviceUniqueIdentifier, create: true);
            isession = await iclient.SessionRefreshAsync(isession);

            var keepAliveIntervalSec = 10;
            isocket = Socket.From(iclient, new WebSocketAdapter(keepAliveIntervalSec));
            await isocket.ConnectAsync(isession, true , keepAliveIntervalSec);



            if (isession.Created)
            {
                displayName = "Player" + Random.RandomRange(0, 5000);
                username = displayName;
                avatarUrl = "https://i.pinimg.com/564x/bc/7f/80/bc7f8058b40eaf9118e762830db84e3e.jpg";
                await iclient.UpdateAccountAsync(isession, username, displayName, avatarUrl, null, null);

                PassData.isocket = isocket;
                PassData.Username = username;
                PassData.MyURL = avatarUrl;
                PassData.iClient = iclient;
                PassData.isession = isession;
                PassData.ImageURL = avatarUrl;
            }
            else
            {
                var account = await iclient.GetAccountAsync(isession);
                var user = account.User;
                displayName = user.DisplayName;
                username = displayName;
                avatarUrl = "https://i.pinimg.com/564x/bc/7f/80/bc7f8058b40eaf9118e762830db84e3e.jpg";
                await iclient.UpdateAccountAsync(isession, username, displayName, avatarUrl, null, null);

                PassData.isocket = isocket;
                PassData.Username = username;
                PassData.MyURL = avatarUrl;
                PassData.iClient = iclient;
                PassData.isession = isession;
            }

            ChangeScene();
            LoadingPanel.SetActive(false);
 

        }
    }

 

#if UNITY_IOS
        public void GamecenterLogin()
        {

        LoadingPanel.SetActive(true);
        GameCenterPlatform.ShowDefaultAchievementCompletionBanner(true);
            Social.localUser.Authenticate(success => {
                if (success)
                {
                    var avatarUrl = "https://www.nicepng.com/png/detail/232-2323319_gamecenter-icon-game-center-app.png";
                    EmailLogin(Social.localUser.userName + "@gmail.com", Social.localUser.id , Social.localUser.userName , avatarUrl);
                }


                else
                    Debug.Log("Failed to authenticate");
            });



        }

#endif

    public async void EmailLogin(string email, string password , string name , string avatarUrl)
        {
  
        isession = await iclient.AuthenticateEmailAsync(email, password);

       
            Debug.Log("session created");
            isession = await iclient.SessionRefreshAsync(isession);

            var keepAliveIntervalSec = 10;
            isocket = Socket.From(iclient, new WebSocketAdapter(keepAliveIntervalSec));
            await isocket.ConnectAsync(isession, true, keepAliveIntervalSec);

 
           var displayName = name;
           var username = displayName;
            await iclient.UpdateAccountAsync(isession, username, displayName, avatarUrl, null, null);

            PassData.isocket = isocket;
            PassData.Username = username;
            PassData.MyURL = avatarUrl;
            PassData.iClient = iclient;
            PassData.isession = isession;
       

            ChangeScene();
            LoadingPanel.SetActive(false);


    }

 


    private void ChangeScene()
    {
        SceneManager.LoadScene("Menu");
    }


    public void Update()
    {


        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            ConnectionPanel.SetActive(false);
        }


        var speed = 3;

 
           DiceRotate.transform.Rotate(Vector3.forward * speed);
        

    }

    private void FixedUpdate()
    {
        var speed = 3;
        DiceRotate.transform.Rotate(Vector3.forward * speed);
    }





}
