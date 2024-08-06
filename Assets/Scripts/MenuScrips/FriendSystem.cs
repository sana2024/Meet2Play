using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using UnityEngine.UI;
using UnityEngine.Networking;
using Nakama.TinyJson;
using System;
using System.Linq;
using System.Threading.Tasks;
using ByteBrewSDK;

public class FriendSystem : MonoBehaviour
{
    [SerializeField] GameObject FriendsSection;
    [SerializeField] GameObject RequestsSection;
    [SerializeField] GameObject FindSection;

    [SerializeField] GameObject FriendsSelected;
    [SerializeField] GameObject RequestsSelected;
    [SerializeField] GameObject FindSelected;

    [SerializeField] GameObject FriendRequestPrefab;
    [SerializeField] public GameObject FriendPanel;

    [SerializeField] GameObject NoUserPanel;
    [SerializeField] InputField FriendName;
    [SerializeField] GameObject UserFound;
    [SerializeField] ArabicText FoundUserName;
    [SerializeField] RawImage FoundUserAvatar;
    [SerializeField] Button AddButton;
    [SerializeField] Image AddImage;
    [SerializeField] Button ChallangeFoundBackButton;
    [SerializeField] Button ChallangeFoundChessButton;
    [SerializeField] GameObject FriendPrefab;
    [SerializeField] Transform FriendListHolderUI;
    [SerializeField] Transform FriendRequestHolderUI;
    [SerializeField] RawImage FriendAvatar;
    [SerializeField] public GameObject FriendChanllegePanel;
    [SerializeField] PlayerList playerList;
    [SerializeField] Image FoundUserStatus;
    [SerializeField] Sprite FriendAdded;
    [SerializeField] Sprite FriendNotAdded;
    [SerializeField] public Text FriendRequestCounts;
    [SerializeField] Button challangePlayButton;
    [SerializeField] GameObject UnfriendPanelPrefab;
    [SerializeField] Transform UnfriendPanelTransform;



    ArabicText FriendNameText;
    public GameObject contentHolderl;
    public GameObject[] element;
    public int requestCounts;
    public int totalelment;
    public static FriendSystem instance;

 
    //Nakama Variables
    IClient iclient;
    ISession isession;
    ISocket isocket;


    string addFriendName;
    GameObject friends;
    GameObject requests;


   public List<IApiNotification> FriendRequests = new List<IApiNotification>();
   public List<IApiFriend> Friends = new List<IApiFriend>();



    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        iclient = PassData.iClient;
        isession = PassData.isession;
        isocket = PassData.isocket;


        ListOfflineRequestsAsync();
        ListFriends();


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public  async void ListOfflineRequestsAsync()
    {


        var cacheableCursor = PlayerPrefs.GetString("nakama.notificationsCacheableCursor");
        var result = await iclient.ListNotificationsAsync(isession, 100 , cacheableCursor);
       // Debug.Log("result cashable cursor " + result.CacheableCursor);
        PlayerPrefs.SetString("nakama.notificationsCacheableCursor", result.CacheableCursor);


        foreach (var notification in result.Notifications)
        {
 
            if (notification.Code == -2)
            {
                requestCounts++;
                FriendRequestCounts.text = requestCounts.ToString();
                FriendRequests.Add(notification);
            }


        }
    }

    public void OnFriendButtonClicked()
    {
        FriendPanel.SetActive(true);
        totalelment = contentHolderl.transform.childCount;
        CreateFriendsUI();
        element = new GameObject[totalelment];

        for (int i = 0; i < totalelment; i++)
        {
            element[i] = contentHolderl.transform.GetChild(i).gameObject;
        }
    }

    public void SelectFriends()
    {
        FriendsSection.SetActive(true);
        FriendsSelected.SetActive(true);

        RequestsSection.SetActive(false);
        RequestsSelected.SetActive(false);

        FindSection.SetActive(false);
        FindSelected.SetActive(false);

        CreateFriendsUI();
    }

    public void  SelectRequests()
    {
        
        FriendsSection.SetActive(false);
        FriendsSelected.SetActive(false);

        RequestsSection.SetActive(true);
        RequestsSelected.SetActive(true);

        FindSection.SetActive(false);
        FindSelected.SetActive(false);

        CreateRequestUI();

    }

    public void SelectFind()
    {
        FriendsSection.SetActive(false);
        FriendsSelected.SetActive(false);

        RequestsSection.SetActive(false);
        RequestsSelected.SetActive(false);

        FindSection.SetActive(true);
        FindSelected.SetActive(true);
    }

