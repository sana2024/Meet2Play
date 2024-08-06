using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ByteBrewSDK;
//using Facebook.Unity;
using Nakama;
using Nakama.TinyJson;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BackAIResult : MonoBehaviour
{
    [SerializeField] GameObject EndGameScreenPanel;
    [SerializeField] Image EndScreenBackground;
    [SerializeField] Sprite RedBackground;
    [SerializeField] Sprite GreenBackground;
    [SerializeField] GameObject LosserImage;
    [SerializeField] GameObject WinnerImage;
    [SerializeField] GameObject menuPanel;
    [SerializeField] Button RematchButton;
    public static BackAIResult Instance;
    [SerializeField] GameObject RejectRematchPanel;
    [SerializeField] ArabicText MyUsername;
    [SerializeField] ArabicText OpponentUsername;
    [SerializeField] Text Mylevel;
    [SerializeField] Text OpponentLevel;
    [SerializeField] RawImage MyAvatar;
    [SerializeField] Text MyPoint;
    [SerializeField] Text OppoenentPoint;
    [SerializeField] Text BetAmountEnd;
    [SerializeField] Button AddFriendButton;
    [SerializeField] Sprite FriendAddedSprite;
    [SerializeField] GameObject LeavePanel;

    IClient client;
    ISession session;

    int matchList = 0;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    private async void Start()
    {

        client = PassData.iClient;
        session = PassData.isession;
        MyUsername.Text = PassData.Username;
        OpponentUsername.Text = PassData.otherUsername;
        Mylevel.text = PassData.ChessLevel.ToString();
        OpponentLevel.text = Random.Range(1, 5).ToString();
        StartCoroutine(GetTexture(PassData.MyURL , MyAvatar));
        //InvokeRepeating("readMatchList", 0.1f, 5);
    }

    public void OpenLeavePanel()
    {
        LeavePanel.SetActive(true);
    }

    public void CloseLeavePanel()
    {
        LeavePanel.SetActive(false);
    }

    public async void leaveInGameStream()
    {

        var payload = JsonWriter.ToJson(new { data = "ingame" });
        await PassData.isocket.RpcAsync("leave_ingame", payload);
    }

    public void AIWinnerResult()
    {
        EndGameScreenPanel.SetActive(true);
        EndScreenBackground.sprite = GreenBackground;
        WinnerImage.SetActive(true);
        PassData.wins++;
        MyPoint.text = "1";
        updateWallet(PassData.betAmount, 25);
        UpdateXP(25);
        WriteWinsAndLosses(PassData.level , PassData.wins , PassData.losses);
        AppFlyer.Instance.AppsFlyerEvent("Won_Backgammon_AI", session.Username);

    }

    public void AILooserResult()
    {

        CloseLeavePanel();
        EndGameScreenPanel.SetActive(true);
        EndScreenBackground.sprite = RedBackground;
        PassData.losses++;
        LosserImage.SetActive(true);
        OppoenentPoint.text = "1";
        WriteWinsAndLosses(PassData.level, PassData.wins, PassData.losses);
        AppFlyer.Instance.AppsFlyerEvent("Lost_Backgammon_AI", session.Username);
    }

    public void OnMenubuttonClicked()
    {
        menuPanel.SetActive(true);
    }

    public void OnMenuCancleClicked()
    {
        menuPanel.SetActive(false);
    }

    public void LeaveGame()
    {
        AILooserResult();
    }
 
    public async void LeaveScene()
    {
        //var result1 = await UserProfile.instance.client.ListUsersStorageObjectsAsync(UserProfile.instance.session, "MatchList", "00000000-0000-0000-0000-000000000000", 1);
        //if (result1.Objects.Any())
        //{
        //    foreach (var ob in result1.Objects)
        //    {
        //        var datas = JsonParser.FromJson<MatchCount>(ob.Value);
        //        matchList = int.Parse(datas.matchCount);
        //        if(matchList != 0)
        //        {
        //        matchList--;
        //       // listMatches.Instance.InsertIntoMatchList(matchList.ToString());
        //        }


        //    }
        //}
        leaveInGameStream();
        SceneManager.LoadScene("Menu");
    }

    public void SendplayAgainRequest()
    {
        RematchButton.interactable = false;
        StartCoroutine(AcceptREmatchRequest());
    }

    IEnumerator AcceptREmatchRequest()
    {
        yield return new WaitForSeconds(3);

        var pick = Random.Range(0, 2);


        if (pick == 1)
        {
            updateWallet(PassData.BoardPrice , 5);
            SceneManager.LoadScene("BackgammonAI");
            ByteBrew.NewCustomEvent("rematch", "Game=backgammon; Type=AI; Username=" + session.Username + ";");
        }

        if (pick == 0)
        {
            RejectRematchPanel.SetActive(true);
            yield return new WaitForSeconds(5);

        }
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

    //       // updateWallet(50, 5);
    //        // UpdateXP(5);
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

            avatar.texture = CropTexture((Texture2D)myTexture);
        }

    }

    Texture2D CropTexture(Texture2D originalTexture)
    {
        int size = Mathf.Min(originalTexture.width, originalTexture.height);
        int offsetX = (originalTexture.width - size) / 2;
        int offsetY = (originalTexture.height - size) / 2;

        Color[] pixels = originalTexture.GetPixels(offsetX, offsetY, size, size);

        Texture2D squareTexture = new Texture2D(size, size);
        squareTexture.SetPixels(pixels);
        squareTexture.Apply();

        return squareTexture;
    }

    public async void updateWallet(int coins, int xp)
    {
        PassData.BoardPrice = coins;
        PassData.betAmount = Math.Abs(coins);


        var payload = JsonWriter.ToJson(new { coins = coins, xp = xp });
        var rpcid = "Update_Wallet";
        await client.RpcAsync(session, rpcid, payload);

    }

    public async void UpdateXP(int xp)
    {
        var payload = JsonWriter.ToJson(new { xp = xp });
        var rpcid = "Update_XP";
        await PassData.iClient.RpcAsync(PassData.isession, rpcid, payload);

    }

    public async void WriteWinsAndLosses(int levelValue, int winsvalue, int lossesValue)
    {

        var Datas = new PlayerDataObj
        {
            Losses = lossesValue.ToString(),
            wins = winsvalue.ToString(),
            Level = levelValue.ToString(),
            Queue = "none",
            BoardType = "paris"
        };

        var Sendata = await client.WriteStorageObjectsAsync(session, new[] {
        new WriteStorageObject
  {
      Collection = "UserData",
      Key = "Data",
      Value = JsonWriter.ToJson(Datas),
      Version = PassData.version


  }
});
        Debug.Log("Version " + PassData.version);
        Debug.Log("wins " + winsvalue);
        Debug.Log("loss " + lossesValue);

        AddLeaderboard(PassData.wins + PassData.ChessWins);

    }

    public async void AddLeaderboard(long wins)
    {
        const string leaderboardId = "level1";
        long score = wins;
        var r = await client.WriteLeaderboardRecordAsync(session, leaderboardId, score);
        System.Console.WriteLine("New record for '{0}' score '{1}'", r.Username, r.Score);

    }

    private void Update()
    {
        ConvertNumberToPrefix(PassData.betAmount, BetAmountEnd);
    }

    void ConvertNumberToPrefix(float number, Text text)
    {
        string prefix = "";
        float absNumber = Mathf.Abs(number);
        if (absNumber >= 1000000000)
        {
            number /= 1000000000;
            prefix = "B";
        }
        else if (absNumber >= 1000000)
        {
            number /= 1000000;
            prefix = "M";
        }
        else if (absNumber >= 1000)
        {
            number /= 1000;
            prefix = "K";
        }



        text.text = number.ToString("0.##") + prefix;
    }

    public void AddFriend()
    {
        AddFriendButton.image.sprite = FriendAddedSprite;
        AddFriendButton.interactable = false;


    }

    //async void readMatchList()
    //{
    //    var result1 = await UserProfile.instance.client.ListUsersStorageObjectsAsync(UserProfile.instance.session, "MatchList", "00000000-0000-0000-0000-000000000000", 1);
    //    if (result1.Objects.Any())
    //    {
    //        foreach (var ob in result1.Objects)
    //        {
    //            var datas = JsonParser.FromJson<MatchCount>(ob.Value);
    //            matchList = int.Parse(datas.matchCount);


    //        }
    //    }
    //}

    private void OnApplicationPause(bool pause)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (pause)
            {
                if(matchList != 0)
                {
                matchList--;
               // listMatches.Instance.InsertIntoMatchList(matchList.ToString());
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
              // listMatches.Instance.InsertIntoMatchList(matchList.ToString());
            }


        }

    }
}
