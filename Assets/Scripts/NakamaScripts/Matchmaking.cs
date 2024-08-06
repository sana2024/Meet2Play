using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using System;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;
using System.Threading.Tasks;
using Nakama.TinyJson;
using UnityEngine.Networking;
using Nakama.Ninja.WebSockets;
using Random = UnityEngine.Random;
using ByteBrewSDK;
using AppsFlyerSDK;

public class Matchmaking : MonoBehaviour
{
    [SerializeField] NakamaConnection Nconnect;
    ISocket isocket;
    IClient iclient;
    ISession isession;
    private string ticket;
    static IMatch currentMatch;
    string OtherId;

    IUserPresence hostPresence;
    IUserPresence SecondPresence;
    string wating = "true";

    bool MatchFound;

    //to recive notification when no one was found
    float FindTimer;
    bool EnteredWaitingPhase = false;
    bool EnteredGame = false;
    bool waitedMoreThan30s = false;

    IMatch match;


    [SerializeField] public GameObject SearchingPanel;
    [SerializeField] GameObject SearchingBackground;
    [SerializeField] GameObject SearingTitle;
    [SerializeField] UserProfile userProfile;
    [SerializeField] GameObject NoEnoughCoinPanel;
    [SerializeField] GameObject RejectPanel;
    [SerializeField] RawImage OpponentImage;
    [SerializeField] ArabicText OponentUsername;
    [SerializeField] GameObject ProgressBar;
    [SerializeField] GameObject HiddenUser;
    [SerializeField] GameObject PlayButton;
    [SerializeField] Button CancleButton;
    [SerializeField] GameObject LostconnectionPanel;
    [SerializeField] Button ChallangePlayButton;


    UnityWebRequest www;
    
    string query;
    Dictionary<string, string> properity;

    public string matchType;


    //if chess or backgammon didnt connect to a real person then load AI scene
   public  bool enteredChessWaiting = false;
   public  float ChesswaitingTime = 0;
   public bool enteredBackgammonWaiting = false;
   public float backgammonWaitingTime = 0;

    int runChecker;
    //creating face personality for AI
    string FakeAIUsername;
    [SerializeField] GameObject FakeAIProfile;


    //cancling ticket if the game connected to AI
    bool isTicketCancled = false;
    int ChessPrice;


    public static Matchmaking Instance;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    public async void Start()
    {
 
        isession = PassData.isession;
        isession = await PassData.iClient.SessionRefreshAsync(isession);
        iclient = PassData.iClient;
        isocket = PassData.isocket;
        var mainThread = UnityMainThreadDispatcher.Instance();
        isocket.ReceivedMatchmakerMatched += match => mainThread.Enqueue(() => OnReceivedMatchmakerMatched(match));
        isocket.ReceivedMatchPresence += m => mainThread.Enqueue(() => OnRecivedMatchPresence(m));
        isocket.ReceivedMatchState += m => mainThread.Enqueue(async () => await OnReceivedMatchState(m));
        isocket.Closed += () => ReConnect();

        ByteBrew.NewCustomEvent("OpenedMenu", "Username=" + isession.Username + ";");

        CancleButton.onClick.AddListener(RemoveTicket);
 

    }

