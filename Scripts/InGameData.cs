using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Nakama;
using Nakama.TinyJson;
using System.Linq;
using UnityEngine.Networking;
using System;

public class InGameData : MonoBehaviour
{

    [SerializeField] Text LocalUsername;
    [SerializeField] Text RemoteUsername;
    [SerializeField] Text DoubleUsername;
    [SerializeField] Text AcceptUsername;
    [SerializeField] Text LevelText;
    [SerializeField] GameManager gameManager;
    [SerializeField] RawImage MyAvatar;
    [SerializeField] RawImage OponentAvatar;
    [SerializeField] Text RewardAmount;
    [SerializeField] Text MyUsername;
    [SerializeField] Text OpponentUsername;

    IClient client;
    ISession session;
    

    // Start is called before the first frame update
    void Start()
    {
        client = PassData.iClient;
        session = PassData.isession;
        LocalUsername.text = PassData.Match.Self.Username;
        RemoteUsername.text = PassData.otherUsername;
        DoubleUsername.text = PassData.otherUsername;
        AcceptUsername.text = PassData.otherUsername;

        MyUsername.text = PassData.Match.Self.Username;
        OpponentUsername.text = PassData.otherUsername;

        StartCoroutine(GetTexture(PassData.MyURL , MyAvatar));
        StartCoroutine(GetTexture(PassData.OpponentURL, OponentAvatar));


        ReadData();

    }

    public void Update()
    {
        RewardAmount.text = PassData.betAmount.ToString();   
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
            LevelText.text = datas.Level;
            var state = MatchDataJson.SetLevel(datas.Level);
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

            avatar.texture = myTexture;
        }

    }

 

    public async void updateWallet(int coins)
    {
        PassData.BoardPrice = coins;
        PassData.betAmount = Math.Abs(coins);

  
            var payload = JsonWriter.ToJson(new { coins = coins });
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


    public async void WriteWinsAndLosses(int levelValue ,int winsvalue , int lossesValue )
    {

        var Datas = new PlayerDataObj
        {
            Losses = lossesValue.ToString(), 
            wins = winsvalue.ToString(),
            Level = levelValue.ToString(),
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

        AddLeaderboard(winsvalue);

    }

    public async void AddFriend()
    {
        var id = new[] { PassData.OtherUserId };
        await PassData.iClient.AddFriendsAsync(PassData.isession, id);
        Debug.Log(" you added " + PassData.OtherUserId);

    }

    public async void AddLeaderboard(long wins)
    {
        const string leaderboardId = "level1";
        long score = wins;
        var r = await client.WriteLeaderboardRecordAsync(session, leaderboardId, score);
        System.Console.WriteLine("New record for '{0}' score '{1}'", r.Username, r.Score);

    }

}
