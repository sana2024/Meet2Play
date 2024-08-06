using System.Xml.Serialization;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Nakama;
using Nakama.TinyJson;
using System.Linq;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using ByteBrewSDK;

public class UserProfile : MonoBehaviour
{
    public static UserProfile instance;

   public IClient client;
   public ISession session;
   public ISocket isocket;



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
    [SerializeField] TextMeshProUGUI LevelText;
    [SerializeField] TextMeshProUGUI TimeText;
    [SerializeField] TextMeshProUGUI Username;
    [SerializeField] RawImage ProfileImage;
    [SerializeField] RawImage ProfilePanel;
    [SerializeField] RawImage MatchImage;
    [SerializeField] GameObject UserPanel;
    [SerializeField] Text CoinUserPanelText;
    [SerializeField] Text CoinText;
    [SerializeField] TextMeshProUGUI WinText;
    [SerializeField] TextMeshProUGUI LossText;
    [SerializeField] ArabicText MatchUserName;
    [SerializeField] TextMeshProUGUI UsernameTMPRO;
    [SerializeField] GameObject AppleSettingsPanel;
    [SerializeField] GameObject OnlineUserPrefab;
    [SerializeField] Transform OnlineUserList;
 

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
    string useridNewYork = "";
    string useridTokyo = "";
    string useridBoston = "";
    string useridDubai = "";
    string useridRoma = "";
    string useridBerlin = "";
 
 

    public void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    async void Start()
    {

        client = PassData.iClient;
        session = PassData.isession;
        isocket = PassData.isocket;
        await PassData.isocket.ConnectAsync(session, true);

        //Debug.Log("texture "+PassData.MyURLTexture);
        // ProfileImage.texture = PassData.MyURLTexture;
        // ProfilePanel.texture = PassData.MyURLTexture; ;
        // MatchImage.texture = PassData.MyURLTexture; 
        StartCoroutine(GetTexture());



        getUserProfile();
        Wallet();

      //  InvokeRepeating("Storageojectcounter", 0.1f, 5);

        // PassData.isocket.ReceivedStreamPresence += presenceEvent => JoinedStream(presenceEvent);


        var result = await PassData.isocket.RpcAsync("join", "payload");



    }
 
    public async void SendNotificationWaiting(string UserID, string board)
    {
        var payload = JsonWriter.ToJson(new { userid = UserID , Board = board});
        await client.RpcAsync(session, "one_signal_wating" , payload);
       // Debug.Log("rcp runned ");
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

    public async void InsertIntoWaiting(string UserID)
    {
        var payload = JsonWriter.ToJson(new { userid = UserID });
        await client.RpcAsync(session, "writedata", payload);
       // Debug.Log("payload " + payload);
    }

 
    public async void writeGlobalData(string UserId)
    {

        var Data = new GlobalUserData
        {
            UserID = UserId
        };

        await client.WriteStorageObjectsAsync(session, new[] {
        new WriteStorageObject
  {
      Collection = "MachList",
      Key = "Data",
      Value = JsonWriter.ToJson(Data),
      Version = PassData.version,
      PermissionRead=2,
    }
});


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



            WinText.text =  datas.wins;
            LossText.text = datas.Losses;
            PlayerPrefs.SetInt("level", sendLevel);
            PlayerPrefs.SetInt("wins", wins);
            PlayerPrefs.SetInt("looses", losses);

            WriteData(PassData.level, PassData.wins, PassData.losses, "none", "paris");



        }
        else
        {
            PlayerPrefs.SetInt("level", sendLevel);
            PassData.level = 1;
            WriteData(1, wins, losses, "none", "paris");



        }



    }

