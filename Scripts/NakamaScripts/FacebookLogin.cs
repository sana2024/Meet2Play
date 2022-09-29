using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using Facebook.Unity;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class FacebookLogin : MonoBehaviour
{
    private IClient iclient;
    private ISession isession;
    private ISocket isocket;
    [SerializeField] GameObject LoadingPanel;
 

    [SerializeField] NakamaConnection nConnect;
    [SerializeField] GameObject DiceRotate;


    public void Awake()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
        }
        else
        {
            FB.Init(() => FB.ActivateApp());
        }
    }

    public void Start()
    {
        iclient = nConnect.client();
    }

    public void OnfacebookInit()
    {
        if (FB.IsLoggedIn)
        {
            FB.LogOut();
        }

        FB.LogInWithReadPermissions(null, LoginWithFacebook);
    }

    private async void LoginWithFacebook(ILoginResult result)
    {
        LoadingPanel.SetActive(true);
 

        var aToken = Facebook.Unity.AccessToken.CurrentAccessToken.TokenString;
        isession = await iclient.AuthenticateFacebookAsync(aToken);

        var keepAliveIntervalSec = 30;
        isocket = Socket.From(iclient, new WebSocketAdapter(keepAliveIntervalSec));
        await isocket.ConnectAsync(isession, true);
         
        Debug.Log("Login with facebook was successful");

        var resultfb = await iclient.GetAccountAsync(isession);
        var user = resultfb.User;

     
        PassData.isocket = isocket;
        PassData.Username = user.DisplayName;
        PassData.MyURL = user.AvatarUrl;
        PassData.iClient = iclient;
        PassData.isession = isession;

         

   
        await iclient.UpdateAccountAsync(isession, user.DisplayName, user.DisplayName, user.AvatarUrl, null, null);


        if (isocket.IsConnected)
        {
            LoadingPanel.SetActive(false);
 
            ChangeScene();
        }

        if (string.IsNullOrEmpty(result.Error))
        {
            if (result.Cancelled)
            {
                Debug.Log("login with facebook was cancelled");
                // SceneManager.LoadScene("LocalTest");

            }
            else
            {

            }
        }
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene("Menu");
    }

    public void FixedUpdate()
    {
        var speed = 3;
        DiceRotate.transform.Rotate(Vector3.forward * speed);
    }
}
