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
    [SerializeField] ArabicText Username;
    [SerializeField] RawImage ProfileImage;
    [SerializeField] RawImage ProfilePanel;
    [SerializeField] RawImage MatchImage;
    [SerializeField] GameObject UserPanel;
    [SerializeField] Text CoinUserPanelText;
    [SerializeField] Text CoinText;
    [SerializeField] Text WinText;
    [SerializeField] Text LossText;
    [SerializeField] Text OnlineCounterText;
    [SerializeField] ArabicText MatchUserName;

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

    string useridParis = "";
    string useridLondon = "";
    string useridOslo = "";
    string useridMoscow = "";
    string useridNewYork= "";
    string useridTokyo = "";
    string useridBoston = "";
    string useridDubai = "";
    string useridRoma = "";
    string useridBerlin = "";

     List<string> OnlineUsers = new List<string>();


    // Start is called before the first frame update
    async void Start()
    {
        client = PassData.iClient;
        session = PassData.isession;

        StartCoroutine(GetTexture());



        getUserProfile();



        Wallet();

        InvokeRepeating("Storageojectcounter", 0.1f, 5);

       // PassData.isocket.ReceivedStreamPresence += presenceEvent => JoinedStream(presenceEvent);

        var result = await PassData.isocket.RpcAsync("join","payload");
 

       InvokeRepeating("Onlines", 0.1f, 2);

 
    }

  
   async void Onlines()
    {
        var payload_online = JsonWriter.ToJson(new { Payload = "OnlineStatus" });
        var rpcid_online = "onlineusers";
        var OnlineUsersRPC_online = await client.RpcAsync(session, rpcid_online, payload_online);
        OnlineCounterText.text = OnlineUsersRPC_online.Payload;
 
    }

    /*

    void JoinedStream(IStreamPresenceEvent presenceEvent)
    {

        foreach (var joined in presenceEvent.Joins)
        {
            OnlineCounter++;

            OnlineUsers.Add(joined.UserId);
            Storageojectcounter();

        }
        foreach (var left in presenceEvent.Leaves)
        {
            OnlineUsers.Remove(left.UserId);
            OnlineCounter--;
        }

 
    }

    */

    public async void rpc()
    {
        var payload_List = JsonWriter.ToJson(new { Payload = "UserList" });
        var rpcid_list = "list_stream_users";
        var OnlineUsersRPC_list = await client.RpcAsync(session, rpcid_list, payload_List);
 
        string TrimedJson = OnlineUsersRPC_list.Payload.Remove(11, 1);
 
        data = JsonUtility.FromJson<PersonData>(TrimedJson);
 
        foreach (var id in data.client)
        {
   
                OnlineUsers.Add(id.id);
            

        }

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
            PassData.level = sendLevel;
            PassData.Queue = datas.Queue;
            PassData.Board = datas.BoardType;
 

            PlayerPrefs.SetInt("level", sendLevel);
            PlayerPrefs.SetInt("wins", wins);
            PlayerPrefs.SetInt("looses", losses);



            Debug.Log(PlayerPrefs.GetInt("level"));

        }
        else
        {
            PlayerPrefs.SetInt("level", sendLevel);
            WriteData(sendLevel, wins, losses, "false", "paris");



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
        Username.Text = user.Username;
        MatchUserName.Text = user.Username;
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
        rpc();
        if (OnlineUsers.Count != 0)
        {
            foreach (var userId in OnlineUsers.ToList())
            {

                const int limit = 10; // default is 10.
                var result1 = await client.ListUsersStorageObjectsAsync(session, "UserData", userId, limit);

                foreach (var storage in result1.Objects)
                {


                    //OSLO
                    if (storage.Value.Contains("oslo"))
                    {
                        if (storage.Value.Contains("true"))
                        {
                            useridOslo = storage.UserId;
                            counteroslo = 1;


                        }

                        if (storage.Value.Contains("false"))
                        {

                            if (storage.UserId == useridOslo)
                            {
                                counteroslo = 0;
                            }

                        }



                        osloText.text = "Waiting Player  " + " " + counteroslo;
                    }

                    //PARIS
                    if (storage.Value.Contains("paris"))
                    {


                        if (storage.Value.Contains("true"))
                        {
                            useridParis = storage.UserId;
                            counterparis = 1;


                        }

                        if (storage.Value.Contains("false"))
                        {

                            if (storage.UserId == useridParis)
                            {
                                counterparis = 0;
                            }

                        }

                        parisText.text = "Waiting Player  " + " " + counterparis;




                    }

                    //MOSCOW
                    if (storage.Value.Contains("mosco"))
                    {
                        if (storage.Value.Contains("true"))
                        {
                            useridMoscow = storage.UserId;
                            countermosco = 1;

                        }

                        if (storage.Value.Contains("false"))
                        {

                            if (storage.UserId == useridMoscow)
                            {
                                countermosco = 0;
                            }

                        }
                        moscoText.text = "Waiting Player  " + " " + countermosco;
                    }

                    //TOKYO
                    if (storage.Value.Contains("tokyo"))
                    {
                        if (storage.Value.Contains("true"))
                        {
                            useridTokyo = storage.UserId;
                            countertokoyo = 1;

                        }

                        if (storage.Value.Contains("false"))
                        {

                            if (storage.UserId == useridTokyo)
                            {
                                countertokoyo = 0;
                            }

                        }
                        tokoyoText.text = "Waiting Player  " + " " + countertokoyo;
                    }

                    //BERLIN
                    if (storage.Value.Contains("berlin"))
                    {
                        if (storage.Value.Contains("true"))
                        {
                            useridBerlin = storage.UserId;
                            counterberlin = 1;

                        }

                        if (storage.Value.Contains("false"))
                        {

                            if (storage.UserId == useridBerlin)
                            {
                                counterberlin = 0;
                            }

                        }
                        berlinText.text = "Waiting Player  " + " " + counterberlin;
                    }

                    //NEWYORK
                    if (storage.Value.Contains("newyork"))
                    {
                        if (storage.Value.Contains("true"))
                        {
                            useridNewYork = storage.UserId;
                            counternewyork = 1;


                        }

                        if (storage.Value.Contains("false"))
                        {

                            if (storage.UserId == useridNewYork)
                            {
                                counternewyork = 0;
                            }

                        }
                        newyorkText.text = "Waiting Player  " + " " + counternewyork;
                    }

                    //LONDON
                    if (storage.Value.Contains("london"))
                    {
                        if (storage.Value.Contains("true"))
                        {
                            useridLondon = storage.UserId;
                            counterlondon = 1;


                        }

                        if (storage.Value.Contains("false"))
                        {

                            if (storage.UserId == useridLondon)
                            {
                                counterlondon = 0;
                            }

                        }
                        londonText.text = "Waiting Player  " + " " + counterlondon;
                    }

                    //BOSTON
                    if (storage.Value.Contains("boston"))
                    {
                        if (storage.Value.Contains("true"))
                        {
                            useridBoston = storage.UserId;
                            counterboston = 1;

                        }

                        if (storage.Value.Contains("false"))
                        {

                            if (storage.UserId == useridBoston)
                            {
                                counterboston = 0;
                            }

                        }
                        bostonText.text = "Waiting Player  " + " " + counterboston;
                    }

                    //DUBAI
                    if (storage.Value.Contains("dubai"))
                    {
                        if (storage.Value.Contains("true"))
                        {
                            useridDubai = storage.UserId;
                            counterdubai = 1;

                        }

                        if (storage.Value.Contains("false"))
                        {

                            if (storage.UserId == useridDubai)
                            {
                                counterdubai = 0;
                            }

                        }
                        dubaiText.text = "Waiting Player  " + " " + counterdubai;
                    }

                    //ROMA
                    if (storage.Value.Contains("roma"))
                    {
                        if (storage.Value.Contains("true"))
                        {
                            useridRoma = storage.UserId;
                            counterroma = 1;

                        }

                        if (storage.Value.Contains("false"))
                        {

                            if (storage.UserId == useridRoma)
                            {
                                counterroma = 0;
                            }

                        }
                        romaText.text = "Waiting Player  " + " " + counterroma;
                    }




                }

            }





        }

    }
}






