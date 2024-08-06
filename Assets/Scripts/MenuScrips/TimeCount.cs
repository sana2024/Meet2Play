using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeCount : MonoBehaviour
{

    [SerializeField] 
    public GameObject Opponent_challenge;
    [SerializeField] 
    public Text TimerCountDown;
     public float timeRemaining = 180;

    void Update()

    {

        if (timeRemaining > 0)

        {

            if (Opponent_challenge.activeSelf)
            {

                timeRemaining -= Time.deltaTime;
                float minutes = Mathf.FloorToInt(timeRemaining / 60);
                float seconds = Mathf.FloorToInt(timeRemaining % 60);
                TimerCountDown.text = minutes.ToString() + ":" + seconds.ToString();


                if (minutes == 0 && seconds == 0)
                {

                    Opponent_challenge.SetActive(false);


                }


                if (minutes == 0 && seconds == 0)
                {

                    timeRemaining = 180;
                }




            }

            else

            {
 
                    timeRemaining = 180;
                

            }

        }
 
    }
 
    

}

