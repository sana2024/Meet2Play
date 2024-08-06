using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using Nakama.TinyJson;
using Nakama;
using System;

public class listMatches : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI GameCounter;
    [SerializeField] GameObject GamesPanel;
 

    public static listMatches Instance;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        var mainThread = UnityMainThreadDispatcher.Instance();
        InvokeRepeating("ListActiveMatches", 0.1f, 3);

         
       // PassData.isocket.ReceivedStreamPresence += presenceEvent => mainThread.Enqueue(() => OnRecievedInGameUsers(presenceEvent));
    }


 

    // Update is called once per frame
    void Update()
    {
        
    }


        public async void ListActiveMatches()
    {
    // readMachList();
     var matches =  await UserProfile.instance.client.ListMatchesAsync(UserProfile.instance.session, 2, 2, 100, false, null, null);
        //var totalCount = matches.Matches.Count() + PassData.matchCount;

     
            GameCounter.text = matches.Matches.Count().ToString();

    }

        //public async void InsertIntoMatchList(string count)
        //{
        //    var payload = JsonWriter.ToJson(new { count = count });
        //    await UserProfile.instance.client.RpcAsync(UserProfile.instance.session, "writetomachlist", payload);

        //}

        //public async void readMachList()
        //{
        //    var result1 = await UserProfile.instance.client.ListUsersStorageObjectsAsync(UserProfile.instance.session, "MatchList", "00000000-0000-0000-0000-000000000000", 1);
        //    if (result1.Objects.Any())
        //    {
        //    foreach (var ob in result1.Objects)
        //    {
        //            var datas = JsonParser.FromJson<MatchCount>(ob.Value);
        //            PassData.matchCount = int.Parse(datas.matchCount);
        //    }
        //    }

        //}
    }
