using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Nakama;
using Nakama.TinyJson;
using System.Linq;
using UnityEngine.Networking;
using System;
using ByteBrewSDK;

public class InGameData : MonoBehaviour
{

    [SerializeField] ArabicText LocalUsername;
    [SerializeField] ArabicText RemoteUsername;
    [SerializeField] ArabicText DoubleUsername;
    [SerializeField] ArabicText AcceptUsername;
    [SerializeField] Text LevelText;
    [SerializeField] GameManager gameManager;
    [SerializeField] RawImage MyAvatar;
    [SerializeField] RawImage OponentAvatar;
    [SerializeField] Text RewardAmount; 
    [SerializeField] ArabicText MyUsername;
    [SerializeField] ArabicText OpponentUsername;
    [SerializeField] Button AddFriendButton;
    [SerializeField] Sprite FriendAddedSprite;
    [SerializeField] GameObject LeavePanel;

    IClient client;
    ISession session;

    public static InGameData Instance;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }


    // Start is called before the first frame update
    async void Start()
    {
        PassData.isession = await PassData.iClient.SessionRefreshAsync(PassData.isession);
        client = PassData.iClient;
        session = PassData.isession;
        LocalUsername.Text = PassData.isession.Username;
        RemoteUsername.Text = PassData.otherUsername;
        DoubleUsername.Text = PassData.otherUsername;
        AcceptUsername.Text = PassData.otherUsername;

        MyUsername.Text = PassData.isession.Username;
        OpponentUsername.Text = PassData.otherUsername;

        StartCoroutine(GetTexture(PassData.MyURL , MyAvatar));
        StartCoroutine(GetTexture(PassData.OpponentURL, OponentAvatar));



        ReadData();

    }

    public void Update()
    {
        RewardAmount.text = PassData.betAmount.ToString();   
    }

    public void OpenLeavePanel()
    {
        LeavePanel.SetActive(true);
    }

    public void CloseLeavePanel()
    {
        LeavePanel.SetActive(false);
    }

    public async void ReadData()
    {
        var result = await client.ReadStorageObjectsAsync(session, new[] {
        new StorageObjectId {
        Collection = "UserData",
        Key = "Data",
        UserId = session.UserId
  }
});
        if (result.Objects.Any())
        {
            var storageObject = result.Objects.First();
            var datas = JsonParser.FromJson<PlayerDataObj>(storageObject.Value);
            LevelText.text = PassData.ChessLevel.ToString();
            var state = MatchDataJson.SetLevel(PassData.level.ToString());
          //  Debug.Log("level " + PassData.level.ToString());
            gameManager.SendMatchState(OpCodes.Player_Level, state);

        }

    }


    IEnumerator GetTexture(string URL , RawImage avatar)
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


    public async void updateWallet(int coins , int xp)
    {
        PassData.BoardPrice = coins;
        PassData.betAmount = Math.Abs(coins);

  
            var payload = JsonWriter.ToJson(new { coins = coins , xp = xp });
            var rpcid = "Update_Wallet";
            var WalletRPC = await client.RpcAsync(session, rpcid, payload);
     
    }

    public async void BonusWallet(int coins)
    {
        PassData.BoardPrice = coins;

        var payload = JsonWriter.ToJson(new { coins = coins });
        var rpcid = "Update_Wallet";
        var WalletRPC = await client.RpcAsync(session, rpcid, payload);
      

    }


    public async void UpdateXP(int xp)
    {
        var payload = JsonWriter.ToJson(new { xp = xp });
        var rpcid = "Update_XP";
        var WalletRPC = await PassData.iClient.RpcAsync(PassData.isession, rpcid, payload);

    }

    public async void WriteWinsAndLosses(int levelValue ,int winsvalue , int lossesValue )
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
 
        AddLeaderboard(PassData.wins + PassData.ChessWins);

    }

    public async void AddFriend()
    {
        AddFriendButton.image.sprite = FriendAddedSprite;
        AddFriendButton.interactable = false;
        var id = new[] { PassData.OtherUserId };
        await PassData.iClient.AddFriendsAsync(PassData.isession, id);
        PlayerList.instance.SendPushNotificationFriend(PassData.OtherUserId, PassData.isession.Username);
      //  Debug.Log(" you added " + PassData.OtherUserId);

    }

    public async void AddLeaderboard(long wins)
    {
        const string leaderboardId = "level1";
        long score = wins;
        var r = await client.WriteLeaderboardRecordAsync(session, leaderboardId, score);
        System.Console.WriteLine("New record for '{0}' score '{1}'", r.Username, r.Score);

    }

    private void OnApplicationPause(bool pause)
    {
        ByteBrew.NewCustomEvent("ClosedApp", "Username=" + PassData.isession.Username + ";");
    }



}
