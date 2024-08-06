using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using Nakama.TinyJson;
using System.Linq;
using ByteBrewSDK;

public class PlayerList : MonoBehaviour
{

    ISession session;
    ISocket socket;
    IClient client;
    string hostPresence;
    string SecondPresence;
    static IMatch currentMatch;

    public GameObject Prefab;
    public Transform PrarentRow;

    [SerializeField] GameObject ChallengeFriendPanell;
    [SerializeField] GameObject FreindPanel;

    [SerializeField] RawImage myProfileImg;
    [SerializeField] RawImage Opponent;
    [SerializeField] Button ButtonPlay;

    [SerializeField] Sprite FriendAdded;

    List<string> friendsList = new List<string>();

    RawImage image;

    UserProfile profile;


    public string matchID = "";
    public static PlayerList instance;

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
       // UserListRPC();
    }

 

    async void UserListRPC()
    {
        var rpcid = "users";
        // var pokemonInfo = await PassData.iClient.RpcAsync(PassData.isession, rpcid);
        var pokemonInfo = await PassData.iClient.RpcAsync(PassData.isession,rpcid);

        string TrimedJson = pokemonInfo.Payload.Remove(11, 1);
 
        var data = JsonUtility.FromJson<PersonData>(TrimedJson);

        List<String> termsList = new List<String>();


      
            foreach(var id in data.client)
            {
               if(data.client.IndexOf(id) < 30)
            {
                termsList.Add(id.id);
            }

            }            


      

        var result2 = await PassData.iClient.GetUsersAsync(PassData.isession, termsList);

        foreach (var player in result2.Users)
        {
             
           
            if (player.Id != "00000000-0000-0000-0000-000000000000" && player.Id != PassData.isession.UserId)
            {


                GameObject game = Instantiate(Prefab, PrarentRow);
                Button[] addButton = game.GetComponentsInChildren<Button>();
                Image[] FriendImaes = game.GetComponentsInChildren<Image>();
                addButton[0].onClick.AddListener(delegate {

                    AddFriend(player.Username);
                    StartCoroutine(SendButton(addButton[0]));
                    FriendImaes[4].sprite = FriendAdded;


                });

                addButton[1].onClick.AddListener(delegate {

 
                    SendChallangeRequest(player.Id , player.Username ,player.AvatarUrl,"backgammon");
                    //SendPushNotification(player.Id, player.Username);
                    StartCoroutine(SendButton(addButton[1]));


                });

                addButton[2].onClick.AddListener(delegate {
 
                    SendChallangeRequest(player.Id, player.Username, player.AvatarUrl, "chess");
                    //SendPushNotification(player.Id, player.Username);
                    StartCoroutine(SendButton(addButton[1]));


                });


                // addbuton=AddFriend(player.Username);

                ArabicText text = game.GetComponentInChildren<ArabicText>();
                image = game.GetComponentInChildren<RawImage>();


                text.Text = player.DisplayName;
                var name = text.Text;


                Image[] UserImages = game.GetComponentsInChildren<Image>();
                if (player.Online)
                {
                    UserImages[5].color = Color.green;
                }
                else
                {
                    UserImages[5].color = Color.grey;
                }
 
                StartCoroutine(FetchTexture.Instance.GetTexture(player.AvatarUrl, image));
             
                StartCoroutine(GetTexture());


                if (friendsList.Contains(player.Id))
                {
                    addButton[0].image.sprite = FriendAdded;
                    addButton[0].interactable = false;
                }


            }

        }

        var FriendList = await  PassData.iClient.ListFriendsAsync(PassData.isession);


        foreach (var f in FriendList.Friends)
        {
 
                if (f.State == 0 || f.State == 1)
                {
                     friendsList.Add(f.User.Id);
                }
                else
                {
                    
                }
            }


        
    }
    //to design the buttun after it's clicked 
   public IEnumerator SendButton(Button button)
    {
        button.interactable = false;
        yield return new WaitForSeconds(3f);

        button.interactable = true;
    }

    // Update is called once per frame


    IEnumerator GetTexture(RawImage image, string url)
    {

        // fetchdata();
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
 
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
          //  Debug.Log(www.error);
        }
        else
        {
            Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;

            image.texture = FetchTexture.Instance.CropTexture((Texture2D)myTexture);

        }

    }

    public async void AddFriend(string user)
    {
        var usernames = new[] { user };
        await PassData.iClient.AddFriendsAsync(PassData.isession, null, usernames);
        var result = await client.GetUsersAsync(session, null, usernames);

        foreach (var u in result.Users)
        {
            Debug.Log("add friend user id " + u.Id);
            SendPushNotificationFriend(u.Id, session.Username);
        }
        UserProfile.instance.AddXP(5);
        UserProfile.instance.updateWallet(0);
        //ChessUserDatas.Instance.UpdateXP(5);
 
    }

    public async void SendPushNotification(string UserID, string Username , string matchId, string Type) {

 
        var payload = JsonWriter.ToJson(new { userid = UserID , username = Username , matchid = matchId +"&"+Type});
        var rpcid = "one_signal";
        await PassData.isocket.RpcAsync(rpcid, payload);
    }


    public async void SendPushNotificationFriend(string UserID, string Username)
    {

        var payload = JsonWriter.ToJson(new { userid = UserID, username = Username});
        var rpcid = "one_signal_friend";
        await PassData.isocket.RpcAsync(rpcid, payload);
    }

    public async void SendChallangeRequest(string Userid , string username ,string OpponentURL , string Type)

    {
            PassData.ChallangeType = Type;
            var match = await PassData.isocket.CreateMatchAsync();
            matchID = match.Id;

            var payload = JsonWriter.ToJson(new { userid = Userid, matchid = match.Id + "&" + Type });
            var rpcid = "custom_rpc_func_id";
            var Notification1 = await PassData.isocket.RpcAsync(rpcid, payload);

            OnlineUsers.instance.CloseOnlineUserPanel();
            ChallengeFriendPanell.SetActive(true);
            StartCoroutine(GetTexture());
            StartCoroutine(GetTexture(Opponent, OpponentURL));

            var matchjoin = await PassData.isocket.JoinMatchAsync(match.Id);

            PassData.MyPresense = match.Self;

            //  ByteBrew.NewCustomEvent("SendChallangeRequest", "Username=" + session.Username + ";");

            foreach (var user in match.Presences)
            {
                if (user.UserId != match.Self.UserId)
                {
                    PassData.OtherPresence = user;
                }
            }

            PassData.betAmount = 100;

            PassData.BoardPrice = 100;
            profile = new UserProfile();
            profile.AddXP(5);
            profile.updateWallet(100);


            hostPresence = match.Self.UserId;

            SecondPresence = Userid;


            PassData.hostPresence = hostPresence;
            PassData.SecondPresence = SecondPresence;

            // PassData.hostPresence = hostPresence;
            // PassData.SecondPresence = SecondPresence;

            currentMatch = match;
            PassData.Match = currentMatch;
            PlayerPrefs.SetString("matchID", currentMatch.Id);


            SendPushNotification(Userid, username, match.Id, Type);
 
    }



    public async void ChallangeRequestInWaiting(string Userid, string username, string OpponentURL, string Type)

    {
 

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

            myProfileImg.texture = CropTexture((Texture2D)myTexture);

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

}