    public async void Update()
    {

        if (enteredChessWaiting)
        {
            ChesswaitingTime += Time.deltaTime;
            isTicketCancled = true;

            if(int.Parse(OnlineUsers.instance.OnlineCounterText.text) < 1)
            {
                Invoke("ConnectToAI", 30);
            }
        }

        if(enteredBackgammonWaiting)
        {
            backgammonWaitingTime += Time.deltaTime;
            isTicketCancled = true;

            if (int.Parse(OnlineUsers.instance.OnlineCounterText.text) < 1)
            {
                Invoke("ConnectToBackgammonAI", 30);
            }}
        if (ChesswaitingTime > 30)
        {
            ConnectToAI();

        }
        if (backgammonWaitingTime > 30)
        {
            ConnectToBackgammonAI();
        }
        // Debug.Log(FindTimer);
        if (EnteredWaitingPhase)
        {
            FindTimer += Time.deltaTime;
        }

        if (FindTimer > 4)
        {
            waitedMoreThan30s = true;
        }
        if (Camera.main.aspect <= 1.5f)
        {
            SearchingBackground.transform.localScale = new Vector3(1.002772f, 1.12765f, 0.2982222f);
            RectTransform titleRect = SearingTitle.GetComponent<RectTransform>();
            RectTransform CancleRect = CancleButton.gameObject.GetComponent<RectTransform>();
            titleRect.anchoredPosition = new Vector2(0, -114);
            titleRect.localScale = new Vector3(0.9f, 0.8f, 0.8f);

            CancleRect.anchoredPosition = new Vector2(-93, -7);
            CancleRect.localScale = new Vector3(0.40f, 0.79f, 0.79f);
        }

    }


#if !UNITY_EDITOR
    private async void OnApplicationFocus(bool focus)
    {
        if(focus == false)
        {
        if (waitedMoreThan30s == true && EnteredGame == false)
        {
            userProfile.InsertIntoWaiting(isession.UserId);
        }
        }
            
            iclient = Nconnect.client();
            var keepAliveIntervalSec = 30;
          //  isocket = Socket.From(iclient, new WebSocketAdapter());
            await isocket.ConnectAsync(isession, true, keepAliveIntervalSec);

 
        if (SearchingPanel.activeSelf == true)
        {
            var matchmakingTickets = await isocket.AddMatchmakerAsync(query, 2, 2, properity);

            ticket = matchmakingTickets.Ticket;
        }

       
    }
#endif


    async void ConnectToAI()
    {
 
        matchType = "chessAI";
        PlayButton.SetActive(true);
        OponentUsername.Text = FakeAIUsername;
        PassData.otherUsername = FakeAIUsername;
        SimpleLoading.Instance.rectComponent.gameObject.SetActive(false);
        CalculateAIrating();
        FakeAIProfile.SetActive(true);


        if (isTicketCancled == true)
        {
            enteredChessWaiting = false;
            isTicketCancled = false;
 
            await isocket.RemoveMatchmakerAsync(ticket);
            matchType = "";
            wating = "ingameAI";
            PassData.Queue = wating;
            PassData.BoardType = " ";
            userProfile.WriteData(PassData.level, PassData.wins, PassData.losses, PassData.Queue, PassData.BoardType);


        }
    }

    async void ConnectToBackgammonAI()
    {
        matchType = "backgammonAI";
        PlayButton.SetActive(true);
        OponentUsername.Text = FakeAIUsername;
        PassData.otherUsername = FakeAIUsername;
        SimpleLoading.Instance.rectComponent.gameObject.SetActive(false);
        FakeAIProfile.SetActive(true);



        if (isTicketCancled == true)
        {
            enteredBackgammonWaiting = false;
            isTicketCancled = false;
  
            await isocket.RemoveMatchmakerAsync(ticket);
            matchType = "";
            wating = "ingameAI";
            PassData.Queue = wating;
 
            userProfile.WriteData(PassData.level, PassData.wins, PassData.losses, PassData.Queue, PassData.BoardType);


        }
    }

    private async void ReConnect()
    {
        try
        {
            if (!isocket.IsConnected)
            {
                await isocket.ConnectAsync(isession, true, 30);
                PassData.isocket = isocket;
            }
        }
        catch (Exception e)
        {
 
        }
    }

    public async void SendMatchState(long opCode, string state)
    {
        await isocket.SendMatchStateAsync(PassData.Match.Id, opCode, state);
    }

    private async Task OnReceivedMatchState(IMatchState matchState)
    {
        var state = matchState.State.Length > 0 ? System.Text.Encoding.UTF8.GetString(matchState.State).FromJson<Dictionary<string, string>>() : null;

        switch (matchState.OpCode)
        {
            case 16:

                if (state["Leave"] == "Left")
                {
                    FriendSystem.instance.FriendChanllegePanel.SetActive(false);
                    SearchingPanel.SetActive(false);
                    StartCoroutine(DisplayRejectPanel());
                    await PassData.isocket.LeaveMatchAsync(PassData.Match.Id);
                    matchType = "";
                    wating = "none";
                    PassData.Queue = wating;
                    PassData.BoardType = " ";
                    userProfile.WriteData(PassData.level, PassData.wins, PassData.losses, "none", PassData.BoardType);

                    HiddenUser.SetActive(true);
                    ProgressBar.SetActive(true);
                    StartCoroutine(GetTexture(""));
                    PlayButton.SetActive(false);
                    ChallangePlayButton.gameObject.SetActive(false);
                    SearchingPanel.SetActive(false);

                }


                if (state["Leave"] == "Join")
                {
                    PassData.JoinedPlayers = 2;
                     
                }




                break;

            case 11:

                PassData.RecivedLevel = state["Level"];
                break;

        }
    }