    public async void Wallet()
    {



        var account = await client.GetAccountAsync(session);
        var wallet = JsonParser.FromJson<Dictionary<string, int>>(account.Wallet);


        ConvertNumberToPrefix(wallet.Values.Last() , CoinText);
        ConvertNumberToPrefix(wallet.Values.Last() ,CoinUserPanelText);
        walletMoney = wallet.Values.Last();
        PassData.WalletMoney = wallet.Values.Last();

 
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

    public void AddXP(int XP)
    {
        PassData.AddedXP = XP;
    }

    public async void updateWallet(int coins)
    {
        PassData.BoardPrice = coins;
        PassData.betAmount = Math.Abs(coins);
       // Debug.Log("bet amount " + PassData.betAmount);
        PassData.AbsuluteBoardPrice = Math.Abs(coins);

        if (walletMoney >= Math.Abs(coins))
        {
            var payload = JsonWriter.ToJson(new { coins = coins , xp = PassData.AddedXP});
            var rpcid = "Update_Wallet";
            var WalletRPC = await client.RpcAsync(session, rpcid, payload);
 
            Wallet();
            ChessUserDatas.Instance.UpdateXPAsync(PassData.AddedXP);
        }



    }

    public async void BonusWallet(int coins)
    {
        PassData.BoardPrice = coins;

        var payload = JsonWriter.ToJson(new { coins = coins , xp = PassData.AddedXP});
        var rpcid = "Update_Wallet";
        var WalletRPC = await client.RpcAsync(session, rpcid, payload);
        Wallet();
        ChessUserDatas.Instance.UpdateXPAsync(PassData.AddedXP);
       // Debug.Log("Bonus wallet ");

    }

    public void Update()
    {

        //LevelText.text = sendLevel.ToString();
        //WinText.text = wins.ToString();
        // LossText.text = losses.ToString();


        if (Application.internetReachability == NetworkReachability.NotReachable)
        {

            if (Application.internetReachability != NetworkReachability.NotReachable)
            {

            }

        }
        else
        {

            if (ChekckedReadData == true)
            {
                ReadData();
                ChekckedReadData = false;

            }


            //    ReadDataStatus();



        }



    }


    public void getUserProfile()
    {

        //time created
        string date = PassData.DateAndTime;
        TimeText.text = date;

        //Username
        Username.text = PassData.Username;
        MatchUserName.Text = PassData.Username;
        UsernameTMPRO.text = PassData.Username;
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
        }
        else
        {

            Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;

            ProfileImage.texture = CropTexture(myTexture);
            ProfilePanel.texture = CropTexture(myTexture);
            MatchImage.texture = CropTexture(myTexture);


        }

      www.Dispose();

    }

    Texture2D CropTexture(Texture2D originalTexture)
    {
        int size = Mathf.Min(originalTexture.width, originalTexture.height);
        int offsetX = (originalTexture.width - size) / 2;
        int offsetY = (originalTexture.height - size) / 2;

        Color[] pixels = originalTexture.GetPixels(offsetX, offsetY, size, size);

        Texture2D squareTexture = new Texture2D(size, size);
        squareTexture.SetPixels(pixels);
        squareTexture.Apply(true, makeNoLongerReadable: true);
        return squareTexture;
    }



    //void ReduceTextureQuality(Texture2D texture, int maxSize)
    //{
    //    // Resize the texture to reduce its size
    //    Texture2D resizedTexture = ResizeTexture(texture, maxSize);

    //    // Assign the resized texture back to the target texture
    //    ProfileImage.texture = resizedTexture;
    //    ProfilePanel.texture = resizedTexture;
    //    MatchImage.texture = resizedTexture;
    //}

    //Texture2D ResizeTexture(Texture2D texture, int maxSize)
    //{
    //    // Create a new texture with reduced size
    //    Texture2D resizedTexture = new Texture2D(maxSize, maxSize, texture.format, false);

    //    // Set the filter mode and wrap mode
    //    resizedTexture.filterMode = texture.filterMode;
    //    resizedTexture.wrapMode = texture.wrapMode;

    //    // Copy the pixels from the original texture to the resized texture
    //    Graphics.CopyTexture(texture, resizedTexture);

    //    return resizedTexture;
    //}



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

    public async void DeleteAppleAccount()
    {
        PlayerPrefs.DeleteKey("progress");
        PlayerPrefs.SetString("login", "");
        await client.RpcAsync(session, "delete");
        SceneManager.LoadScene("Login");
    }

    public async void AccountDeletation()
    {
       // Debug.Log(PlayerPrefs.GetString("login"));
        if (PlayerPrefs.GetString("login") == "Apple")
        {
            
            AppleSettingsPanel.SetActive(true);
        }
        else
        {

        PlayerPrefs.DeleteKey("progress");
        PlayerPrefs.SetString("login", "");
        await client.RpcAsync(session, "delete");
        SceneManager.LoadScene("Login");
        }
        ByteBrew.NewCustomEvent("DeleteAccount", "Username=" + PassData.isession.Username + ";");

    }



 
}






