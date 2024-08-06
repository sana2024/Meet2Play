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

public class OnlineUsers : MonoBehaviour
{
    [SerializeField] GameObject waitingUserPanel;
    [SerializeField] public GameObject OnlineUserPanel;
    [SerializeField] GameObject OnlineUserPrefab;
    [SerializeField] Transform OnlineUserHolderUi;
    [SerializeField] public Text OnlineCounterText;
    [SerializeField] GameObject Notificationpanel;
    [SerializeField] GameObject[] OtherPanels;
    [SerializeField] Transform WaitingUsersList;
    [SerializeField] Text WaitingDialog;
    [SerializeField] Button JoinGame;

    [SerializeField] Image waitingPanelImage;
    [SerializeField] Sprite chessSprite;
    [SerializeField] Sprite backgammonSprite;

    public static OnlineUsers instance;

    IClient iclient;
    ISession isession;
    ISocket isocket;

    public List<String> AvailableUsers = new List<String>();
    List<PrefabWithNumber> prefabList = new List<PrefabWithNumber>();

    //ArabicText OnlineUserText;
    //RawImage UserAvatar;
    //ArabicText WaitingUserNameText;
    //ArabicText GameTypeText;
    //RawImage WaitingUserAvatar;
    GameObject OnlineUser;
    //PersonData data;
    //GameObject WaitingPlayer;
    //public List<string> OnlineTransforms = new List<string>();
    //public List<Person> Onlines = new List<Person>();
    //public List<string> Waitings = new List<string>();
    //[SerializeField] Sprite FriendAdded;


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    async void Start()
    {

        InvokeRepeating("GetOnlineCount", 0.1f, 2);

        iclient = PassData.iClient;
        isession = PassData.isession;
        isocket = PassData.isocket;
        var mainThread = UnityMainThreadDispatcher.Instance();

        listAvailables();

        isocket.ReceivedStreamPresence += presenceEvent => mainThread.Enqueue(() => OnRecievedAvaialableUsers(presenceEvent));

        isocket.ReceivedStreamState += stream => mainThread.Enqueue(() => OnRecievedStreamMessageAsync(stream));



    }


    public async void LeaveAvailables()
    {

        var payload = JsonWriter.ToJson(new { data = "Beginner" });
        await PassData.isocket.RpcAsync("leave_availables", payload);

    }


    public async void joinAvaiables()
    {

        var payload = JsonWriter.ToJson(new { data = "Beginner" });
        await PassData.isocket.RpcAsync("available_users", payload);

    }

    public async void listAvailables()
    {
        var Beginner_payload = JsonWriter.ToJson(new { data = "Beginner" });
        var Beginner_response = await PassData.isocket.RpcAsync("list_availables", Beginner_payload);
        //Debug.Log("beginner response"+Beginner_response);
        if (Beginner_response.Payload != null)
        {

            string Trimed_beginner = Beginner_response.Payload.Remove(11, 1);
            PersonData beginner_data = JsonUtility.FromJson<PersonData>(Trimed_beginner);

            foreach (var online in beginner_data.client)
            {
                if (online.id != PassData.isession.UserId)
                {
                    AvailableUsers.Add(online.id);
                }

            }
        }
        //var intermediate_payload = JsonWriter.ToJson(new { data = "Intermediate" });
        //var intermediate_response = await PassData.isocket.RpcAsync("list_availables", intermediate_payload);
        ////Debug.Log("intermidiate response"+intermediate_response);
        //if (intermediate_response.Payload != null)
        //{
        //    string Trimed_intermediate = intermediate_response.Payload.Remove(11, 1);
        //    PersonData intermediate_data = JsonUtility.FromJson<PersonData>(Trimed_intermediate);

        //    foreach (var online in intermediate_data.client)
        //    {
        //        if (online.id != PassData.isession.UserId)
        //        {
        //            AvailableUsers.Add(online.id);
        //        }

        //    }
        //}
        //var advanced_payload = JsonWriter.ToJson(new { data = "Advanced" });
        //var advanced_response = await PassData.isocket.RpcAsync("list_availables", advanced_payload);
        ////Debug.Log("advanced response" +advanced_response);
        //if (advanced_response.Payload != null)
        //{
        //    string Trimed_advanced = advanced_response.Payload.Remove(11, 1);
        //    PersonData advanced_data = JsonUtility.FromJson<PersonData>(Trimed_advanced);

        //    foreach (var online in advanced_data.client)
        //    {
        //        if (online.id != PassData.isession.UserId)
        //        {
        //            AvailableUsers.Add(online.id);
        //        }

        //    }
        //}
    }

