using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using Nakama.TinyJson;
using System.Linq;

public class Notifications : MonoBehaviour
{
    IClient client;
    ISession session;
    ISocket isocket;
    static IMatch currentMatch;
    IMatchmakerMatched MatchmakerMatched;


    [SerializeField] Text NotificationAmountText;
    [SerializeField] Text username;
    [SerializeField] RawImage userAvatar;
    [SerializeField] RawImage OpponentAvatar;

    [SerializeField] GameObject NotificationPanel;
    [SerializeField] GameObject notificationPrefab;
    [SerializeField] Transform notificationParent;
    [SerializeField] FriendSystem friendsSystem;
    [SerializeField] GameObject FriendRequestPanel;
    [SerializeField] Button ButtonPlay;
    [SerializeField] GameObject ChallengeFriendPanell;
    [SerializeField] Button ButtonPlayRequest;
    UserProfile profile;


    int NotificationAmount = 0;
    string SenderUsername = "";
    string reciverId = "";
    string subject = "";
    string message = "";
    string sendId = "";

    string messageAccept;
 


    // Start is called before the first frame update
    async void Start()
    {
        client = PassData.iClient;
        session = PassData.isession;


        isocket = PassData.isocket;


        var mainThread = UnityMainThreadDispatcher.Instance();


        var result = await client.ListNotificationsAsync(session, 4 , PlayerPrefs.GetString("nakama.notificationsCacheableCursor"));

        NotificationAmount = result.Notifications.Count();
        NotificationAmountText.text = NotificationAmount.ToString();


        isocket.ReceivedNotification += notification => mainThread.Enqueue(() => onRecivedNotification(notification));
 

    }


    private void onRecivedNotification(IApiNotification notification)
    {
        switch (notification.Code)
        {
            case -2:

                NotificationAmount++;
                NotificationAmountText.text = NotificationAmount.ToString();

                break;

            case 102:
 
                subject = notification.Subject;
                sendId = notification.SenderId;
                PassData.BoardPrice = 100;
                profile = new UserProfile();
                profile.updateWallet(100);
 

                NotificationAmount++;
                NotificationAmountText.text = NotificationAmount.ToString();

                // Debug.Log("Notification Subject"+notification.Subject+"/"+notification.SenderId);
                break;

            case 103:
 
                message = notification.Subject;
                sendId = notification.SenderId;
                NotificationAmount++;
                NotificationAmountText.text = NotificationAmount.ToString();
                ChallengeFriendPanell.SetActive(false);
                break;
                

            case 104:

                messageAccept = notification.Subject;
                sendId = notification.SenderId;
                NotificationAmount++;
                NotificationAmountText.text = NotificationAmount.ToString();
                ButtonPlayRequest.gameObject.SetActive(true);
                break;



        }

    }


    public async void ListNotifications()
    {

        foreach (Transform item in notificationParent)
        {
            Destroy(item.gameObject);
        }

        var cacheableCursor = PlayerPrefs.GetString("nakama.notificationsCacheableCursor", null);


        var result = await client.ListNotificationsAsync(session , 4 , cacheableCursor);
        PlayerPrefs.SetString("nakama.notificationsCacheableCursor", result.CacheableCursor);

 


 

            foreach (var n in result.Notifications)
            {
                 

            if (NotificationPanel.active)
               {
                Debug.Log(n.Subject);
                GameObject Notification = Instantiate(notificationPrefab, notificationParent);
                Notification.transform.parent = notificationParent.transform;
                Text Message = Notification.GetComponentInChildren<Text>();
                Button[] ButtonResponse = Notification.GetComponentsInChildren<Button>();


                string[] SenderProfile = { n.SenderId };


                var senderaacount = await PassData.iClient.GetUsersAsync(PassData.isession, SenderProfile);
                foreach (var sender in senderaacount.Users)
                 {

                     if (n.Code == 102)
                     {
 
                        ButtonResponse[0].onClick.AddListener(() => JoinedPlayers(sender.Id, messageAccept, subject));
 
                        ButtonResponse[1].onClick.AddListener(() => SendNotificationRpcReject(sender.Id, message));

                        // SenderProfile(n.SenderId);
                        Message.text = sender.Username + " " + "Challenged You For A game";

                    }

 
                    if (n.Code == -2)
                    {
 
                        ButtonResponse[0].onClick.AddListener(() => AcceptRequest(sender.Id) );
                        ButtonResponse[1].gameObject.SetActive(false);

                        Message.text = sender.Username + " " + "Sent you friend request";

                    }

                    if (n.Code == -3)
                    {
                        ButtonResponse[0].gameObject.SetActive(false);
                        ButtonResponse[1].gameObject.SetActive(false);
                        Message.text = sender.Username + " " + "Accepted your friend request";

                    }
                }
            }

        }
 
        }
    