    IEnumerator DisplayRejectPanel()
    {

        RejectPanel.SetActive(true);
        yield return new WaitForSeconds(3);
        RejectPanel.SetActive(false);
    }

    public async void FindBackgammonMatch(string BoardName)
    {
        matchType = "backgammon";
        ByteBrew.NewCustomEvent("SrearchedForGame", "type=Backgammon; Username=" + isession.Username + ";");

        AppFlyer.Instance.AppsFlyerEvent("SearchedForBackgammon", isession.Username);

        FakeAIUsername = "Player" + UnityEngine.Random.Range(0, 5000);
        var result1 = await iclient.ListUsersStorageObjectsAsync(isession, "wait", "00000000-0000-0000-0000-000000000000", 1);
        PlayButton.SetActive(false);
        SimpleLoading.Instance.rectComponent.gameObject.SetActive(true);
        foreach (var userid in result1.Objects)
        {
           
            if (userid.Value.Count() > 30)
            {
                var id = userid.Value.Remove(0, 12);
                id = id.Remove(36, 2);
               // Debug.Log("dissapointed user "+ id);
                if (id != PassData.isession.UserId)
                {
                    userProfile.SendNotificationWaiting(id, "backgammon");
                    userProfile.InsertIntoWaiting("");

                }
            }
            else
            {
                
            }}
        EnteredWaitingPhase = true;
        OponentUsername.Text = "";
        PassData.BoardType = BoardName;

        int wallet = Math.Abs(PassData.WalletMoney);
        int boardPrice = Math.Abs(PassData.BoardPrice);
        if (wallet >= boardPrice)
        {
           // ChessUserDatas.Instance.UpdateXP(5);
            SearchingPanel.SetActive(true);
            LostconnectionPanel.SetActive(false);
            enteredBackgammonWaiting = true;

           // Debug.Log("find a backgammon match");
           properity = new Dictionary<string, string>() {
          {"board", "paris"},
          {"game", "backgammon"}
         };
            var query = "+properties.board:paris +properties.game:backgammon ";


            var matchmakingTickets = await isocket.AddMatchmakerAsync(query, 2, 2, properity);


            ticket = matchmakingTickets.Ticket;

            wating = "backgammon";
            PassData.Queue = wating;

            PassData.Board = PassData.BoardType;
            var level = PlayerPrefs.GetInt("level");
            userProfile.WriteData(level, PassData.wins, PassData.losses, PassData.Queue, PassData.Board);


//            userProfile.Storageojectcounter();

            isession = await iclient.SessionRefreshAsync(isession);
            var account = await iclient.GetAccountAsync(isession);

        }
        else
        {
            NoMoneyPanelTimer();
       
        }

        OnlineUsers.instance.sendToAvaiables("backagmmon", PlayerPrefs.GetInt("time").ToString());

    }