    public async void sendToAvaiables(string gameType , string time)
    {

        AvailablesMessage message = new AvailablesMessage();
        message.gameType = gameType;
        message.time = time;
        message.label = "Beginner";

        var payload = JsonWriter.ToJson(new { label = message.label , gameType = message.gameType, time = message.time });
        await isocket.RpcAsync("send_to_avaialables", payload);
    }



    private void OnRecievedAvaialableUsers(IStreamPresenceEvent presenceEvent)
    {


       // Debug.Log("Received data from stream: '{0}' " + presenceEvent.Stream);
        foreach (var joined in presenceEvent.Joins)
        {
            //Debug.Log("Joined: {0} "+ joined);

            if (joined.UserId != isession.UserId)
            {
            
                if (!AvailableUsers.Contains(joined.UserId))
                {
                    AvailableUsers.Add(joined.UserId);
                   
                }

            }

}
        foreach (var left in presenceEvent.Leaves)
        {
           // Debug.Log("Left: {0} " + left);
            foreach (Transform OnlineObject in OnlineUserHolderUi)
            {

                if (OnlineObject.name == left.UserId)
                {
                    Destroy(OnlineObject.gameObject);
                }
            }


            if (AvailableUsers.Contains(left.UserId))
            {
                AvailableUsers.Remove(left.UserId);
                
            }




        }
    }

    private async void OnRecievedStreamMessageAsync(IStreamState stream)
    {
        var message_content = JsonParser.FromJson<AvailablesMessage>(stream.State);
        if (message_content.subject != isession.UserId)
        {
            var usr = new[] { message_content.subject };
            var senders = await iclient.GetUsersAsync(UserProfile.instance.session, usr);
            ShowWaitingPlayerPopup(message_content.gameType, message_content.time, senders.Users.First().Username);

        }




}

    public void OpenOnlineUserPanel()
    {
        OnlineUserPanel.SetActive(true);
        Notificationpanel.SetActive(false);
        CreateAvaialableUserUI();

    }

    public async void CreateAvaialableUserUI()
    {
        foreach (Transform OnlineObject in OnlineUserHolderUi)
        {
            Destroy(OnlineObject.gameObject);
        }
        prefabList.Clear();
        var avaiable_users = await iclient.GetUsersAsync(UserProfile.instance.session, AvailableUsers);
        foreach(var avaiable in avaiable_users.Users)
        {
            OnlineUser = Instantiate(OnlineUserPrefab, OnlineUserHolderUi);
            OnlineUser.name = avaiable.Id;

           RawImage Avaiable_avatar = OnlineUser.GetComponentInChildren<RawImage>();
           ArabicText Avaiable_username = OnlineUser.GetComponentInChildren<ArabicText>();
           Text[] Avaialable_elo = OnlineUser.GetComponentsInChildren<Text>();
            Button[] ChallangeButton = OnlineUser.GetComponentsInChildren<Button>();
           Image[] AddImage = ChallangeButton[0].gameObject.GetComponentsInChildren<Image>();
           Avaiable_username.Text = avaiable.Username;

            StartCoroutine(FetchTexture.Instance.GetTexture(avaiable.AvatarUrl, Avaiable_avatar));

            FriendSystem.instance.CheckIfFriend(avaiable.Username, ChallangeButton[0], AddImage[1]);

            ChallangeButton[0].onClick.AddListener(() =>
            {
                PlayerList.instance.AddFriend(avaiable.Username);
                StartCoroutine(PlayerList.instance.SendButton(ChallangeButton[0]));
            });
            ChallangeButton[1].onClick.AddListener(() =>
            {
                AppFlyer.Instance.AppsFlyerEvent("Send_Challange_Availables_Backgammon", isession.Username);
                PlayerList.instance.SendChallangeRequest(avaiable.Id, avaiable.Username, avaiable.AvatarUrl, "backgammon");
            });

            ChallangeButton[2].onClick.AddListener(() =>
            {
                AppFlyer.Instance.AppsFlyerEvent("Send_Challange_Availables_Chess", isession.Username);
                AppFlyer.Instance.AppsFlyerEvent("FoundBackgammon", isession.Username);
                PlayerList.instance.SendChallangeRequest(avaiable.Id, avaiable.Username, avaiable.AvatarUrl, "chess");
            });

            const int limit = 100; // default is 10.
            var result = await iclient.ListUsersStorageObjectsAsync(isession, "ChessDatabase", avaiable.Id , limit);
            
            foreach(var ob in result.Objects)
            {
                if (result.Objects.Any())
                {
                    var datas = JsonParser.FromJson<ChessDataObj>(result.Objects.First().Value);
                   // Debug.Log("name "+avaiable.Username + " elo "+datas.ChessElo);
                    Avaialable_elo[1].text = datas.ChessElo;
                    prefabList.Add(new PrefabWithNumber { prefab = OnlineUser, number = int.Parse(datas.ChessElo) });
                }
                     
            }
        }

        prefabList.Sort((x, y) => Mathf.Abs(x.number - PassData.ChessELO).CompareTo(Mathf.Abs(y.number - PassData.ChessELO)));
        for (int i = 0; i < prefabList.Count; i++)
        {
            prefabList[i].prefab.transform.SetSiblingIndex(i);
        }

    }

