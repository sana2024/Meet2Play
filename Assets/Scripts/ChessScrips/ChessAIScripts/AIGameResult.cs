using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using UnityEngine.UI;
using Nakama.TinyJson;
using UnityEngine.SceneManagement;
//using Facebook.Unity;
using UnityEngine.Networking;
using ByteBrewSDK;
using System.Linq;
using System.Threading.Tasks;
using System;

public class AIGameResult : MonoBehaviour
{
    [SerializeField] GameObject EndScreenPanel;
    [SerializeField] Image ResultImage;
    [SerializeField] Sprite WinnerImage;
    [SerializeField] Sprite WhiteLooserImage;
    [SerializeField] Sprite BlackLooserImage;
    [SerializeField] Sprite DrawImage;
    [SerializeField] Sprite RedBackground;
    [SerializeField] Sprite GreenBackground;
    [SerializeField] Sprite YellowBackground;
    [SerializeField] GameObject Reward;
    [SerializeField] Image EndScreenBackground;
    [SerializeField] RawImage MyAvatar;
    [SerializeField] RawImage OpponentAvatar;
    [SerializeField] Text Rewardamount;

    [SerializeField] Text MyRatingText;
    [SerializeField] Text OpponentRatingText;
    [SerializeField] Text MyLevelText;
    [SerializeField] Text OppoenentLevelText;
    [SerializeField] Text MyWinnerScore;
    [SerializeField] Text AIWinnerScore;
    public static  AIGameResult Instance;
    [SerializeField] Text DrawTypeText;
    [SerializeField] GameObject DrawType;
    [SerializeField] Button LeaveButton;
    [SerializeField] GameObject FreezeGame;
    [SerializeField] Button RematchButton;
    [SerializeField] GameObject LeavePanel;
    [SerializeField] GameObject OfferDrawPanel;
    [SerializeField] Button DrawButton;
    [SerializeField] Button RequestDrawButton;
    [SerializeField] GameObject RejectDrawPanel;
    [SerializeField] GameObject RejectRematchPanel;
    [SerializeField] Button AddFriendButton;
    [SerializeField] Sprite FriendAddedSprite;
    [SerializeField] Text OldEloText;
    [SerializeField] Text EloDiffText;


    bool IsLooseRunned = false;
    bool IsDrawRunned = false;

    int oldElo;
    int newElo = 0;


    int matchList =0;
    [SerializeField] GameObject NoConnectionPanel;

    //nakama variables
    ISocket isocket;
    ISession isession;
    IClient iclient;

    // Start is called before the first frame update
    async void Start()
    {

        // LeaveButton.onClick.AddListener(AILooserResult);
        iclient = PassData.iClient;
        isession = PassData.isession;
        isocket = PassData.isocket;
        oldElo = PassData.ChessELO;

        LeaveButton.onClick.AddListener(OpenLeavePanel);

        if (Instance == null)
        {
            Instance = this;
        }


        MyRatingText.text = PassData.ChessELO.ToString();
        MyLevelText.text = PassData.ChessLevel.ToString();
        OpponentRatingText.text = PassData.AIRating.ToString();
        OppoenentLevelText.text = PassData.AIlevel.ToString();


        StartCoroutine(GetTexture(PassData.MyURL , MyAvatar));
        StartCoroutine(GetTexture("https://i.pinimg.com/564x/70/5e/fb/705efbcfb56e45f52636d4e9f441a369.jpg", MyAvatar));

      //  InvokeRepeating("readMatchList", 0.1f, 5);

    }

    public async void AddLeaderboard(long wins)
    {
        const string leaderboardId = "level1";
        long score = wins;
        var r = await iclient.WriteLeaderboardRecordAsync(isession, leaderboardId, score);
        System.Console.WriteLine("New record for '{0}' score '{1}'", r.Username, r.Score);

    }

    public async void leaveInGameStream()
    {

        var payload = JsonWriter.ToJson(new { data = "ingame" });
        await PassData.isocket.RpcAsync("leave_ingame", payload);
    }

