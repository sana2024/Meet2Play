using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using ByteBrewSDK;

public class backTimerAI : MonoBehaviour
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
 

    Color Red = new Color(1, 0.2f, 0, 1);
    Color Orange = new Color(1, 0.6f, 0, 1);
    Color Yellow = new Color(1, 0.8f, 0, 1);

    private AudioSource audioS;

    private bool shouldUpdate = false;

    bool playerLost = false;

    public static backTimerAI Instance;

    private void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
    private void FixedUpdate()
    {
       // Debug.Log("player " + BackgammonAI.Instance.player);
 
        if (BackgammonAI.Instance.player == 0)
        {
            playerTimer();
        }
        else
        {
            OponentTimer();
        }
    }


    public async void playerTimer()
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
                      //  MyTimerMask.color = Red;
                        // radialIndicator1.color = Color.red;


                        if (MyTimer.fillAmount == 0)
                        {
                        // ByteBrew.NewCustomEvent("Lost", "Game=Backgammon; Type= ; Username=" + PassData.isession.Username + ";");
                        BackAIResult.Instance.AILooserResult();

                        }
                    }


                }
            }

    }
    public async void OponentTimer()
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
                        //OtherPlayerTimerMask.color = Red;
                        // radialIndicator1.color = Color.red;

                    }

                    if (OtherPlayerTimer.fillAmount <= 0)
                    {

                    BackAIResult.Instance.AIWinnerResult();
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
      //  MyTimerMask.color = Yellow;
        OtherPlayerTimer.fillAmount = 1;
      //  OtherPlayerTimerMask.color = Yellow;


    }



}

