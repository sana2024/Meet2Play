using System.Collections;
using System.Collections.Generic;
using System.Text;
using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth.Native;
using UnityEngine;
using Nakama;

public class AppleLogin : MonoBehaviour
{
    private IAppleAuthManager appleAuthManager;
    public static AppleLogin Instance;


    public string AppleUserIdKey { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {

        // If the current platform is supported
        if (AppleAuthManager.IsCurrentPlatformSupported)
        {
            // Creates a default JSON deserializer, to transform JSON Native responses to C# instances
            var deserializer = new PayloadDeserializer();
            // Creates an Apple Authentication manager with the deserializer
            this.appleAuthManager = new AppleAuthManager(deserializer);
        }

    }

    void Update()
    {

        // Updates the AppleAuthManager instance to execute
        // pending callbacks inside Unity's execution loop
        if (this.appleAuthManager != null)
        {
            this.appleAuthManager.Update();
        }




    }

    public void SigninWithAppleID()
    {
        // PlayerPrefs.DeleteAll();
        if (PlayerPrefs.HasKey("AppleUserIdKey"))
        {
            QuickLoginWithApple();
        }
        else
        {
            RegisterWithApple();
        }
    }

    void RegisterWithApple()
    {
        Debug.Log("1");
        NakamaLogin.Instance.LoadingPanel.SetActive(true);
        var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);
        Debug.Log("2");
       
        appleAuthManager.LoginWithAppleId(
            loginArgs,
            async credential =>
            {
                Debug.Log("3");

                // Obtained credential, cast it to IAppleIDCredential
                var appleIdCredential = credential as IAppleIDCredential;
                Debug.Log("4");
                Debug.Log("state " + appleIdCredential.State);

                if (appleIdCredential != null)
                {

                    // Apple User ID
                    var userId = appleIdCredential.User;

                    PlayerPrefs.SetString("AppleUserIdKey", userId);
                    PlayerPrefs.SetString("login", "Apple");

                    if (appleIdCredential.FullName != null)
                    {
                       
                        var fullName = appleIdCredential.FullName.GivenName + " " + appleIdCredential.FullName.FamilyName;
                        PlayerPrefs.SetString("AppleUsername", fullName);
                        NakamaLogin.Instance.signUpApple(userId, fullName);
                    }
                    else
                    {
                        NakamaLogin.Instance.signinApple(userId);
                    }

                    

                    }
            },
            error =>
            {
                NakamaLogin.Instance.LoadingPanel.SetActive(false);
                // Something went wrong
                var authorizationErrorCode = error.GetAuthorizationErrorCode();
            });

        appleAuthManager.SetCredentialsRevokedCallback(result =>
        {
            Debug.Log("canceled");
            NakamaLogin.Instance.LoadingPanel.SetActive(false);
        });


    }

    void QuickLoginWithApple()
    {
        PlayerPrefs.SetString("login", "Apple");
        NakamaLogin.Instance.signinApple(PlayerPrefs.GetString("AppleUserIdKey"));
    }
}