    public void CloseOnlineUserPanel()
    {
        OnlineUserPanel.SetActive(false);
    }

    async void GetOnlineCount()
    {
        var payload_online = JsonWriter.ToJson(new { Payload = "OnlineStatus" });
        var rpcid_online = "onlineusers";
        var OnlineUsersRPC_online = await UserProfile.instance.client.RpcAsync(UserProfile.instance.session, rpcid_online, payload_online);
        int userCount = int.Parse(OnlineUsersRPC_online.Payload.ToString()) - 1;
        if(userCount < 0)
        {
            userCount = 0;
        }
        OnlineCounterText.text = userCount.ToString();
       // Storageojectcounter(userCount);

    }

    void ShowWaitingPlayerPopup(string gameType, string time, string senderName)
    {

                if (gameType == "chess")
                {

                    waitingPanelImage.sprite = chessSprite;
                    PlayerPrefs.SetInt("time", int.Parse(time));
                    SelectChessTime.Instance.AutomaticChange();
                    WaitingDialog.text = senderName+" is waiting for you to join them on a chess game";
                    JoinGame.onClick.RemoveAllListeners();
                    JoinGame.onClick.AddListener(delegate
                    {
                        Matchmaking.Instance.FindChessMatch(100);

                    });


                }

                if (gameType == "backagmmon")
                {

                    waitingPanelImage.sprite = backgammonSprite;
                    WaitingDialog.text = senderName+" is waiting for you to join them on a backgammon game";
                    JoinGame.onClick.RemoveAllListeners();
                    JoinGame.onClick.AddListener(delegate
                    {

                        Matchmaking.Instance.FindBackgammonMatch("paris");

                    });
                }


                PanelHandlers.instance.OpenWaitingsPanel();

         
    }

    //public async void GetOnlineUsers()
    //{

    //    foreach (Transform OnlineObject in OnlineUserHolderUi)
    //    {
    //        Destroy(OnlineObject.gameObject);
    //    }

    //    var payload_List = JsonWriter.ToJson(new { Payload = "UserList" });
    //    var rpcid_list = "list_stream_users";
    //    var OnlineUsersRPC_list = await UserProfile.instance.client.RpcAsync(UserProfile.instance.session, rpcid_list, payload_List);

    //    string TrimedJson = OnlineUsersRPC_list.Payload.Remove(11, 1);

    //    data = JsonUtility.FromJson<PersonData>(TrimedJson);


    //    foreach (var online in data.client)
    //    {
    //        if (online.id != UserProfile.instance.session.UserId)
    //        {

    //            var usr = new[] { online.id };
    //            var names = await UserProfile.instance.client.GetUsersAsync(UserProfile.instance.session, usr);

    //            foreach (var user in names.Users)
    //            {
    //                var result = await UserProfile.instance.client.ReadStorageObjectsAsync(UserProfile.instance.session, new[] {
    //                                        new StorageObjectId {
    //                                             Collection = "UserData",
    //                                             Key = "Data",
    //                                             UserId = user.Id
    //                                                            }
    //                                                         });
    //                if (result.Objects.Any())
    //                {
    //                    var datas = JsonParser.FromJson<PlayerDataObj>(result.Objects.First().Value);
    //                    if (!datas.Queue.Contains("ingame"))
    //                    {
    //                        OnlineUser = Instantiate(OnlineUserPrefab, OnlineUserHolderUi);
    //                        OnlineUser.name = user.Id;
    //                        UserAvatar = OnlineUser.GetComponentInChildren<RawImage>();
    //                        OnlineUserText = OnlineUser.GetComponentInChildren<ArabicText>();

    //                        OnlineUserText.Text = user.Username;

    //                        StartCoroutine(FetchTexture.Instance.GetTexture(user.AvatarUrl, UserAvatar));



