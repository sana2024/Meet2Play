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

    RawImage image;

    UserProfile profile;
    // Start is called before the first frame update
    void Start()
    {
        UserListRPC();
    }

   async void UserListRPC()
    {
        var rpcid = "users";
        // var pokemonInfo = await PassData.iClient.RpcAsync(PassData.isession, rpcid);
        var pokemonInfo = await PassData.isocket.RpcAsync(rpcid);

        string TrimedJson = pokemonInfo.Payload.Remove(11, 1);

        var data = JsonUtility.FromJson<PersonData>(TrimedJson);

        List<String> termsList = new List<String>();


        foreach (var id in data.client)
        {

            termsList.Add(id.id);


        }


        var result2 = await PassData.iClient.GetUsersAsync(PassData.isession, termsList);

        foreach (var player in result2.Users)
        {


            if (player.Id != "00000000-0000-0000-0000-000000000000" && player.Id != PassData.isession.UserId)
            {

                GameObject game = Instantiate(Prefab, PrarentRow);
                Button[] addButton = game.GetComponentsInChildren<Button>();
                addButton[0].onClick.AddListener(() => AddFriend(player.Username));
                addButton[1].onClick.AddListener(() => SendNotificationRpc(player.Id));





                // addbuton=AddFriend(player.Username);

                Text text = game.GetComponentInChildren<Text>();
                image = game.GetComponentInChildren<RawImage>();


                text.text = player.DisplayName;
                var name = text.text;


                Image[] UserImages = game.GetComponentsInChildren<Image>();
                if (player.Online)
                {
                    UserImages[4].color = Color.green;
                }
                else
                {
                    UserImages[4].color = Color.grey;
                }

                StartCoroutine(GetTexture(image, player.AvatarUrl));
                StartCoroutine(GetTexture());
                StartCoroutine(GetTexture(Opponent, player.AvatarUrl));





            }

        }
    }

    // Update is called once per frame


    IEnumerator GetTexture(RawImage image, string url)
    {

        // fetchdata();
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        Debug.Log("image url " + www);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;

            image.texture = myTexture;

        }

    }

    public async void AddFriend(string user)
    {
        var usernames = new[] { user };
        await PassData.iClient.AddFriendsAsync(PassData.isession, null, usernames);

        Debug.Log("it is working Properly");


    }



    public async void SendNotificationRpc(string Userid)

    {

        var match = await PassData.isocket.CreateMatchAsync();

        var payload = JsonWriter.ToJson(new { userid = Userid, matchid = match.Id });
        var rpcid = "custom_rpc_func_id";
        // var Notification = await PassData.iClient.RpcAsync(PassData.isession, rpcid, payload);
        var Notification1 = await PassData.isocket.RpcAsync(rpcid, payload);
 
        ChallengeFriendPanell.SetActive(true);
        StartCoroutine(GetTexture());
        var matchjoin = await PassData.isocket.JoinMatchAsync(match.Id);
        PassData.betAmount = 100;

        PassData.BoardPrice = 100;
        profile = new UserProfile();
        profile.updateWallet(100);


        hostPresence = match.Self.UserId;

        SecondPresence = Userid;


        PassData.hostPresence = hostPresence;
        PassData.SecondPresence = SecondPresence;




        // PassData.hostPresence = hostPresence;
        // PassData.SecondPresence = SecondPresence;

        Debug.Log("hostPlayer " + hostPresence);
        Debug.Log("SecondPresence" + Userid);


        Debug.Log("Our Match ID: " + match.Self.SessionId);
        currentMatch = match;
        PassData.Match = currentMatch;
        PlayerPrefs.SetString("matchID", currentMatch.Id);








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

            myProfileImg.texture = myTexture;

        }

    }

}