    public void AIWinnerResult()
    {
        ByteBrew.NewCustomEvent("Won", "Game=Chess; Type=AI; Username=" + isession.Username + ";");
        AppFlyer.Instance.AppsFlyerEvent("Won_Chess_AI", isession.Username);
        huggingFaceStock.Instance.skill = "";
        ChessTimer.Instance.gameEnded = true;
        ChessBoardAI.instance.isWhiteTurn = true;
        EndScreenPanel.SetActive(true);
        ResultImage.sprite = WinnerImage;
        EndScreenBackground.sprite = GreenBackground;
        PassData.ChessWins++;
        Rewardamount.text = "200";
        PassData.ChessELO = calculateEloChess(1);
        PassData.AIRating = CalculateAIRating(0);
        EloAddOn("win");
        if (PassData.BotLeveling != 20)
        {
          PassData.BotLeveling++;
        }

        MyWinnerScore.text = "1";
        LeaveButton.onClick.RemoveAllListeners();
        LeaveButton.onClick.AddListener(LeaveGame);
        RequestDrawButton.interactable = false;
        string Rating = PlayerPrefs.GetString("progress");
        Rating = Rating + " " + PassData.ChessELO;
        PlayerPrefs.SetString("progress", Rating);
        //ChessTimer.Instance.OpponentTimerText.text = "00:00";
        //ChessTimer.Instance.MyTimerText.text = "00:00";
        updateWallet(200 , 25);
        //UpdateXP(25);
        WriteData();
        UserProfile.instance.WriteData(PassData.level, PassData.wins, PassData.losses, "none", "paris");
        AIfakeCam.Instance.MuteLocalVideo();
        AIfakeCam.Instance.StopWebCam();
        //PassData.AIlevel++;
        //PassData.AIRating += 50;

        AddLeaderboard(PassData.wins + PassData.ChessWins);

    }

    public void AILooserResult(Sprite LooserSprite)
    {
        ByteBrew.NewCustomEvent("Lost", "Game=Chess; Type=AI; Username=" + isession.Username + ";");
        AppFlyer.Instance.AppsFlyerEvent("Lost_Chess_AI", isession.Username);
        LeavePanel.SetActive(false);
        huggingFaceStock.Instance.skill = "";
        ChessTimer.Instance.gameEnded = true;
        ChessBoardAI.instance.isWhiteTurn = true;
        if (IsLooseRunned == false)
        {
            Debug.Log("looser result");
            EndScreenPanel.SetActive(true);
            EndScreenBackground.sprite = RedBackground;
            ResultImage.sprite = LooserSprite;
            PassData.ChessLooses++;
            PassData.ChessELO = calculateEloChess(0);
            PassData.AIRating = CalculateAIRating(1);
            EloAddOn("loose");
            RequestDrawButton.interactable = false;
            if (PassData.BotLeveling != 1)
            {
              PassData.BotLeveling--;
            }
            AIWinnerScore.text = "1";
            LeaveButton.onClick.RemoveAllListeners();
            LeaveButton.onClick.AddListener(LeaveGame);
            string Rating = PlayerPrefs.GetString("progress");
            Rating = Rating + " " + PassData.ChessELO;
            PlayerPrefs.SetString("progress", Rating);
            // ChessTimer.Instance.OpponentTimerText.text = "00:00";
            // ChessTimer.Instance.MyTimerText.text = "00:00";
            leaveInGameStream();
            WriteData();
            UserProfile.instance.WriteData(PassData.level, PassData.wins, PassData.losses, "none", "paris");
            AIfakeCam.Instance.MuteLocalVideo();
            AIfakeCam.Instance.StopWebCam();
            IsLooseRunned = true;
            //PassData.AIlevel--;
            //PassData.AIRating -= 50;
        }

    }

    public void DrawResult(string DrawString)
    {
        ByteBrew.NewCustomEvent("Draw", "Game=Chess; Type=AI; Username=" + isession.Username + ";");
        AppFlyer.Instance.AppsFlyerEvent("Draw_Chess_AI", isession.Username);
        if (IsDrawRunned == false)
        {
            huggingFaceStock.Instance.skill = "";
            ChessTimer.Instance.gameEnded = true;
            ChessBoardAI.instance.isWhiteTurn = true;
            Reward.SetActive(true);
            ResultImage.sprite = DrawImage;
            EndScreenBackground.sprite = YellowBackground;
            EndScreenPanel.SetActive(true);
            AIWinnerScore.text = "1";
            MyWinnerScore.text = "1";
            PassData.ChessDraws++;
            if (PassData.BotLeveling != 20)
            {
                PassData.BotLeveling++;
            }
            Rewardamount.text = "100";
            DrawType.SetActive(true);
            DrawTypeText.text = DrawString;
            RequestDrawButton.interactable = false;
            LeaveButton.onClick.RemoveAllListeners();
            LeaveButton.onClick.AddListener(LeaveGame);
           // ChessTimer.Instance.OpponentTimerText.text = "00:00";
           // ChessTimer.Instance.MyTimerText.text = "00:00";
            updateWallet(100 , 15);
           // UpdateXP(15);
            WriteData();
            UserProfile.instance.WriteData(PassData.level, PassData.wins, PassData.losses, "none", "paris");
            IsDrawRunned = true;
            //PassData.AIlevel++;
            //PassData.AIRating += 50;
        }

    }

