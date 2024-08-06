using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using Nakama.TinyJson;
using System.Threading.Tasks;
using ByteBrewSDK;
using UnityEngine.UI;
using System;

public class Connections : MonoBehaviour
{
    [SerializeField] GameObject NoConnectionPanel;
    [SerializeField] GameObject WeakConnection;
    [SerializeField] public GameObject OppoenentOffile;
    [SerializeField] GameObject UserOffline;
    [SerializeField] public Sprite LooserSprite;
    [SerializeField] Text endGameDialog;
    [SerializeField] public Text opponentOfflineDialog;
   public float leaveInvokeTimer = 15f;
   public bool leaveInvokeStarted = false;
    bool Reconnected = false;
    public static Connections Instance;
    private DateTime timeWhenPaused;
    private bool wasPaused;

    private float backgroundStartTime;
    private float currentBackgroundTime;

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
        var mainThread = UnityMainThreadDispatcher.Instance();
        PassData.isocket.Closed += () => Connect();
        PassData.isocket.Connected += () => Connect();
        PassData.isocket.ReceivedMatchPresence += m => mainThread.Enqueue(() => CheckPresence(m));

        InvokeRepeating("CheckOpponentAailabilty", 0.1f, 1);

    }
    //this method created to call the win invoke from another script
    public void WinInvoke()
    {
        Invoke("WinGame", 15f);
    }

    public void CancelAllInvokes()
    {
        CancelInvoke();
    }

    async void CheckOpponentAailabilty()
    {
        if (DataSync.Instance.AppInBackground == false)
        {
            var ids = new[] { PassData.OtherUserId };
            var result = await PassData.iClient.GetUsersAsync(PassData.isession, ids);

            foreach (var user in result.Users)
            {

                if (user.Online)
                {
                    OppoenentOffile.SetActive(false);
                    // timeToEndGame = 0;
                }

                if (user.Online == false)
                {
                    OppoenentOffile.SetActive(true);
                }
            }

        }

    }


    public async void CheckPresence(IMatchPresenceEvent matchPresenceEvent)
    {
        if (DataSync.Instance.AppInBackground == false)
        {


            foreach (var user in matchPresenceEvent.Leaves)
            {
                if (user.UserId != PassData.Match.Self.UserId)
                {
                    Debug.Log(user.Username + " left the game ");

                    if (ChessBoard.Instance.victoryScreen.activeSelf == false)
                    {
                        OppoenentOffile.SetActive(true);
                        ByteBrew.NewCustomEvent("GameInterrupted", "Username=" + PassData.isession.Username + ";");
                        leaveInvokeStarted = true;
                        Invoke("WinGame", 15f);

                    }
                }

            }


            foreach (var user in matchPresenceEvent.Joins)
            {
                Debug.Log(user.Username + " joined the game ");
                OppoenentOffile.SetActive(false);
                leaveInvokeTimer = 15f;
                leaveInvokeStarted = false;
                CancelInvoke();


            }
        }

    }

    public void WinGame()
    {
        if (!GameEndResult.Instance.EndScreenPanel.activeSelf)
        {
          endGameDialog.text = PassData.OtherPresence.Username + " abandoned the game.";

          GameEndResult.Instance.WinnerResult();
        }

    }

    public void LooseGame()
    {
        GameEndResult.Instance.LooserResult(LooserSprite);
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            NoConnectionPanel.SetActive(true);
            ByteBrew.NewCustomEvent("GameInterrupted", "Username=" + PassData.isession.Username + ";");
            Invoke("LooseGame", 15f);
        }
        else
        {
             
            NoConnectionPanel.SetActive(false);
            if(Reconnected == false)
            {
                Connect();
                Reconnected = true;
            }

        }

        if (leaveInvokeStarted)
        {
            if(leaveInvokeTimer >= 0)
            {
                leaveInvokeTimer -= Time.deltaTime;
                opponentOfflineDialog.text = "Opponent is offline, the game will end in "+ Mathf.Abs(Mathf.Floor(leaveInvokeTimer));

            }

        }
    }

    private async void Connect()
    {
     
        try
        {
            var keepAliveIntervalSec = 30;
            await PassData.isocket.ConnectAsync(PassData.isession, true, keepAliveIntervalSec);
            if (PassData.isocket.IsConnected)
            {
                await PassData.isocket.JoinMatchAsync(PassData.Match.Id);
                Debug.Log("socket reconnected ");
                UserOffline.SetActive(false);

            }
        }


        catch (TaskCanceledException e)
        {

            Debug.Log("task canceled " + e.Task);
            var retryConfiguration = new Nakama.RetryConfiguration(1, 5, delegate { });

            // Configure the retry configuration globally.
            PassData.iClient.GlobalRetryConfiguration = retryConfiguration;
            await PassData.isocket.JoinMatchAsync(PassData.Match.Id);

        }
    }
 

    public void CloseTheApp()
    {
        Application.Quit();
    }

    async void OnApplicationPause(bool pauseStatus)
    {
#if !UNITY_EDITOR
#if UNITY_ANDROID
        if (pauseStatus)
        {
            // App is paused (moved to background)
            timeWhenPaused = DateTime.Now;
            var state = MatchDataJson.SetAndroidBackground("Background");
            DataSync.Instance.SendMatchState(OpCodes.Android_Background, state);
            wasPaused = true;
        }
        else
        {
            // App is resumed (gained focus)
            if (wasPaused)
            {
    
                TimeSpan timeAway = DateTime.Now - timeWhenPaused;
                StartCoroutine(CheckMyFocus((float)timeAway.TotalSeconds));
                var state = MatchDataJson.SetAndroidBackground("Forground");
                DataSync.Instance.SendMatchState(OpCodes.Android_Background, state);
                wasPaused = false;
            }
        }

#endif


#endif
    }

    void OnApplicationFocus(bool hasFocus)
    {
        #if !UNITY_EDITOR
#if UNITY_ANDROID
        if (hasFocus && wasPaused)
        {
            // App is resumed (gained focus)
            TimeSpan timeAway = DateTime.Now - timeWhenPaused;
            StartCoroutine(CheckMyFocus((float)timeAway.TotalSeconds));
            var state = MatchDataJson.SetAndroidBackground("Forground");
            DataSync.Instance.SendMatchState(OpCodes.Android_Background, state);
            // Connect();
            wasPaused = false;
        }

#endif

#if UNITY_IOS
        if (!hasFocus)
        {
            // App lost focus (equivalent to going to background)
            backgroundStartTime = Time.realtimeSinceStartup;
        }
        else
        {
            currentBackgroundTime = Time.realtimeSinceStartup - backgroundStartTime;
            StartCoroutine(CheckMyFocus(currentBackgroundTime));
             
        }
#endif


        #endif

    }

    public float GetCurrentBackgroundTime()
    {
        return currentBackgroundTime;
    }

    IEnumerator CheckMyFocus(float timeaway)
    {
        yield return new WaitForSeconds(2f);
 
        if (ChessTimer.Instance.OpponentSlider.fillAmount != 0 && ChessTimer.Instance.MySlider.fillAmount != 0 && Mathf.Floor(timeaway) >= 15)
        {

                endGameDialog.text = "You left the game for more than 15 seconds.";
                GameEndResult.Instance.LooserResult(LooserSprite);

        
            
        }
        else if (ChessTimer.Instance.OpponentSlider.fillAmount == 0)
        {
            endGameDialog.text = PassData.otherUsername + " ran out of time.";
            GameEndResult.Instance.WinnerResult();
        }
    }

    //for Android 
    private void CheckBackgroundTime()
    {
        TimeSpan timeAway = DateTime.Now - timeWhenPaused;
        
        if (timeAway.TotalSeconds > 15)
        {
            endGameDialog.text = "You've been away for more than 15 seconds.";
        }



         
    }

}
