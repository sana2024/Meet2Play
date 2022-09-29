using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using UnityEngine.UI;
using UnityEngine.Networking;
using Nakama.TinyJson;

public class FriendSystem : MonoBehaviour
{

    [SerializeField] GameObject FriendPanel;
    [SerializeField] GameObject FacebookFriendListPanel;
    [SerializeField] GameObject GameFriendListPanel;
    [SerializeField] Button SwitchListButton;
    [SerializeField] Sprite FacebookOnSprite;
    [SerializeField] Sprite GameFriendOnSprite;
    [SerializeField] GameObject NoUserPanel;
    [SerializeField] InputField FriendName;
    [SerializeField] GameObject UserFound;
    [SerializeField] Text FoundUserName;
    [SerializeField] RawImage FoundUserAvatar;
    [SerializeField] Button AddButton;
    [SerializeField] GameObject FriendPrefab;
    [SerializeField] Transform FriendListHolderUI;
    [SerializeField] Text FriendNameText;
    [SerializeField] RawImage FriendAvatar;
    [SerializeField] GameObject FriendChanllegePanel;
    [SerializeField] PlayerList playerList;
    [SerializeField] Image FoundUserStatus;


    bool FacebookOn = false;
    bool GameFriendsOn = true;

    //Nakama Variables
    IClient iclient;
    ISession isession;
    ISocket isocket;


    string addFriendName;
    GameObject friends;

    // Start is called before the first frame update
    void Start()
    {
        iclient = PassData.iClient;
        isession = PassData.isession;
        isocket = PassData.isocket;

        ListFriends();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnFriendButtonClicked()
    {
        FriendPanel.SetActive(true);
        ListFriends();
    }

    public void OnCloseButtonClicked()
    {
        FriendPanel.SetActive(false);
        NoUserPanel.SetActive(false);
        UserFound.SetActive(false);
        FriendName.text = "";

    }

    public void OnSwitchedButtonClicked()
    {
        FacebookOn = !FacebookOn;
        GameFriendsOn = !GameFriendsOn;

        if(FacebookOn == true && GameFriendsOn == false)
        {
            FacebookFriendListPanel.SetActive(true);
            GameFriendListPanel.SetActive(false);
            SwitchListButton.image.sprite = FacebookOnSprite;
        }
        if(FacebookOn == false && GameFriendsOn == true)
        {
            FacebookFriendListPanel.SetActive(false);
            GameFriendListPanel.SetActive(true);
            SwitchListButton.image.sprite = GameFriendOnSprite;
        }
    }

    public async void AddFriend()
    {
        var usernames = new[] { addFriendName };
        await iclient.AddFriendsAsync(isession, null, usernames);
 
    }

    public async void ListFriends()
    {
        if (GameFriendListPanel.active)
        {
            foreach (Transform item in FriendListHolderUI)
            {
                Destroy(item.gameObject);
            }

        }
        var result = await iclient.ListFriendsAsync(isession , null , 10);
 
        foreach (var f in result.Friends)
        {
            if (f.State == 0)
            {

                       friends = Instantiate(FriendPrefab, FriendListHolderUI);
                       FriendAvatar = friends.GetComponentInChildren<RawImage>();
                       FriendNameText = friends.GetComponentInChildren<Text>();
                       FriendNameText.text = f.User.Username;
                       StartCoroutine(GetTexture(f.User.AvatarUrl ,FriendAvatar ));
                       Button ChallangeButton = friends.GetComponentInChildren<Button>();
                       ChallangeButton.onClick.AddListener(() => { playerList.SendNotificationRpc(f.User.Id); });


                     Image[] UserImages = friends.GetComponentsInChildren<Image>();
                     if (f.User.Online)
                      {
                    UserImages[4].color = Color.green;
                       }
                    else
                      {
                    UserImages[4].color = Color.grey;
                       }



            }

            
        }
    }

   

    public async void FindFriend()
    {
        var name = FriendName.text;
        var username = new[] { name };
        var result = await iclient.GetUsersAsync(isession, null ,username );

        Debug.Log(result);
        Debug.Log(result.Users);

        if(result.ToString() == "Users: [], ")
        {
            NoUserPanel.SetActive(true);
            UserFound.SetActive(false);
        }
        else
        {
        foreach (var u in result.Users)
        {
            Debug.Log(u.DisplayName + " " + u.Online +" "+u.AvatarUrl);

            Debug.Log(result.Users.Equals(u));

                UserFound.SetActive(true);
                NoUserPanel.SetActive(false);
                FoundUserName.text = u.DisplayName;
                addFriendName = u.DisplayName;
                StartCoroutine(GetTexture(u.AvatarUrl , FoundUserAvatar));

                Image[] UserImages = friends.GetComponentsInChildren<Image>();
                if (u.Online)
                {
                    FoundUserStatus.color = Color.green;
                }
                else
                {
                    FoundUserStatus.color = Color.grey;
                }



            }

            var FriendList = await iclient.ListFriendsAsync(isession);


            foreach (var f in FriendList.Friends)
            {
                if (f.User.Username == addFriendName)
                {

                    if(f.State == 0 || f.State == 1)
                    {
                        AddButton.interactable = false;
                    }
                    else { 
                        AddButton.interactable = true;
                    }
                }


            }
        }  

    }

    public void OnCloseButtonChallengePanel()
    {
        FriendChanllegePanel.SetActive(false);
    }

    IEnumerator GetTexture(string uri , RawImage rawImage)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(uri);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;

            if(rawImage != null)
            {
            rawImage.texture = myTexture;
            }

 
            
        }

    }

 

}
