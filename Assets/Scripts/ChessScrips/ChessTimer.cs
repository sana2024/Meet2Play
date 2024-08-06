using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChessTimer : MonoBehaviour
{
    public float MyTimer = 600;
    [SerializeField] public Text MyTimerText;


    public float OpponentTimer = 600;
    [SerializeField] public Text OpponentTimerText;


  public  bool IsMyTurn = false;
  public bool IsOpponentTurn = false;


   public static ChessTimer Instance;
 

    [SerializeField] float MyTimerDuration = 10.0f; // The duration of the timer in seconds
    [SerializeField] public Image MySlider; // The slider that represents the progress bar
    private float MyelapsedTime = 0.0f; // The elapsed time since the timer started
    private float OpponentelapsedTime = 0.0f;

    [SerializeField] float OppoenentTimerDuration = 10.0f; // The duration of the timer in seconds
    [SerializeField] public Image OpponentSlider; // The slider that represents the progress bar
    [SerializeField] Sprite LooserSprite;
    [SerializeField] Text endGameDialog;
    private DateTime lastTime;

    Scene currentScene;
   public bool isEndScreen = false;


   public bool gameEnded = false;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentScene = SceneManager.GetActiveScene();

        MySlider.fillAmount = 1.0f;
        OpponentSlider.fillAmount = 1.0f;

        var time = PlayerPrefs.GetInt("time");

        lastTime = DateTime.Now;

        if (time == 5)
        {
            MyTimer = 300;
            OpponentTimer = 300;
            MyTimerDuration = 300;
            OppoenentTimerDuration = 300;
        }

        if (time == 10)
        {
            MyTimer = 600;
            OpponentTimer = 600;
            MyTimerDuration = 600;
            OppoenentTimerDuration = 600;
        }

        if (time == 30)
        {
            MyTimer = 1800;
            OpponentTimer = 1800;
            MyTimerDuration = 1800;
            OppoenentTimerDuration = 1800;
        }

        string minutes = Mathf.Floor(MyTimer / 60).ToString("00");
        string seconds = (MyTimer % 60).ToString("00");
        MyTimerText.text = string.Format("{0}:{1}", minutes, seconds);
        OpponentTimerText.text = string.Format("{0}:{1}", minutes, seconds);
    }

    // Update is called once per frame
    void Update()
    {

        if(IsMyTurn && MyTimer > 0 && gameEnded == false)
        {
            MyTimeCounter();
 
        }

        if (IsOpponentTurn && OpponentTimer > 0 && gameEnded == false)
        {
            OpponentTimeCounter();
        }

        if (currentScene.name == "ChessAI")
        {
            if (gameEnded == false)
            {
                if (ChessBoardAI.instance.isWhiteTurn)
                {
                    MyTimeCounter();
                }
                else
                {
                    OpponentTimeCounter();
                }
            }
        }

     }


    public void MyTimeCounter()
    {
        DateTime currentTime = DateTime.Now;
        double elapsedSeconds = (currentTime - lastTime).TotalSeconds;
        lastTime = currentTime;

        if (MyelapsedTime < MyTimerDuration)
        {
            MyelapsedTime += (float)elapsedSeconds; // Update the elapsed time
            float progress = MyelapsedTime / MyTimerDuration; // Calculate the progress as a fraction of the total duration
            MySlider.fillAmount = 1.0f - progress; // Update the progress bar's value (subtract progress from 1.0 to make it decrease)
        }
        else
        {
            // The timer is complete, stop updating the progress bar
            MySlider.fillAmount = 0.0f;
        }

    
        MyTimer -= (float)elapsedSeconds;

        string minutes =Mathf.Abs( Mathf.Floor(MyTimer / 60)).ToString("00");
        string seconds =Mathf.Abs(MyTimer % 60).ToString("00");
        MyTimerText.text = string.Format("{0}:{1}", minutes, seconds);

        if(MySlider.fillAmount == 0)
        {
            if (currentScene.name == "Chess")
            {
                endGameDialog.text = "You ran out of time.";
                MyTimerText.text = "00:00";
                GameEndResult.Instance.LooserResult(LooserSprite);

                var state = MatchDataJson.SetLeaveMatch("leave");
                DataSync.Instance.SendMatchState(OpCodes.Leave_match, state);

            }

            if (currentScene.name == "ChessAI")
            {
                MyTimerText.text = "00:00";
               // OpponentTimerText.text = "00:00";
                MySlider.fillAmount = 1;
                OpponentSlider.fillAmount = 1;
                AIGameResult.Instance.AILooserResult(LooserSprite);
                isEndScreen = true;

            }
        }

        
    }


    public void OpponentTimeCounter()
    {
        DateTime currentTime = DateTime.Now;
        double elapsedSeconds = (currentTime - lastTime).TotalSeconds;
        lastTime = currentTime;

        if (OpponentelapsedTime < OppoenentTimerDuration)
        {
            OpponentelapsedTime += (float)elapsedSeconds; // Update the elapsed time
            float progress = OpponentelapsedTime / OppoenentTimerDuration; // Calculate the progress as a fraction of the total duration
            OpponentSlider.fillAmount = 1.0f - progress; // Update the progress bar's value (subtract progress from 1.0 to make it decrease)
        }
        else
        {
            // The timer is complete, stop updating the progress bar
            OpponentSlider.fillAmount = 0.0f;
        }

        OpponentTimer -= (float)elapsedSeconds;

        string minutes = Mathf.Floor(OpponentTimer / 60).ToString("00");
        string seconds = (OpponentTimer % 60).ToString("00");
        OpponentTimerText.text = string.Format("{0}:{1}", minutes, seconds);


        if(OpponentSlider.fillAmount == 0)
        {
            
            if (currentScene.name == "Chess")
            {
                OpponentTimerText.text = "00:00";
                endGameDialog.text = PassData.OtherPresence.Username + " ran out of time.";
                GameEndResult.Instance.WinnerResult();

            }

            if(currentScene.name == "ChessAI")
            {
               // MyTimerText.text = "00:00";
                OpponentTimerText.text = "00:00";
                MySlider.fillAmount = 1;
                OpponentSlider.fillAmount = 1;
                AIGameResult.Instance.AIWinnerResult();
                isEndScreen = true;
            }
        }
    }

        void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            // App is going to background, save the time
            lastTime = DateTime.Now;
        }
        else
        {
            // App is returning to foreground, update the last time
           // lastTime = DateTime.Now;
        }
    }
    //      void OnApplicationFocus(bool focus)
    //{
    //    Debug.Log("Gained focus "+focus);
    //    if (focus)
    //    {
    //        lastTime = DateTime.Now;
    //    }
    //    else
    //    {
    //        lastTime = DateTime.Now;
    //    }
    //}

}