    public async void updateWallet(int coins , int xp)
    {


        var payload = JsonWriter.ToJson(new { coins = coins , xp = xp});
        var rpcid = "Update_Wallet";
        var WalletRPC = await PassData.iClient.RpcAsync(PassData.isession, rpcid, payload);



    }

    public async void WriteData()
    {
        var Datas = new ChessDataObj
        {
            description = PassData.PlayerDesc,
            flag = "",
            ChessLevel = PassData.ChessLevel.ToString(),
            ChessElo = PassData.ChessELO.ToString(),
            chesswin = PassData.ChessWins.ToString(),
            chessloses = PassData.ChessLooses.ToString(),
            chessDraw = PassData.ChessDraws.ToString(),
            AILeveling = PassData.BotLeveling



        };


        var Sendata = await iclient.WriteStorageObjectsAsync(isession, new[] {
        new WriteStorageObject
       {
      Collection = "ChessDatabase",
      Key = "Data",
      Value = JsonWriter.ToJson(Datas),
      Version = PassData.version,
      PermissionRead=2,

       }
});

    }


    public async void UpdateXP(int xp)
    {
        var payload = JsonWriter.ToJson(new { xp = xp });
        var rpcid = "Update_XP";
        var WalletRPC = await PassData.iClient.RpcAsync(PassData.isession, rpcid, payload);

    }

    public int calculateEloChess(int Result)
    {

        var Expected = 1.0f / (1 + (Mathf.Pow(10, 1.0f * (PassData.AIRating - PassData.ChessELO) / 400)));
        PassData.ChessELO = (int)(PassData.ChessELO + 30 * (Result - Expected));
        newElo = PassData.ChessELO;

        return PassData.ChessELO;

    }

    public int EloAddOn(string result)
    {
        var diffrence = oldElo - newElo;
        var absDiffrence = Math.Abs(diffrence);

        OldEloText.text = oldElo.ToString();

        if (result == "win")
        {
            EloDiffText.color = Color.green;
            EloDiffText.text = "+ " + absDiffrence;
        }

        if (result == "loose")
        {
            EloDiffText.color = Color.red;
            EloDiffText.text = "- " + absDiffrence;
        }

        return absDiffrence;
    }

    public void OpenLeavePanel()
    {
        LeavePanel.SetActive(true);
    }

    public void CloseLeavePanel()
    {
        LeavePanel.SetActive(false);
    }

    public void LeaveMatch()
    {
        LeaveGame();
    }

    public async void LeaveGame()
    {
        var result1 = await UserProfile.instance.client.ListUsersStorageObjectsAsync(UserProfile.instance.session, "MatchList", "00000000-0000-0000-0000-000000000000", 1);
        if (result1.Objects.Any())
        {
            foreach (var ob in result1.Objects)
            {
                var datas = JsonParser.FromJson<MatchCount>(ob.Value);
                matchList = int.Parse(datas.matchCount);
                if(matchList != 0)
                {
                matchList--;
             //   listMatches.Instance.InsertIntoMatchList(matchList.ToString());
                }

            }
        }
        leaveInGameStream();
        SceneManager.LoadScene("Menu");
    }
    
    //public void FacebookShareLink()
    //{
    //    FB.Mobile.ShareDialogMode = ShareDialogMode.NATIVE;
    //    FB.ShareLink(
    //            new System.Uri("https://www.meet2play.app/download"),
    //            callback: ShareCallback);

    //}

    //private void ShareCallback(IShareResult result)
    //{
    //    if (result.Cancelled || !string.IsNullOrEmpty(result.Error))
    //    {
    //        Debug.Log("ShareLink Error: " + result.Error);
    //    }
    //    else if (!string.IsNullOrEmpty(result.PostId))
    //    {
    //        // Print post identifier of the shared content
    //        Debug.Log(result.PostId);
    //    }
    //    else
    //    {
    //        // Share succeeded without postID

