using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerTimer : MonoBehaviour
{
    [Header("Radial Timers")]
    [SerializeField] private float timerIndicator = 1.0f;
    [SerializeField] private float maxIndicatorTimer = 2.0f;


    [Header("Ui Indicator")]
    [SerializeField] public Image MyTimer = null;
    [SerializeField] public Image OtherPlayerTimer = null;


    [Header("Key Codes")]
    [SerializeField] private KeyCode selecKey = KeyCode.Mouse0;
    [Header("Unity Event")]


    [SerializeField] private UnityEvent myevent = null;
    [SerializeField] public AudioClip audioC;

    [SerializeField] GameManager gameManager;
    [SerializeField] ButtonController buttonController;
    
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
                        MyTimer.color = Color.white;
                    }

                        if (MyTimer.fillAmount <= 0.25f)
                    {
                        MyTimer.color = Color.red;
                        // radialIndicator1.color = Color.red;


                        if (MyTimer.fillAmount == 0)
                        {

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
                        OtherPlayerTimer.color = Color.white;
                    }

                    if (OtherPlayerTimer.fillAmount <= 0.25f)
                    {
                        OtherPlayerTimer.color = Color.red;
                        // radialIndicator1.color = Color.red;


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
        MyTimer.color = Color.green;
        OtherPlayerTimer.fillAmount = 1;
        OtherPlayerTimer.color = Color.green;


    }
}