    public async void CreateRequestUI()
    {

        if (RequestsSection.active)
        {
            foreach (Transform item in FriendRequestHolderUI)
            {
                Destroy(item.gameObject);
            }

        }

        foreach (var friendRequest in FriendRequests)
        {
 
            requests = Instantiate(FriendRequestPrefab, FriendRequestHolderUI);


            var usr = new[] { friendRequest.SenderId };
            var senders = await UserProfile.instance.client.GetUsersAsync(UserProfile.instance.session, usr);



            foreach (var sender in senders.Users)
            {

                RawImage senderAvatar = requests.GetComponentInChildren<RawImage>();
                ArabicText senderName = requests.GetComponentInChildren<ArabicText>();
                Button[] ButtonResponse = requests.GetComponentsInChildren<Button>();
                Image[] images = requests.GetComponentsInChildren<Image>();


                StartCoroutine(Notifications.Instance.GetTexture(sender.AvatarUrl, senderAvatar));
                senderName.Text = sender.Username;

                if (sender.Online)
                {
                images[2].color = Color.green;
                }
                else
                {
                images[2].color = Color.grey;
                }


                ButtonResponse[0].onClick.AddListener(async () =>
                {
                    RejectRequest(sender.Id, ButtonResponse[0]);
                    await iclient.DeleteNotificationsAsync(isession, new[] { friendRequest.Id });
                    FriendRequests.Remove(friendRequest);
                    Destroy(requests);
                    CreateRequestUI();


                });


                ButtonResponse[1].onClick.AddListener(async () =>
                {
                    AcceptRequest(sender.Id, ButtonResponse[1]);
                    await iclient.DeleteNotificationsAsync(isession, new[] { friendRequest.Id });
                    FriendRequests.Remove(friendRequest);
                    Destroy(requests);
                    CreateRequestUI();

                });
            }

        }
    }

    public async void AcceptRequest(string UserId, Button acceptButton)
    {
        var id = new[] { UserId };
        await iclient.AddFriendsAsync(isession, id, null);
        Friends.Clear();
        ListFriends();
        StartCoroutine(ButtonClickedDesign(acceptButton));

        requestCounts--;
        FriendRequestCounts.text = requestCounts.ToString();
    }

    public async void RejectRequest(string UserId, Button rejectButton)
    {
        var id = new[] { UserId };
        await iclient.DeleteFriendsAsync(isession, id, null);
        StartCoroutine(ButtonClickedDesign(rejectButton));
        if(requestCounts != 0)
        {
           requestCounts--;
        }

        FriendRequestCounts.text = requestCounts.ToString();
    }

    public void Search()
    {
 

        int searchlength = FriendName.text.Length;
        int searchElements = 0;
        foreach (GameObject item in element)
        {
            searchElements += 1;
            if (item.transform.GetChild(0).GetComponent<Text>().text.Length > searchlength)
            {

                if (FriendName.text.ToLower() == item.transform.GetChild(0).GetComponent<Text>().text.Substring(0, searchlength).ToLower())
                {

                    item.SetActive(true);
                }
                else
                {

                    item.SetActive(false);
                }

            }
        }

        if(FriendName.text.Length == 0)
        {
            NoUserPanel.SetActive(false);
            UserFound.SetActive(false);
        }
    }

    public void OnCloseButtonClicked()
    {
        FriendPanel.SetActive(false);
        NoUserPanel.SetActive(false);
        UserFound.SetActive(false);
        FriendName.text = "";

    }

    public async void AddFriend()
    {
        var usernames = new[] { addFriendName };
        await iclient.AddFriendsAsync(isession, null, usernames);

        var result = await iclient.GetUsersAsync(isession, null, usernames);

        foreach (var u in result.Users)
        {
           // Debug.Log("add friend user id "+ u.Id);
            PlayerList.instance.SendPushNotificationFriend(u.Id, isession.Username);
        }

        AddImage.sprite = FriendAdded;
        ByteBrew.NewCustomEvent("AddFriend", "Username=" + PassData.isession.Username + ";");
    }

