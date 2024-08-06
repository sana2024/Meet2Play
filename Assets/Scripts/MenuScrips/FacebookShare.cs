//using System.Diagnostics.SymbolStore;
//using System.Security.Cryptography;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Facebook.Unity;
//using System;

//public class FacebookShare : MonoBehaviour
//{

 
//    [SerializeField] UserProfile user;
//    [SerializeField] ParticleSystem CoinReward;

//    // These ad units are configured to always serve test ads.
//#if UNITY_ANDROID
//   [SerializeField]  private string _adUnitId = "ca-app-pub-3940256099942544/1033173712";
//#elif UNITY_IPHONE
//   [SerializeField]  private string _adUnitId = "ca-app-pub-3940256099942544/4411468910";
//#else
//  private string _adUnitId = "unused";
//#endif


//    private void Start()
//    {
//        if(PlayerPrefs.GetString("sharesplash")== "true")
//        {
//            CoinReward.Play();
//            PlayerPrefs.SetString("sharesplash", "false");
//        }
//    }

//    public void FacebookShareLink(){
//        FB.Mobile.ShareDialogMode = ShareDialogMode.WEB;
//        FB.ShareLink(
//                new System.Uri("https://www.meet2play.app/download"),

//				callback: ShareCallback);
 
//    }


//    private void ShareCallback (IShareResult result) {

//        Debug.Log("ree " + result);
//        if (result.Cancelled || !String.IsNullOrEmpty(result.Error))
//        {
//            Debug.Log("canceled -------------------------");
//        }
//        else if (!String.IsNullOrEmpty(result.PostId))
//        {
//            // Print post identifier of the shared content
//            Debug.Log(result.PostId);
//        }
//        else
//        {
//            Debug.Log("success -------------------------");
//            CoinReward.Play();
//            user.AddXP(5);
//            user.updateWallet(5);
//           // ChessUserDatas.Instance.UpdateXP(5);

            
//        }

//    }

 
 

//}