    //                        Button[] ChallangeButton = OnlineUser.GetComponentsInChildren<Button>();
    //                        Image[] onlineImaeges = OnlineUser.GetComponentsInChildren<Image>();

    //                        ChallangeButton[0].onClick.AddListener(delegate
    //                        {

    //                            PlayerList.instance.AddFriend(user.Username);
    //                            StartCoroutine(PlayerList.instance.SendButton(ChallangeButton[0]));
    //                            onlineImaeges[3].sprite = FriendAdded;


    //                        });


    //                        ChallangeButton[1].onClick.AddListener(() =>
    //                        {
    //                            OnlineUserPanel.SetActive(false);
    //                            PlayerList.instance.SendChallangeRequest(user.Id, user.Username, user.AvatarUrl, "backgammon");
    //                        // playerList.SendPushNotification(f.User.Id, f.User.Username);
    //                    });
    //                        ChallangeButton[2].onClick.AddListener(() =>
    //                        {
    //                            OnlineUserPanel.SetActive(false);
    //                            PlayerList.instance.SendChallangeRequest(user.Id, user.Username, user.AvatarUrl, "chess");
    //                        // playerList.SendPushNotification(f.User.Id, f.User.Username);
    //                    });




    //                    }

                

    //        }

    //            }

    //        }
    //    }
        //Onlines = data.client;
        //foreach(var id in Onlines)
        //{
        //    Debug.Log(id);
        //}
        //Onlines.Clear();

        //foreach (Transform item in OnlineUserHolderUi)
        //{
            
 
        //}




        //foreach (Transform item in OnlineUserHolderUi)
        //{
        //    if (!OnlineTransforms.Contains(item.name))
        //    {
        //        OnlineTransforms.Add(item.name);
        //    }
        //}

   


        //foreach (var id in data.client)
        //{
        //    if (id.id != UserProfile.instance.session.UserId)
        //    {

        //        if (!Onlines.Contains(id.id))
        //        {
        //            Onlines.Add(id.id);
        //        }

        //        if (!OnlineTransforms.Contains(id.id))
        //        {


        //            var usr = new[] { id.id };
        //            var names = await UserProfile.instance.client.GetUsersAsync(UserProfile.instance.session, usr);

        //            foreach (var user in names.Users)
        //            {
        //                var result = await UserProfile.instance.client.ReadStorageObjectsAsync(UserProfile.instance.session, new[] {
        //                            new StorageObjectId {
        //                                 Collection = "UserData",
        //                                 Key = "Data",
        //                                 UserId = user.Id
        //                                                }
        //                                             });
        //                if (result.Objects.Any())
        //                {
        //                    var datas = JsonParser.FromJson<PlayerDataObj>(result.Objects.First().Value);
        //                   if(!datas.Queue.Contains("ingame"))
        //                    {
        //                        OnlineUser = Instantiate(OnlineUserPrefab, OnlineUserHolderUi);
        //                        OnlineUser.name = user.Id;
        //                        UserAvatar = OnlineUser.GetComponentInChildren<RawImage>();
        //                        OnlineUserText = OnlineUser.GetComponentInChildren<ArabicText>();

        //                        OnlineUserText.Text = user.Username;

        //                        StartCoroutine(FetchTexture.Instance.GetTexture(user.AvatarUrl, UserAvatar));



        //                        Button[] ChallangeButton = OnlineUser.GetComponentsInChildren<Button>();
        //                        Image[] onlineImaeges = OnlineUser.GetComponentsInChildren<Image>();

        //                        ChallangeButton[0].onClick.AddListener(delegate
        //                        {

        //                            PlayerList.instance.AddFriend(user.Username);
        //                            StartCoroutine(PlayerList.instance.SendButton(ChallangeButton[0]));
        //                            onlineImaeges[3].sprite = FriendAdded;


        //                        });


        //                        ChallangeButton[1].onClick.AddListener(() =>
        //                        {
        //                            OnlineUserPanel.SetActive(false);
        //                            PlayerList.instance.SendChallangeRequest(user.Id, user.Username, user.AvatarUrl, "backgammon");
        //                            // playerList.SendPushNotification(f.User.Id, f.User.Username);
        //                        });
        //                        ChallangeButton[2].onClick.AddListener(() =>
        //                        {
        //                            OnlineUserPanel.SetActive(false);
        //                            PlayerList.instance.SendChallangeRequest(user.Id, user.Username, user.AvatarUrl, "chess");
        //                            // playerList.SendPushNotification(f.User.Id, f.User.Username);
        //                        });

        //                    }


        //                }



        //            }


        //        }