    public async void ListFriends()
    {
        if (Friends.Any())
        {
            Friends.Clear();
        }

        var friends = await iclient.ListFriendsAsync(isession, null, 100);

        foreach(var friend in friends.Friends)
        {
            Friends.Add(friend);
        }



        //if (GameFriendListPanel.active)
        //{
        //    foreach (Transform item in FriendListHolderUI)
        //    {
        //        Destroy(item.gameObject);
        //    }

        //}
        //var result = await iclient.ListFriendsAsync(isession , null , 10);

        //foreach (var f in result.Friends)
        //{
        //    if (f.State == 0)
        //    {

        //               friends = Instantiate(FriendPrefab, FriendListHolderUI);
        //               FriendAvatar = friends.GetComponentInChildren<RawImage>();
        //               FriendNameText = friends.GetComponentInChildren<ArabicText>();

        //               FriendNameText.Text = f.User.Username;

        //            StartCoroutine(FetchTexture.Instance.GetTexture(f.User.AvatarUrl, FriendAvatar));


        //               Button[] ChallangeButton = friends.GetComponentsInChildren<Button>();
        //               ChallangeButton[0].onClick.AddListener(() => {
        //                   playerList.SendChallangeRequest(f.User.Id ,f.User.Username ,f.User.AvatarUrl, "backgammon");
        //              // playerList.SendPushNotification(f.User.Id, f.User.Username);
        //               });
        //        ChallangeButton[1].onClick.AddListener(() => {
        //            playerList.SendChallangeRequest(f.User.Id, f.User.Username, f.User.AvatarUrl, "chess");
        //            // playerList.SendPushNotification(f.User.Id, f.User.Username);
        //        });

        //        Image[] UserImages = friends.GetComponentsInChildren<Image>();
        //             if (f.User.Online)
        //              {
        //            UserImages[4].color = Color.green;
        //               }
        //            else
        //              {
        //            UserImages[4].color = Color.grey;
        //               }
        //    }    
        //  }
    }

    public void CreateFriendsUI()
    {
        if (FriendsSection.activeSelf)
        {
            foreach (Transform item in FriendListHolderUI)
            {
                Destroy(item.gameObject);
            }

        }

        foreach(var friend in Friends)
        {
            if (friend.State == 0)
            {
                friends = Instantiate(FriendPrefab, FriendListHolderUI);
                FriendAvatar = friends.GetComponentInChildren<RawImage>();
                FriendNameText = friends.GetComponentInChildren<ArabicText>();

                FriendNameText.Text = friend.User.Username;

                StartCoroutine(FetchTexture.Instance.GetTexture(friend.User.AvatarUrl, FriendAvatar));


                Button[] FriendButton = friends.GetComponentsInChildren<Button>();
                FriendButton[0].onClick.AddListener(() =>
                {
                    playerList.SendChallangeRequest(friend.User.Id, friend.User.Username, friend.User.AvatarUrl, "backgammon");
                    AppFlyer.Instance.AppsFlyerEvent("Send_Challange_Friends_Backgammon", isession.Username);
                    // playerList.SendPushNotification(f.User.Id, f.User.Username);
                });
                FriendButton[1].onClick.AddListener(() =>
                {
                    playerList.SendChallangeRequest(friend.User.Id, friend.User.Username, friend.User.AvatarUrl, "chess");
                    AppFlyer.Instance.AppsFlyerEvent("Send_Challange_Friends_Backgammon", isession.Username);
                    // playerList.SendPushNotification(f.User.Id, f.User.Username);
                });

                FriendButton[2].onClick.AddListener(() =>
                {
                    OpenUnfriendPanel(friend, friends);
                });

                Image[] UserImages = friends.GetComponentsInChildren<Image>();
              //  Debug.Log("fiend status "+friend.User.Online);
                if (friend.User.Online)
                {
                    UserImages[6].color = Color.green;
                    if (!OnlineUsers.instance.AvailableUsers.Contains(friend.User.Id))
                    {
                        friends.transform.GetChild(5).gameObject.SetActive(true);
                    }
                }
                else
                {
                    UserImages[6].color = Color.grey;
                    friends.transform.GetChild(5).gameObject.SetActive(false);

                }

            }
        }
    }

    public async void OpenUnfriendPanel(IApiFriend friend , GameObject friendOb)
    {
      var unfriend = Instantiate(UnfriendPanelPrefab, UnfriendPanelTransform);
      Text[] unfriendText = unfriend.GetComponentsInChildren<Text>();
      unfriendText[0].text = "Are you sure you want to remove " + friend.User.Username + " from your friends list?";

      Button[] unfriendButtons = unfriend.GetComponentsInChildren<Button>();
        unfriendButtons[0].onClick.AddListener(() =>
        {
            CloseUnfriendPanel(unfriend);
        });

        unfriendButtons[1].onClick.AddListener(() =>
        {
            Unfriend(unfriend, friend, friendOb);
        });

    }

