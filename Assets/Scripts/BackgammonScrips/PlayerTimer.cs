using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using ByteBrewSDK;

public class PlayerTimer : MonoBehaviour
{
    [Header("Radial Timers")]
    [SerializeField] private float timerIndicator = 1.0f;
    [SerializeField] private float maxIndicatorTimer = 2.0f;


    [Header("Ui Indicator")]
    [SerializeField] public Image MyTimer = null;
    [SerializeField] public Image OtherPlayerTimer = null;

    [SerializeField] Image MyTimerMask = null;
    [SerializeField] Image OtherPlayerTimerMask = null;


    [Header("Key Codes")]
    [SerializeField] private KeyCode selecKey = KeyCode.Mouse0;
    [Header("Unity Event")]


    [SerializeField] private UnityEvent myevent = null;
    [SerializeField] public AudioClip audioC;

    [SerializeField] GameManager gameManager;
    [SerializeField] ButtonController buttonController;

    Color Red = new Color(1, 0.2f, 0, 1);
    Color Orange = new Color(1, 0.6f, 0,1);
    Color Yellow = new Color(1, 0.8f, 0 ,1);

    private AudioSource audioS;

    private bool shouldUpdate = false;

    bool playerLost = false;

 

    public async void playerTimer()
    {
        
        if (PassData.JoinedPlayers == 2)
        {
            if (playerLost == false)
            {
                if (MyTimer.fillAmount != 0)
                {
                    MyTimer.fillAmount -= 0.0002f;
                    //  radialIndicator1.fillAmount -= 0.0002f;


                    if (MyTimer.fillAmount <= 0.50f)
                    {
                      //  MyTimerMask.color = Orange;
                    }

                        if (MyTimer.fillAmount <= 0.25f)
                    {
                       // MyTimerMask.color =  Red;
                        // radialIndicator1.color = Color.red;


                        if (MyTimer.fillAmount == 0)
                        {
                            ByteBrew.NewCustomEvent("Lost", "Game=Backgammon; Type= ; Username=" +PassData.isession.Username + ";");
                            AppFlyer.Instance.AppsFlyerEvent("Lost_Backgammon_Online", PassData.isession.Username);
                            buttonController.GameOver();

                        }
                    }


                }
            }

        }

       

    }

    public async void OponentTimer()
    {
        
        if (PassData.JoinedPlayers == 2)
        {
            if (playerLost == false)
            {
                if (OtherPlayerTimer.fillAmount != 0)
                {
                    OtherPlayerTimer.fillAmount -= 0.0002f;
                    //  radialIndicator1.fillAmount -= 0.0002f;

                    if (OtherPlayerTimer.fillAmount <= 0.50f)
                    {
                       // OtherPlayerTimerMask.color = Orange;
                    }

                    if (OtherPlayerTimer.fillAmount <= 0.25f)
                    {
                       // OtherPlayerTimerMask.color = Red;
                        // radialIndicator1.color = Color.red;
 
                    }

                    if(OtherPlayerTimer.fillAmount <= 0)
                    {
                        if (GameManager.instance.RecievedEndGame == false)
                        {
                            ByteBrew.NewCustomEvent("Won", "Game=Backgammon; Type= ; Username=" + PassData.isession.Username + ";");
                            AppFlyer.Instance.AppsFlyerEvent("Won_Backgammon_Online", PassData.isession.Username);
                            GameManager.instance.gameEndScreen.SetActive(true);
                            GameManager.instance.WinnerImage.SetActive(true);
                            GameManager.instance.EndGameBackground.sprite = gameManager.GreenBackground;
                            GameManager.instance.playerWonRound = GameManager.instance.MyPlayer;
                            GameManager.instance.Reward.SetActive(true);
                            InGameData.Instance.updateWallet(PassData.betAmount , 25);
                            PassData.wins++;
                            InGameData.Instance.WriteWinsAndLosses(PassData.level, PassData.wins, PassData.losses);
                            InGameData.Instance.UpdateXP(25);
                            GameEnded();
                            HelloVideoAgora.instance.OnApplicationQuit();

                        }
                    }

                   


                }
            }
        }

       

    }

    public void GameEnded()
    {
        MyTimer.fillAmount = 1;
        OtherPlayerTimer.fillAmount = 1;

        playerLost = true;
    }

 

    public void restart()
    {
        MyTimer.fillAmount = 1;
       // MyTimerMask.color = Yellow;
        OtherPlayerTimer.fillAmount = 1;
      //  OtherPlayerTimerMask.color = Yellow;


    }
}