    //        updateWallet(50 , 5);
    //       // UpdateXP(5);
    //        PlayerPrefs.SetString("sharesplash", "true");
    //        SceneManager.LoadScene("Menu");


    //        Debug.Log("ShareLink success!");
    //    }
    //}


    IEnumerator GetTexture(string URL, RawImage avatar)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(URL);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;

            avatar.texture = myTexture;
        }

    }

    //Texture2D CropTexture(Texture2D originalTexture)
    //{
    //    int size = Mathf.Min(originalTexture.width, originalTexture.height);
    //    int offsetX = (originalTexture.width - size) / 2;
    //    int offsetY = (originalTexture.height - size) / 2;

    //    Color[] pixels = originalTexture.GetPixels(offsetX, offsetY, size, size);

    //    Texture2D squareTexture = new Texture2D(size, size);
    //    squareTexture.SetPixels(pixels);
    //    squareTexture.Apply();

    //    return squareTexture;
    //}
    public void Rematch()
    {
        RematchButton.interactable = false;
        StartCoroutine(ResetScene());

    }


    IEnumerator ResetScene()
    {
        yield return new WaitForSeconds(3);

        var pick = UnityEngine.Random.Range(0, 2);


        if(pick == 1)
        {
        SceneManager.LoadScene("ChessAI");
        }

        if(pick == 0)
        {
            RejectRematchPanel.SetActive(true);
            yield return new WaitForSeconds(5);
            RejectRematchPanel.SetActive(false);
        }
        ByteBrew.NewCustomEvent("rematch", "Game=Chess; Type=AI; Username=" + isession.Username + ";");


    }

    // Update is called once per frame
    void Update()
    {

       


        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            NoConnectionPanel.SetActive(true);
        }
        else
        {

            NoConnectionPanel.SetActive(false);

        }
    }

    public void CloseTheApp()
    {
        Application.Quit();
    }

    public void CloseEndPanel()
    {
        EndScreenPanel.SetActive(false);
        FreezeGame.SetActive(true);
 
    }

    public void MonitorLeave()
    {
        ByteBrew.NewCustomEvent("LeftGame", "Username=" + PassData.isession.Username + ";");
    }

    public int CalculateAIRating(int Result)
    {
        var Expected = 1.0f / (1 + (Mathf.Pow(10, 1.0f * (PassData.ChessELO - PassData.AIRating) / 400)));
        PassData.AIRating = (int)(PassData.AIRating + 30 * (Result - Expected));

        return PassData.AIRating;
    }

    public void OpenDrawOfferPanel()
    {
        OfferDrawPanel.SetActive(true);
    }

    public void CloseDrawOfferPanel()
    {
        OfferDrawPanel.SetActive(false);
    }

    public void SendDrawRequest()
    {
        DrawButton.interactable = false;
        RequestDrawButton.interactable = false;
        StartCoroutine(DrawGame());
    }

    IEnumerator DrawGame()
    {
        yield return new WaitForSeconds(3);

        var pick = UnityEngine.Random.Range(0, 2);

        if(pick == 1)
        {
        OfferDrawPanel.SetActive(false);
        DrawResult("Draw By Agreement");
        }

        if(pick== 0)
        {
            OfferDrawPanel.SetActive(false);
            RejectDrawPanel.SetActive(true);

            yield return new WaitForSeconds(2);

            RejectDrawPanel.SetActive(false);

        }
    }

    public  void AddFriend()
    {
        AddFriendButton.image.sprite = FriendAddedSprite;
        AddFriendButton.interactable = false;
 

    }

 async void readMatchList()
    {
        var result1 = await UserProfile.instance.client.ListUsersStorageObjectsAsync(UserProfile.instance.session, "MatchList", "00000000-0000-0000-0000-000000000000", 1);
        if (result1.Objects.Any())
        {
            foreach (var ob in result1.Objects)
            {
                var datas = JsonParser.FromJson<MatchCount>(ob.Value);
                matchList = int.Parse(datas.matchCount);
                
                 
            }
        }
    }
 
    private void OnApplicationPause(bool pause)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (pause)
        {
                if(matchList != 0)
                {
            matchList--;
          //  listMatches.Instance.InsertIntoMatchList(matchList.ToString());
                }

        }

        }

    }
    
    private void OnApplicationQuit()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXEditor)
        {
            if(matchList != 0)
            {
               matchList--;

          //  listMatches.Instance.InsertIntoMatchList(matchList.ToString());
            }

        }

    }
}
