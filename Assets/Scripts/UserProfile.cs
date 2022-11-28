using System.Xml.Serialization;
using System.Globalization;
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

    string QueueUser = "true";
    string Boardtype = "oslo";




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



    [SerializeField] Text parisText;
    [SerializeField] Text osloText;
    [SerializeField] Text romaText;
    [SerializeField] Text dubaiText;
    [SerializeField] Text moscoText;
    [SerializeField] Text berlinText;
    [SerializeField] Text newyorkText;
    [SerializeField] Text londonText;
    [SerializeField] Text tokoyoText;
    [SerializeField] Text bostonText;

    int counterparis = 0;
    int counteroslo = 0;
    int counterroma = 0;
    int counterberlin = 0;
    int countermosco = 0;
    int counternewyork = 0;
    int counterboston = 0;
    int counterdubai = 0;
    int counterlondon = 0;
    int countertokoyo = 0;


    bool ChekckedReadData = true;




    // Start is called before the first frame update
    void Start()
    {
        client = PassData.iClient;
        session = PassData.isession;

        StartCoroutine(GetTexture());



        getUserProfile();



        Wallet();

        rpc();


        Storageojectcounter();
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


        var result = await client.GetUsersAsync(session, termsList);

        List<bool> onlines = new List<bool>();

        foreach (var userId in result.Users)
        {

 

            if (userId.Online)
            {
                onlines.Add(userId.Online);
            }

        }

        OnlineCounterText.text = onlines.Count.ToString();

    }


    public async void WriteData(int levelvalue, int winsvalue, int LossValue, string wating, string BoardType)
    {

        var Datas = new PlayerDataObj
        {
            Level = levelvalue.ToString(),
            wins = winsvalue.ToString(),
            Losses = LossValue.ToString(),
            Queue = wating.ToString(),
            BoardType = BoardType.ToString(),


        };





        var Sendata = await client.WriteStorageObjectsAsync(session, new[] {
        new WriteStorageObject
  {
      Collection = "UserData",
      Key = "Data",
      Value = JsonWriter.ToJson(Datas),
      Version = PassData.version,
      PermissionRead=2,



    }
}); 


  

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
            QueueUser = datas.Queue;
            Boardtype = datas.BoardType;



            PassData.ReadPermission = storageObject.PermissionRead;
            PassData.wins = int.Parse(datas.wins);
            PassData.losses = int.Parse(datas.Losses);
            PassData.level = int.Parse(datas.Level);
            PassData.Queue = datas.Queue;
            PassData.Board = datas.BoardType;
 

            PlayerPrefs.SetInt("level", sendLevel);
            PlayerPrefs.SetInt("wins", wins);
            PlayerPrefs.SetInt("looses", losses);

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

        if (walletMoney >= Math.Abs(coins))
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
       // LossText.text = losses.ToString();


        if (Application.internetReachability == NetworkReachability.NotReachable)
        {

            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                Debug.Log("user reconnected");
            }

        }
        else
        {
            if(ChekckedReadData == true)
            {
                ReadData();
                ChekckedReadData = false;

            }


            //    ReadDataStatus();



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



            Debug.Log(u.Id + " " + u.Online);
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
 
    public async void Storageojectcounter()
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


        var result = await client.GetUsersAsync(session, termsList);

        foreach (var userId in result.Users)
        {

            const int limit = 10; // default is 10.
            var result1 = await client.ListUsersStorageObjectsAsync(session, "UserData", userId.Id, limit);

            foreach (var storage in result1.Objects)
            {
 
                if (storage.Value.Contains("true") && storage.Value.Contains("oslo"))
                {
                    counteroslo++;
                    Debug.Log(storage);


                    osloText.text = "Waiting Player  " + " " + counteroslo;
 
                }

                if (storage.Value.Contains("true") && storage.Value.Contains("paris"))
                {
                    counterparis++;
 
                    parisText.text = "Waiting Player  " + " " + counterparis;
 

                }

                if (storage.Value.Contains("true") && storage.Value.Contains("mosco"))
                {
                    countermosco++;

                    moscoText.text = "Waiting Player  " + " " + countermosco;
 
                }
                if (storage.Value.Contains("true") && storage.Value.Contains("tokoyo"))
                {
                    countertokoyo++;

                    tokoyoText.text = "Waiting Player  " + " " + countertokoyo;
 
                }
                if (storage.Value.Contains("true") && storage.Value.Contains("berlin"))
                {
                    counterberlin++;

                    berlinText.text = "Waiting Player  " + " " + counterberlin;
 
                }
                if (storage.Value.Contains("true") && storage.Value.Contains("newyork"))
                {
                    counternewyork++;

                    newyorkText.text = "Waiting  Player  " + " " + counternewyork;
 
                }
                if (storage.Value.Contains("true") && storage.Value.Contains("london"))
                {
                    counterlondon++;

                    londonText.text=" Waiting Player  "+" " +counterlondon;
 
                }
                if (storage.Value.Contains("true") && storage.Value.Contains("boston"))
                {
                    counterboston++;

                    bostonText.text = "Waiting Player  " + " " + counterboston;
 
                }
                if (storage.Value.Contains("true") && storage.Value.Contains("dubai"))
                {
                    counterdubai++;

                    dubaiText.text = "Waiting Player  " + " " + counterdubai;
 
                }
                if (storage.Value.Contains("true") && storage.Value.Contains("roma"))
                {
                    counterroma++;

                    romaText.text = " Waiting Player  " + " " + counterroma;
 
                }
 



            }
            Console.WriteLine("List objects: {0}", result);
        }





    }


}






