using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class leaderboard1 : MonoBehaviour
{



    ISession session;
    ISocket socket;
    IClient client;

    public GameObject Prefab;
    public Transform PrarentRow;
    private string leaderboardId = "level1";
    RawImage image;


    [SerializeField] Sprite Gold;
    [SerializeField] Sprite Sliver;
    [SerializeField] Sprite bronze;
    [SerializeField] PlayerList playerList;







    // Start is called before the first frame update
    void Start()
    {

        // click();

      //  createleaderBoard();

    }

 
    void Update()
    {



    }

    public void createleaderBoard()
    {

        listLeaderBoard();
    }

    async void listLeaderBoard()
    {
        foreach (Transform leaderboardOb in PrarentRow)
        {
            Destroy(leaderboardOb.gameObject);
        }

        var result = await PassData.iClient.ListLeaderboardRecordsAsync(PassData.isession, leaderboardId, null, null, 10);

        foreach (var d in result.Records)
            {

                var Rank = d.Rank;

 
                GameObject game = Instantiate(Prefab, PrarentRow);
        

                Image[] img = game.GetComponentsInChildren<Image>();


            ArabicText[] text = game.GetComponentsInChildren<ArabicText>();
                image = game.GetComponentInChildren<RawImage>();

            string [] userIds = { d.OwnerId };
            var result2 = await PassData.iClient.GetUsersAsync(PassData.isession, userIds);


            

            foreach( var user in result2.Users)
            {


                Button[] ChallangeButton  = game.GetComponentsInChildren<Button>();
                ChallangeButton[0].onClick.AddListener(() => {
                    AppFlyer.Instance.AppsFlyerEvent("Send_Challange_Leaderboard_Backgammon",PassData.isession.Username);
                    playerList.SendChallangeRequest(d.OwnerId, d.Username,user.AvatarUrl, "backgammon");
                   // playerList.SendPushNotification(d.OwnerId, d.Username);

                });

                ChallangeButton[1].onClick.AddListener(() => {
                    AppFlyer.Instance.AppsFlyerEvent("Send_Challange_Leaderboard_chess", PassData.isession.Username);
                    playerList.SendChallangeRequest(d.OwnerId, d.Username, user.AvatarUrl, "chess");
                    // playerList.SendPushNotification(d.OwnerId, d.Username);

                });
 
                    StartCoroutine(FetchTexture.Instance.GetTexture( user.AvatarUrl, image));
                 

                if (user.Online)
                {
                    img[8].color = Color.green;

                    if (user.Id == PassData.isession.UserId)
                    {
                        game.transform.GetChild(8).gameObject.SetActive(true);
                    }


                        if (!OnlineUsers.instance.AvailableUsers.Contains(user.Id))
                    {
                        
                        game.transform.GetChild(7).gameObject.SetActive(true);
                    }

                }
                else
                {
                    img[8].color = Color.grey;
                }
            }

            

            switch (Rank)
                {

                    case "1":

                        img[5].sprite = Gold;

                        break;

                    case "2":

                        img[5].sprite = Sliver;

                        break;

                    case "3":

                        img[5].sprite = bronze;

                        break;

                    default:
                        img[5].gameObject.SetActive(false);
                        break;




                }
               
 
                text[0].Text = d.Username.ToString();
            if(d.Score != null)
            {
           text[1].Text = d.Score.ToString();
            }
     
                text[2].Text = d.Rank.ToString();

            /*

            foreach (var player in result2.Users)
            {
               
 
               StartCoroutine(GetTexture(image, player.AvatarUrl));

  
            }

            */
        }
    }

 
    //IEnumerator GetTexture(RawImage image, string url)
    //{

    //    // fetchdata();
    //    UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
    
    //    yield return www.SendWebRequest();

    //    if (www.isNetworkError || www.isHttpError)
    //    {
    //        Debug.Log(www.error);
    //    }
    //    else
    //    {
    //        Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;

    //        image.texture = myTexture;

    //    }

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

}