    public async void CloseUnfriendPanel(GameObject unfriendPanel)
    {
        Destroy(unfriendPanel);
    }

    public async void Unfriend(GameObject unfriendPanel, IApiFriend friend, GameObject friendOb)
    {
        var usernames = new[] { friend.User.Username };
        await iclient.DeleteFriendsAsync(isession, null, usernames);
        Friends.Remove(friend);
        Destroy(unfriendPanel);
        Destroy(friendOb);
        //Unfriend(friend.User.Username);
        //Friends.Remove(friend);
        //Destroy(friends);

    }



    public async void FindFriend()
    {
        AddButton.interactable = true;
        AddImage.sprite = FriendNotAdded;

        var payload = JsonWriter.ToJson(new { name = FriendName.text });
        var result = await iclient.RpcAsync(isession, "test_rpc", payload);
        if(result.ToString().Count() > 28)
        {
            string TrimedJson = result.Payload.Remove(11, 1);
            var data = JsonUtility.FromJson<PersonData>(TrimedJson);

            List<String> termsList = new List<String>();

                foreach (var id in data.client)
                {
                    if (data.client.IndexOf(id) < 30)
                    {
                        termsList.Add(id.id);

                    }

                }

            var user_result = await iclient.GetUsersAsync(isession, termsList);
           // Debug.Log(user_result);


                foreach (var u in user_result.Users)
                {
                    if(u.Id != "00000000-0000-0000-0000-000000000000" && u.Id != isession.UserId)
                    {
                        UserFound.SetActive(true);
                        NoUserPanel.SetActive(false);
                        FoundUserName.Text = u.DisplayName;
                        addFriendName = u.DisplayName;

                        StartCoroutine(FetchTexture.Instance.GetTexture(u.AvatarUrl, FoundUserAvatar));


                        ChallangeFoundBackButton.onClick.AddListener(delegate
                        {
                            AppFlyer.Instance.AppsFlyerEvent("Send_Challange_FriendSearch_Backgammon", isession.Username);
                            PlayerList.instance.SendChallangeRequest(u.Id, u.Username, u.AvatarUrl, "backgammon");
                            // PlayerList.instance.SendPushNotification(u.Id, u.Username);
                        });

                        ChallangeFoundChessButton.onClick.AddListener(delegate
                        {
                            AppFlyer.Instance.AppsFlyerEvent("Send_Challange_FriendSearch_Chess", isession.Username);
                            PlayerList.instance.SendChallangeRequest(u.Id, u.Username, u.AvatarUrl, "chess");
                            // PlayerList.instance.SendPushNotification(u.Id, u.Username);
                        });


                        if (u.Online)
                        {
                            FoundUserStatus.color = Color.green;
                        }
                        else
                        {
                            FoundUserStatus.color = Color.grey;
                        }

                    }


                }

                var FriendList = await iclient.ListFriendsAsync(isession);


                foreach (var f in FriendList.Friends)
                {
                    if (f.User.Username == addFriendName)
                    {
                   // Debug.Log("friendship state "+ f.State);
                        if (f.State == 0 || f.State == 1 || f.State == 2)
                        {
                            AddButton.interactable = false;
                            AddImage.sprite = FriendAdded;
                        }
                        else
                        {
                            AddButton.interactable = true;
                            AddImage.sprite = FriendNotAdded;
                    }
                    }


                
            }
        }
        else
        {
            NoUserPanel.SetActive(true);
            UserFound.SetActive(false);
        }

    }

    public void OnCloseButtonChallengePanel()
    {
        challangePlayButton.gameObject.SetActive(false);
        FriendChanllegePanel.SetActive(false);
    }

    IEnumerator ButtonClickedDesign(Button button)
    {
        button.interactable = false;
        yield return new WaitForSeconds(3f);
        button.interactable = true;

    }

    public async void CheckIfFriend(string username, Button Add, Image AddButtonImage)
    {
        var FriendList = await iclient.ListFriendsAsync(isession, null , 100);


        foreach (var f in FriendList.Friends)
        {

            if (f.User.Username == username)
            {

                if (f.State == 0 || f.State == 1)
                {
                    Add.interactable = false;
                    AddButtonImage.sprite = FriendAdded;
                }
                else
                {
                    Add.interactable = true;
                    AddButtonImage.sprite = FriendAdded;
                }
            }


        }
    }


}
