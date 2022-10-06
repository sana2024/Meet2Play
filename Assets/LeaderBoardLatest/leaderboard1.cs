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

        createleaderBoard();

    }

 
    void Update()
    {



    }

    public async void createleaderBoard()
    {

        listLeaderBoard();
    }

    async void listLeaderBoard()
    {

        var rpcid = "users";
        var pokemonInfo = await PassData.iClient.RpcAsync(PassData.isession, rpcid);

        string TrimedJson = pokemonInfo.Payload.Remove(11, 1);

        var data = JsonUtility.FromJson<PersonData>(TrimedJson);

        List<String> termsList = new List<String>();


        foreach (var id in data.client)
        {

            termsList.Add(id.id);
 
        }

        var result = await PassData.iClient.ListLeaderboardRecordsAsync(PassData.isession, leaderboardId, null, null, 10);


        List<String> userId = new List<String>();

        foreach (var user in result.Records)
        {
            userId.Add(user.OwnerId);


        }
 
 

 
        foreach (var d in result.Records)
            {

 
                var Rank = d.Rank;

 
                GameObject game = Instantiate(Prefab, PrarentRow);

                Image[] img = game.GetComponentsInChildren<Image>();

                Button ChallangeButton = game.GetComponentInChildren<Button>();
                ChallangeButton.onClick.AddListener(() => { playerList.SendNotificationRpc(d.OwnerId); });


            Text[] text = game.GetComponentsInChildren<Text>();
                image = game.GetComponentInChildren<RawImage>();

            string [] userIds = { d.OwnerId };
            var result2 = await PassData.iClient.GetUsersAsync(PassData.isession, userIds);


            

            foreach( var user in result2.Users)
            {
                StartCoroutine(GetTexture(image, user.AvatarUrl));

                if (user.Online)
                {
                    img[4].color = Color.green;

                }
                else
                {
                    img[4].color = Color.grey;
                }
            }

            

            switch (Rank)
                {

                    case "1":

                        img[2].sprite = Gold;

                        break;

                    case "2":

                        img[2].sprite = Sliver;

                        break;

                    case "3":

                        img[2].sprite = bronze;

                        break;

                    default:
                        img[2].gameObject.SetActive(false);
                        break;




                }

 
                text[0].text = d.Username.ToString();
                text[1].text = d.Score.ToString();
                text[2].text = d.Rank.ToString();

            /*

            foreach (var player in result2.Users)
            {
               
 
               StartCoroutine(GetTexture(image, player.AvatarUrl));

  
            }

            */
        }
    }

 
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