    public async void FindChessMatch(int BoardPrice)
    {
        int wallet = Math.Abs(PassData.WalletMoney);
        EnteredWaitingPhase = true;
        ByteBrew.NewCustomEvent("SrearchedForGame", "type=Chess; Username=" + isession.Username + ";");

        AppFlyer.Instance.AppsFlyerEvent("SearchedForChess", isession.Username);

        FakeAIUsername = "Player" + UnityEngine.Random.Range(0, 5000);
        var time = PlayerPrefs.GetInt("time");
        var result1 = await iclient.ListUsersStorageObjectsAsync(isession, "wait", "00000000-0000-0000-0000-000000000000", 1);

        PlayButton.SetActive(false);
        SimpleLoading.Instance.rectComponent.gameObject.SetActive(true);
        foreach (var userid in result1.Objects)
        {

            if (userid.Value.Count() > 30)
            {
                var id = userid.Value.Remove(0, 12);
                id = id.Remove(36, 2);
                if (id != PassData.isession.UserId)
                {

                    userProfile.SendNotificationWaiting(id, "chess@" + time);
                    userProfile.InsertIntoWaiting("");

                }
            }
            else
            {

            }
        }

        if (FakeAIUsername == PassData.Username)
        {
            FakeAIUsername = "Player" + UnityEngine.Random.Range(0, 5000);
        }
        if (wallet >= BoardPrice)
        {
           // ChessUserDatas.Instance.UpdateXP(5);
            ChessPrice = BoardPrice;
            OponentUsername.Text = "";
            enteredChessWaiting = true;
            matchType = "chess";
            PlayButton.SetActive(false);
            SearchingPanel.SetActive(true);
            SimpleLoading.Instance.rectComponent.gameObject.SetActive(true);
            //enteredWaitingPhase = true;
            var stringProperties = new Dictionary<string, string>() {
    {"time", time.ToString()},
    {"game", "chess"},
};

            var numericProperties = new Dictionary<string, double>() {
    {"skill", PassData.ChessELO}
};

            var first_attempt_min = PassData.ChessELO - 50;
            var first_attempt_max = PassData.ChessELO + 50;

            var second_attempt_min = PassData.ChessELO - 100;
            var second_attempt_max = PassData.ChessELO + 100;

            var third_attempt_min = PassData.ChessELO - 250;
            var third_attempt_max = PassData.ChessELO + 250;

            var fourth_attempt_min = PassData.ChessELO - 3000;
            var fourth_attempt_max = PassData.ChessELO + 3000;


            var first_query = "+properties.game:chess +properties.time:" + time + " +properties.skill:>=" + first_attempt_min + "^4 properties.skill:<=" + first_attempt_max + "^4 properties.skill:>=" + second_attempt_min + "^3 properties.skill:<=" + second_attempt_max +"^3 properties.skill:>=" + third_attempt_min + "^2 properties.skill:<=" + third_attempt_max + "^2 properties.skill:>=" + fourth_attempt_min + " +properties.skill:<=" + fourth_attempt_max;

            var matchmakingTickets = await PassData.isocket.AddMatchmakerAsync(first_query, 2, 2, stringProperties, numericProperties);
            ticket = matchmakingTickets.Ticket;

            wating = "chess+"+ time;
            PassData.Queue = wating;

            PassData.Board = PassData.BoardType;
            var level = PlayerPrefs.GetInt("level");
            userProfile.WriteData(level, PassData.wins, PassData.losses, "chess+" + time, "");


        }
        else
        {
            NoMoneyPanelTimer();
        }

        OnlineUsers.instance.sendToAvaiables("chess", PlayerPrefs.GetInt("time").ToString());
    }





    void NoMoneyPanelTimer()
    {
        NoEnoughCoinPanel.SetActive(true);

    }

    public async void RemoveTicket()
    {
        CancelInvoke();
        enteredChessWaiting = false;
        ChesswaitingTime = 0;

        enteredBackgammonWaiting = false;
        backgammonWaitingTime = 0;

        ByteBrew.NewProgressionEvent(ByteBrewProgressionTypes.Failed, "Chess", "ChessAI");
        FakeAIProfile.SetActive(false);
        FindTimer = 0;
        UserProfile.instance.AddXP(-5);

        wating = "none";
        PassData.Queue = wating;
        PassData.BoardType = " ";
        userProfile.WriteData(PassData.level, PassData.wins, PassData.losses, "none", PassData.BoardType);

        if (MatchFound == false)
        {
            SearchingPanel.SetActive(false);
            LostconnectionPanel.SetActive(false);
            if (matchType != "chessAI" || matchType != "backgammonAI")
            {
                if(ticket != null)
                {
                await isocket.RemoveMatchmakerAsync(ticket);
                }

            }
            if (matchType == "chessAI" || matchType == "chess")
            {
                ByteBrew.NewCustomEvent("CanceledGame", "type=Chess; Username=" + isession.Username + ";");
                UserProfile.instance.BonusWallet(ChessPrice);
            }

            if (matchType == "backgammon")
            {
                ByteBrew.NewCustomEvent("CanceledGame", "type=Backgammon; Username=" + isession.Username + ";");
                int boardPrice = Math.Abs(PassData.BoardPrice);
                UserProfile.instance.BonusWallet(boardPrice);
            }

       
          //  ChessUserDatas.Instance.UpdateXP(-5);

        }

        matchType = "";
 
    }

