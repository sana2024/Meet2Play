//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Nakama;
//using Facebook.Unity;
//using UnityEngine.SceneManagement;
//using UnityEngine.Video;
//using UnityEditor;
//using ByteBrewSDK;

//public class FacebookLogin : MonoBehaviour
//{
//    public IClient iclient;
//    private ISession isession;
//    private ISocket isocket;
//    [SerializeField] GameObject LoadingPanel;
 

//    [SerializeField] NakamaConnection nConnect;
//    [SerializeField] GameObject DiceRotate;
//    [SerializeField] GameObject Connectionpanel;

//    public static FacebookLogin Instance;


    

//    public void Awake()
//    {
//         if(Instance == null)
//        {
//            Instance = this;
//        }

//        if (FB.IsInitialized)
//        {
//            FB.ActivateApp();
 
 
            
//        }
//        else
//        {

//            FB.Init(() => FB.ActivateApp());





//        }
//    }

//    [System.Obsolete]
//    public void Start()
//    {
        
//     // iclient = nConnect.client();
//    }

//    public void OnfacebookInit()
//    {
 
//        if (FB.IsLoggedIn)
//        {
 
//            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken.TokenString;
 
//            LoginWithFacebook();


//        }
//        else
//        {

//        FB.LogInWithReadPermissions(null ,LoginWithFacebook);
 
//        }






//    }

//    private async void LoginWithFacebook(ILoginResult result)
//    {
  
//        if (Application.internetReachability == NetworkReachability.NotReachable)
//        {
//            Connectionpanel.SetActive(true);


//        }
//        else
//        {
//            Connectionpanel.SetActive(false);

//            if (result.Cancelled)
//            {
 
//                LoadingPanel.SetActive(false);
//            }
//            else
//            {
//                LoadingPanel.SetActive(true);

//                var aToken = Facebook.Unity.AccessToken.CurrentAccessToken.TokenString;
//                isession = await iclient.AuthenticateFacebookAsync(aToken);
//                isession = await iclient.SessionRefreshAsync(isession);

//                var keepAliveIntervalSec = 30;
//                isocket = Socket.From(iclient, new WebSocketAdapter(keepAliveIntervalSec));
//                await isocket.ConnectAsync(isession, true);
 
 
 
//                var resultfb = await iclient.GetAccountAsync(isession);
//                var user = resultfb.User;

//                PassData.FacebookID = user.FacebookId;
//                PassData.isocket = isocket;
//                PassData.Username = user.DisplayName;
//                PassData.MyURL = user.AvatarUrl;
//                PassData.iClient = iclient;
//                PassData.isession = isession;
//                PassData.DateAndTime = isession.CreateTime.ToString();
 

//                ByteBrew.NewCustomEvent("Login", "type=Facebook; Username=" + isession.Username + ";");

//                await iclient.UpdateAccountAsync(isession, user.DisplayName, user.DisplayName, user.AvatarUrl, null, null);
//                isession = await iclient.SessionRefreshAsync(isession);

//                if (isocket.IsConnected)
//                {
//                    PlayerPrefs.SetString("login", "facebook");
//                    LoadingPanel.SetActive(false);

//                    ChangeScene();
//                }
//            }

//        }
//    }


//    private async void LoginWithFacebook()
//    {
 
//        if (Application.internetReachability == NetworkReachability.NotReachable)
//        {
 
//            Connectionpanel.SetActive(true);


//        }
//        else
//        {
       
//            Connectionpanel.SetActive(false);
      
//            LoadingPanel.SetActive(true);
  
//            FB.Mobile.RefreshCurrentAccessToken();
 
//            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken.TokenString;
 
        
//                isession = await iclient.AuthenticateFacebookAsync(aToken);
//                isession = await iclient.SessionRefreshAsync(isession);
 
//            var keepAliveIntervalSec = 30;
//                isocket = Socket.From(iclient, new WebSocketAdapter(keepAliveIntervalSec));
//                await isocket.ConnectAsync(isession, true);
    
 

//            if (isession.Created)
//            {
//                ByteBrew.NewCustomEvent("Registered", "type=Facebook; Username=" + isession.Username + ";");
//            }
//            else
//            {
//                ByteBrew.NewCustomEvent("LoggedIn", "type=Facebook; Username=" + isession.Username + ";");
//            }


//                var resultfb = await iclient.GetAccountAsync(isession);
//                var user = resultfb.User;
        
//            PassData.FacebookID = user.FacebookId;
//                PassData.isocket = isocket;
//                PassData.Username = user.DisplayName;
//                PassData.MyURL = user.AvatarUrl;
//                PassData.iClient = iclient;
//                PassData.isession = isession;
//                PassData.DateAndTime = isession.CreateTime.ToString();

 


       
//            await iclient.UpdateAccountAsync(isession, user.DisplayName, user.DisplayName, user.AvatarUrl, null, null);
//                isession = await iclient.SessionRefreshAsync(isession);
        
//            if (isocket.IsConnected)
//                {
            
//                PlayerPrefs.SetString("login", "facebook");
//                    LoadingPanel.SetActive(false);

//                    ChangeScene();
//                }
            

//        }
//    }

//    private void ChangeScene()
//    {
//        SceneManager.LoadScene("Menu");
//    }

//    public void FixedUpdate()
//    {
//        var speed = 3;
//        DiceRotate.transform.Rotate(Vector3.forward * speed);
//    }

    
//}