        //    }
        //}

        //foreach(var item in OnlineTransforms.ToList())
        //{
        //    if (!Onlines.Contains(item))
        //    {

        //        OnlineTransforms.Remove(item);
        //        Destroy(GameObject.Find(item));


        //    }
        //    else
        //    {
        //        var result = await UserProfile.instance.client.ReadStorageObjectsAsync(UserProfile.instance.session, new[] {
        //                            new StorageObjectId {
        //                                 Collection = "UserData",
        //                                 Key = "Data",
        //                                 UserId = item
        //                                                }
        //                                             });
        //        if (result.Objects.Any())
        //        {
        //            var datas = JsonParser.FromJson<PlayerDataObj>(result.Objects.First().Value);
        //            if(datas.Queue.Contains("ingame"))
        //            {
        //                OnlineTransforms.Remove(item);
        //                Destroy(GameObject.Find(item));
        //            }
        //        }


        //        }
        //}

  //  }

 

 

    //public async void Storageojectcounter(int OnlineCounter)
    //{
    //    if(OnlineCounter > 0 &&  Matchmaking.Instance.SearchingPanel.activeSelf == false && ChallangePanel.activeSelf == false && FriendsPanel.activeSelf == false && CollectBonusPanel.activeSelf == false && ShopPanel.activeSelf == false && Notificationpanel.activeSelf == false)
    //    {

    //        var payload_List = JsonWriter.ToJson(new { Payload = "UserList" });
    //        var rpcid_list = "list_stream_users";
    //        var OnlineUsersRPC_list = await UserProfile.instance.client.RpcAsync(UserProfile.instance.session, rpcid_list, payload_List);

    //        string TrimedJson = OnlineUsersRPC_list.Payload.Remove(11, 1);

    //        data = JsonUtility.FromJson<PersonData>(TrimedJson);


    //        foreach (var online in data.client)
    //        {
    //            if (online.id != UserProfile.instance.session.UserId)
    //            {

    //                var usr = new[] { online.id };
    //                var names = await UserProfile.instance.client.GetUsersAsync(UserProfile.instance.session, usr);

    //                foreach (var user in names.Users)
    //                {
    //                    var result = await UserProfile.instance.client.ReadStorageObjectsAsync(UserProfile.instance.session, new[] {
    //                                        new StorageObjectId {
    //                                             Collection = "UserData",
    //                                             Key = "Data",
    //                                             UserId = user.Id
    //                                                            }
    //                                                         });
    //                    if (result.Objects.Any())
    //                    {
    //                        var datas = JsonParser.FromJson<PlayerDataObj>(result.Objects.First().Value);

    //                        var waitingUserInfo = new[] { user.Id };
    //                        var userInfo = await UserProfile.instance.client.GetUsersAsync(UserProfile.instance.session, waitingUserInfo);
    //                        if (datas.Queue == "backgammon" || datas.Queue.Contains("chess"))
    //                        {
    //                            if (!Waitings.Contains(user.Id))
    //                            {

    //                                if (datas.Queue.Contains("chess"))
    //                                {

    //                                    var game = datas.Queue.Split("+");
    //                                    var gameTime = game[1];

    //                                    PlayerPrefs.SetInt("time", int.Parse(gameTime));
    //                                    SelectChessTime.Instance.AutomaticChange();
    //                                    WaitingDialog.text = "A chess player is waiting for you to join them on a game";

    //                                    Matchmaking.Instance.RemoveTicket();
    //                                    JoinGame.onClick.AddListener(delegate { Matchmaking.Instance.FindChessMatch(100); });


    //                                }

    //                                if (datas.Queue.Contains("backgammon"))
    //                                {
    //                                    WaitingDialog.text = "A backgammon player is waiting for you to join them on a game";

    //                                    Matchmaking.Instance.RemoveTicket();
    //                                    JoinGame.onClick.AddListener(delegate { Matchmaking.Instance.FindBackgammonMatch("paris"); });
    //                                }




    //                                Waitings.Add(user.Id);
    //                                PanelHandlers.instance.OpenWaitingsPanel();

    //                            }


    //                        }
    //                        else
    //                        {
    //                            if (Waitings.Contains(user.Id))
    //                            {
    //                                Waitings.Remove(user.Id);

    //                            }
    //                        }


    //                    }
    //                }
    //            }

    //        }
    //    }




    //}
}


[Serializable]
public class AvailablesMessage
{
    public string label;
    public string gameType;
    public string subject;
    public string time;

}

public class PrefabWithNumber
{
    public GameObject prefab;
    public int number;
}