    public async void OnApplicationQuit()
    {
        ByteBrew.NewCustomEvent("ClosedApp", "Username=" + PassData.isession.Username + ";");
        if (waitedMoreThan30s == true && EnteredGame == false)
        {
            userProfile.InsertIntoWaiting(isession.UserId);

        }
        PassData.BoardType = "paris";
        if (PassData.BoardType != null)
        {
            wating = "none";
            var level = PlayerPrefs.GetInt("level");
            var win = PlayerPrefs.GetInt("wins");
            var losses = PlayerPrefs.GetInt("looses");
            var board = "paris";

            UserProfile.instance.WriteData(level, win, losses, "none", "paris");


        }
        await isocket.RemoveMatchmakerAsync(ticket);


    }

    public async void OnApplicationPause(bool pause)
    {
        ByteBrew.NewCustomEvent("ClosedApp", "Username=" + PassData.isession.Username + ";");
        /*
           if (waitedMoreThan30s == true && EnteredGame == false)
           {
               userProfile.InsertIntoWaiting(isession.UserId);
           }s

         */
        PassData.BoardType = "paris";
        if (PassData.BoardType != null)
        {
            wating = "none";
            var level = PlayerPrefs.GetInt("level");
            var win = PlayerPrefs.GetInt("wins");
            var losses = PlayerPrefs.GetInt("looses");
            var board = PassData.BoardType;

            UserProfile.instance.WriteData(level, win, losses, "none", "paris");

        }

        await isocket.RemoveMatchmakerAsync(ticket);


    }
 
    private async void OnReceivedMatchmakerMatched(IMatchmakerMatched matchmakerMatched)
    {
         
        CancelInvoke();
        ByteBrew.NewProgressionEvent(ByteBrewProgressionTypes.Started, "Chess", "ChessOnline");
        //when a chess match was found cancle the waiting face of the AI
        enteredChessWaiting = false;
        ChesswaitingTime = 0;

        enteredBackgammonWaiting = false;
        backgammonWaitingTime = 0;

        //these variables are specific for backagmmon, if a game was found you will be kicked out of wating list and you will not recieve notification based on that
        if (matchType == "backgammon" || matchType == "chess")
        {
            EnteredGame = true;
            EnteredWaitingPhase = false;
            FindTimer = 0;

            MatchFound = true;

            wating = "none";

 

        }


        PassData.Queue = wating;
        userProfile.WriteData(PassData.level, PassData.wins, PassData.losses, "ingame", "");



        var users = matchmakerMatched.Users;
        try
        {
            match = await isocket.JoinMatchAsync(matchmakerMatched);


        }
        catch (TaskCanceledException ex)
        {
            LostconnectionPanel.SetActive(true);
          
            var keepAliveIntervalSec = 30;
            //  isocket = Socket.From(iclient, new WebSocketAdapter());
            await isocket.ConnectAsync(isession, true, keepAliveIntervalSec);
            match = await isocket.JoinMatchAsync(matchmakerMatched);

        }

        hostPresence = matchmakerMatched.Users.OrderBy(x => x.Presence.SessionId).First().Presence;
        SecondPresence = matchmakerMatched.Users.OrderBy(x => x.Presence.SessionId).Last().Presence;

        PassData.hostPresence = hostPresence.UserId;
        PassData.SecondPresence = SecondPresence.UserId;


        currentMatch = match;
        PassData.Match = currentMatch;
        PlayerPrefs.SetString("matchID", currentMatch.Id);

        PlayerPrefs.SetString("MatchSelf", currentMatch.Self.UserId);
        foreach (var presence in match.Presences)
        {

            CancleButton.onClick.AddListener(RejectGame);

            if (presence.UserId != match.Self.UserId)
            {
                PassData.OtherUserId = presence.UserId;

                PassData.OtherPresence = presence;

                isession = await iclient.SessionRefreshAsync(isession);
                var ids = new[] { presence.UserId };
                var result = await iclient.GetUsersAsync(isession, ids);



                foreach (var u in result.Users)
                {


                    if (u.Username != match.Self.Username)
                    {
                        PassData.OpponentURL = u.AvatarUrl;
                        OponentUsername.Text = u.Username;
                        PassData.otherUsername = u.Username;
                        StartCoroutine(GetTexture(u.AvatarUrl));
                    }
                }

            }

            if (presence.UserId == match.Self.UserId)
            {
                PassData.MyPresense = presence;
            }


        }

        var state = MatchDataJson.SetLevel(PassData.ChessLevel.ToString());
        SendMatchState(OpCodes.Player_Level, state);
 
 

    }