    public async void AcceptRequest(string UserId)
    {

        var id = new[] { UserId };
        await client.AddFriendsAsync(session, id, null);

    }


    public async void JoinedPlayers(string UserId, string messageAccept, string matchmakerMatched)
    {
        NotificationPanel.SetActive(false);
        SendAcceptRequest(UserId, messageAccept);
        var rpcidUser = "users";
        // var pokemonInfo = await PassData.iClient.RpcAsync(PassData.isession, rpcidUser);
        var pokemonInfo = await PassData.isocket.RpcAsync(rpcidUser);

        string TrimedJson = pokemonInfo.Payload.Remove(11, 1);

        var data = JsonUtility.FromJson<PersonData>(TrimedJson);

        List<String> termsList = new List<String>();


        foreach (var id in data.client)
        {

            termsList.Add(id.id);





        }


        var result2 = await PassData.iClient.GetUsersAsync(PassData.isession, termsList);



        foreach (var user in result2.Users)
        {
            StartCoroutine(GetTexture(user.AvatarUrl, OpponentAvatar));
            StartCoroutine(GetTexture());
            //   StartCoroutine(GetTexture(user.AvatarUrl,userAvatar));
            ButtonPlay.gameObject.SetActive(true);
            FriendRequestPanel.SetActive(true);



        }

        Debug.Log(matchmakerMatched);

        var match = await PassData.isocket.JoinMatchAsync(matchmakerMatched);
        PassData.betAmount = 100;
        PassData.BoardPrice = 100;
        profile = new UserProfile();
        profile.updateWallet(100);
        PassData.hostPresence = sendId;
        PassData.SecondPresence = match.Self.UserId;

 
        currentMatch = match;
        PassData.Match = currentMatch;
        PlayerPrefs.SetString("matchID", currentMatch.Id);




        Debug.Log(PassData.hostPresence);

        var otheruserid = PassData.SecondPresence;
 



    }


    public async void SendAcceptRequest(string Userid, string message)
    {

        var payload = JsonWriter.ToJson(new { userid = Userid, messageAccept = message });
        var rpcid = "custom_rpc_func_Accept";
        // var Notification = await PassData.iClient.RpcAsync(PassData.isession, rpcid, payload);
        var Notification1 = await PassData.isocket.RpcAsync(rpcid, payload);




    }

    public async void SendNotificationRpcReject(string Userid, string message)

    {

        var payload = JsonWriter.ToJson(new { userid = Userid, message = message });
        var rpcid = "custom_rpc_func_reject";
        // var Notification = await PassData.iClient.RpcAsync(PassData.isession, rpcid, payload);
        var Notification1 = await PassData.isocket.RpcAsync(rpcid, payload);




    }


    public async void RemoveNotification(string NotificationIds, GameObject notification)
    {
        var notificationIds = new[] { NotificationIds };
        await client.DeleteNotificationsAsync(session, notificationIds);
        notification.SetActive(false);
    }



    public async void SenderProfile(string SenderId)
    {
        var id = new[] { SenderId };
        var result = await client.GetUsersAsync(session, id, null);

        foreach (var user in result.Users)
        {
            SenderUsername = user.Username;
        }
    }


    IEnumerator GetTexture(string uri, RawImage rawImage)
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
            rawImage.texture = myTexture;

        }

    }


    public void OnNotificationClicked()
    {
        NotificationPanel.SetActive(true);
        ListNotifications();
    }

    public void OnCancelClicked()
    {
        NotificationPanel.SetActive(false);
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

            userAvatar.texture = myTexture;

        }

    }

}



 

