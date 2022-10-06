using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Nakama;
using Nakama.TinyJson;
using System.Linq;
using System;

public class UserProfile : MonoBehaviour
{
    public static UserProfile instance;

    IClient client;
    ISession session;
    ISocket isocket;



    //USER DATAS
 
    public int sendLevel = 1;
    int wins = 0;
    int losses = 0;
    int walletMoney;
    int BoardPrice;
    int OnlineCounter;

 


    //USER TEXTFRIENDS
    [SerializeField] Text LevelText;
    [SerializeField] Text TimeText;
    [SerializeField] Text Username;
    [SerializeField] RawImage ProfileImage;
    [SerializeField] RawImage ProfilePanel;
    [SerializeField] RawImage MatchImage;
    [SerializeField] GameObject UserPanel;
    [SerializeField] Text CoinUserPanelText;
    [SerializeField] Text CoinText;
    [SerializeField] Text WinText;
    [SerializeField] Text LossText;
    [SerializeField] Text OnlineCounterText;
    [SerializeField] Text MatchUserName;
 
    PersonData data;


 

    // Start is called before the first frame update
    void Start()
    {
        client = PassData.iClient;
        session = PassData.isession;

        StartCoroutine(GetTexture());

 

        getUserProfile();

 
       
          Wallet();

          rpc();

    }

    public async void rpc()
    {
        var rpcid = "users";
        var pokemonInfo = await client.RpcAsync(session, rpcid);

        string TrimedJson = pokemonInfo.Payload.Remove(11, 1);
 
        data = JsonUtility.FromJson<PersonData>(TrimedJson);

        List<String> termsList = new List<String>();


        foreach (var id in data.client)
        {

            termsList.Add(id.id);

        }

 
       var result = await client.GetUsersAsync(session , termsList);

        List<bool> onlines = new List<bool>();

        foreach ( var userId in result.Users)
        {
            if (userId.Online)
            {
               onlines.Add(userId.Online);
            }

        }

        OnlineCounterText.text = onlines.Count.ToString();

    }


    public async void WriteData(int levelvalue, int winsvalue , int LossValue)
    {

        var Datas = new PlayerDataObj
        {
            Level = levelvalue.ToString(),
            wins = winsvalue.ToString(),
            Losses = LossValue.ToString()
        };

        var Sendata = await client.WriteStorageObjectsAsync(session, new[] {
        new WriteStorageObject
  {
      Collection = "UserData",
      Key = "Data",
      Value = JsonWriter.ToJson(Datas),
      Version = PassData.version



    }
}); ;
 

        Debug.Log(string.Join(",\n  ", Sendata));

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
 

           sendLevel = int.Parse(datas.Level);
           wins = int.Parse(datas.wins);
           losses = int.Parse(datas.Losses);

            PassData.version = storageObject.Version;

            PassData.wins = int.Parse(datas.wins);
            PassData.losses = int.Parse(datas.Losses);
            PassData.level = int.Parse(datas.Level);

        }
        else
        {
 
            WriteData(sendLevel, wins, losses);
            


        }


    }

    public async void Wallet()
    {

 

        var account = await client.GetAccountAsync(session);
        var wallet = JsonParser.FromJson<Dictionary<string, int>>(account.Wallet);

        foreach (var currency in wallet.Values)
        {
            CoinText.text = currency.ToString();
            CoinUserPanelText.text = currency.ToString();
            walletMoney = currency;
            PassData.WalletMoney = currency;
           

        }
    }

    public async void updateWallet(int coins)
    {
        PassData.BoardPrice = coins;
        PassData.betAmount = Math.Abs(coins);

        if(walletMoney >= Math.Abs(coins))
        {
        var payload = JsonWriter.ToJson(new { coins = coins });
        var rpcid = "Update_Wallet";
        var WalletRPC = await client.RpcAsync(session, rpcid, payload);
        Wallet();
        }   

    }

    public async void BonusWallet(int coins)
    {
        PassData.BoardPrice = coins;
 
            var payload = JsonWriter.ToJson(new { coins = coins });
            var rpcid = "Update_Wallet";
            var WalletRPC = await client.RpcAsync(session, rpcid, payload);
            Wallet();
 
    }







    public void Update()
    {

        LevelText.text = sendLevel.ToString();
        WinText.text = wins.ToString();
        LossText.text = losses.ToString();


        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
 
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                Debug.Log("user reconnected");
            }

        }
        else
        {
 
               ReadData();

    

        }



 
    }


    public async void getUserProfile()
    {
        var account = await client.GetAccountAsync(session);
        var user = account.User;

        var ids = new[] { "071bb808-118f-40e6-a1ea-744584c69c91", "09121f55-5c50-4b94-9698-dd42e5bd4f32" };
        var result = await client.GetUsersAsync(session, ids);

       

        foreach (var u in result.Users)
        {
            
            Debug.Log(u.Id +" "+u.Online);
            System.Console.WriteLine("User id '{0}' username '{1}'", u.Id, u.Online);

          
        }


        //time created
        string date = user.CreateTime.ToString().Substring(0, 10);
        TimeText.text = date;

        //Username
        Username.text = user.Username;
        MatchUserName.text = user.Username;
    }

    public void OpenProfilePanel()
    {
        UserPanel.SetActive(true);
    }

    public void CloseProfilePanel()
    {
        UserPanel.SetActive(false);
    }

 

 

        IEnumerator GetTexture()
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(PassData.MyURL);
 
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;

                ProfileImage.texture = myTexture;
                ProfilePanel.texture = myTexture;
                MatchImage.texture = myTexture;
            }

        }

    [System.Serializable]
    public class Person
    {
        public string id;
   
        public Person(string _id)
        {
            id = _id;
 
        }
    }

    [System.Serializable]
    public class PersonData
    {
        public List<Person> client;
    }


}