    private async void OnRecivedMatchPresence(IMatchPresenceEvent matchPresenceEvent)
    {
        
        CancelInvoke();
        MatchFound = true;

        foreach (var user in matchPresenceEvent.Joins)
        {

            CancleButton.onClick.AddListener(RejectGame);

            var UserId = PlayerPrefs.GetString("MatchSelf");

            if (user.UserId != UserId)
            {
                PassData.OtherUserId = user.UserId;

                PassData.OtherPresence = user;
                isession = await iclient.SessionRefreshAsync(isession);
                var ids = new[] { user.UserId };
                var result = await iclient.GetUsersAsync(isession, ids);

                foreach (var u in result.Users)
                {
 
                    if (u.Username != PassData.Username)
                    {
                        PassData.otherUsername = u.Username;
                        OponentUsername.Text = u.Username;

                    }
 
                        StartCoroutine(GetTexture(u.AvatarUrl));
                 }

            }

        }
 

    }

    public async void AcceptGame()
    {
 
        if (matchType == "backgammon" || PassData.ChallangeType == "backgammon")
        {
            ByteBrew.NewCustomEvent("FoundGame", "type=Backgammon; Username=" + isession.Username + ";");
            AppFlyer.Instance.AppsFlyerEvent("FoundBackgammon", isession.Username);
            var state = MatchDataJson.SetLeaveMatch("Join");
            SendMatchState(OpCodes.Reject_Match, state);
            OnlineUsers.instance.LeaveAvailables();
            SceneManager.LoadScene("GameScene");
            PassData.IsFirstRound = true;
           
 
           
        }

        if (matchType == "chess" || PassData.ChallangeType == "chess")
        {
            ByteBrew.NewCustomEvent("FoundGame", "type=ChessOnline; Username=" + isession.Username + ";");
            AppFlyer.Instance.AppsFlyerEvent("FoundChess", isession.Username);
            var state = MatchDataJson.SetLeaveMatch("Join");
            
            SendMatchState(OpCodes.Reject_Match, state);
            OnlineUsers.instance.LeaveAvailables();
            SceneManager.LoadScene("Chess");
            
  
            
        }

        if (matchType == "chessAI")
        {
            ByteBrew.NewCustomEvent("FoundGame", "type=ChessAI; Username=" + isession.Username + ";");
            AppFlyer.Instance.AppsFlyerEvent("FoundChessAI", isession.Username);
            //listMatches.Instance.readMachList();
            //PassData.matchCount++;
            //listMatches.Instance.InsertIntoMatchList(PassData.matchCount.ToString());
            OnlineUsers.instance.LeaveAvailables();
            SceneManager.LoadScene("ChessAI");

            
           
          
        }


        if (matchType == "backgammonAI")
        {
            ByteBrew.NewCustomEvent("FoundGame", "type=BackgammonAI; Username=" + isession.Username + ";");
            AppFlyer.Instance.AppsFlyerEvent("FoundbackgammonAI", isession.Username);
            //listMatches.Instance.readMachList();
            //PassData.matchCount++;
            //listMatches.Instance.InsertIntoMatchList(PassData.matchCount.ToString());
            OnlineUsers.instance.LeaveAvailables();
             
            SceneManager.LoadScene("BackgammonAI");

            
 
          
        }

    }

    public async void RejectGame()
    {
        
        Notifications.Instance.ChallangePanelCancelBtn.onClick.RemoveListener(RejectGame);
        SearchingPanel.SetActive(false);
        MatchFound = false;
        var state = MatchDataJson.SetLeaveMatch("Left");
        SendMatchState(OpCodes.Reject_Match, state);

        await PassData.isocket.LeaveMatchAsync(PassData.Match.Id);


        HiddenUser.SetActive(true);
        ProgressBar.SetActive(true);
        StartCoroutine(GetTexture(""));
        PlayButton.SetActive(false);
        matchType = "";

    }

    IEnumerator GetTexture(string url)
    {


        www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
   
        }
        else
        {
            Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;

            if (PassData.FacebookID != null)
            {

                if (url != PassData.MyURL)
                {
                    OpponentImage.texture = FetchTexture.Instance.CropTexture((Texture2D)myTexture);
                    PassData.OtherPlayerTexture = FetchTexture.Instance.CropTexture((Texture2D)myTexture);
                }
                else
                {
                 
                }

            }
            else
            {
                if (PassData.otherUsername.Contains("Player") && url == "https://cdn3.iconfinder.com/data/icons/inficons/512/apple.png")
                {
                }
                else if (PassData.otherUsername.Contains("Player") && url == "https://play-lh.googleusercontent.com/szHQCpMAb0MikYIhvNG1MlruXFUggd6DJHXkMPG1H4lJPB7Lee_BkODfwxpQazxfO9mA")
                {
                }
                else
                {
                    OpponentImage.texture = FetchTexture.Instance.CropTexture((Texture2D)myTexture);
                    
                }



            }
        }

    }

    public String GetPlayerChessSkill(int Rating)
    {
        if (Rating <= 1200)
        {
            PassData.SkillLevel = "Beginner";
            return "Beginner";
 
        }

        if (Rating > 1200 && Rating < 2000)
        {
            PassData.SkillLevel = "Intermediate";
            return "Intermediate";
        }

        if (Rating >= 2000)
        {
            PassData.SkillLevel = "Advanced";
            return "Advanced";
        }
        else
        {
            return "";
        }
    }

    public void CalculateAIrating()
    {
        switch (PassData.BotLeveling)
        {
            case <= 1:
                PassData.AIRating = Random.Range(950,1001);
                PassData.AIlevel = 1;
                break;

            case 2:
                PassData.AIRating = 1050;
                PassData.AIlevel = 2;
                break;

            case 3:
                PassData.AIRating = 1100;
                PassData.AIlevel = 3;
                break;


            case 4:
                PassData.AIRating = 1150;
                PassData.AIlevel = 4;
                break;

            case 5:
                PassData.AIRating = 1200;
                PassData.AIlevel = 5;
                break;

            case 6:
                PassData.AIRating = 1250;
                PassData.AIlevel = 6;
                break;

            case 7:
                PassData.AIRating = 1300;
                PassData.AIlevel = 7;
                break;

            case 8:
                PassData.AIRating = 1350;
                PassData.AIlevel = 8;
                break;

            case 9:
                PassData.AIRating = 1400;
                PassData.AIlevel = 9;
                break;

            case 10:
                PassData.AIRating = 1450;
                PassData.AIlevel = 10;
                break;


            case 11:
                PassData.AIRating = 1500;
                PassData.AIlevel = 11;
                break;


            case 12:
                PassData.AIRating = 1550;
                PassData.AIlevel = 12;
                break;


            case 13:
                PassData.AIRating = 1600;
                PassData.AIlevel = 13;
                break;


            case 14:
                PassData.AIRating = 1650;
                PassData.AIlevel = 14;
                break;


            case 15:
                PassData.AIRating = 1700;
                PassData.AIlevel = 15;
                break;


            case 16:
                PassData.AIRating = 1750;
                PassData.AIlevel = 16;
                break;


            case 17:
                PassData.AIRating = 1800;
                PassData.AIlevel = 17;
                break;

            case 18:
                PassData.AIRating = 1850;
                PassData.AIlevel = 18;
                break;

            case 19:
                PassData.AIRating = 1900;
                PassData.AIlevel = 19;
                break;


            case >= 20:
                PassData.AIRating = 2000;
                PassData.AIlevel = 20;
                break;

        }

    }


}