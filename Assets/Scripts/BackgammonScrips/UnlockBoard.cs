using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Nakama;
using Nakama.TinyJson;

public class UnlockBoard : MonoBehaviour
{
    [SerializeField] UserProfile userProfile;
    [SerializeField] Button BoardButton;
    [SerializeField] GameObject LockImage;

    IClient client;
    ISession session;

    int level;
    int wins;
    public string BoardName;




    public int Boardlevel;


    public void Start()
    {
        client = PassData.iClient;
        session = PassData.isession;
        wins = PassData.wins;

       // MatchLists();
        
    }

    // Start is called before the first frame update
    void Update()
    {

        if (PassData.ChessLevel >= Boardlevel)
        {
            BoardButton.interactable = true;
            LockImage.SetActive(false);
    }


}

    // list matches that is playing on this board
    public async void MatchLists()
    {
        var minPlayers = 2;
        var maxPlayers = 2;
        var limit = 2;
        var authoritative = true;
        
        var query = "+properties.board:" + BoardName;
        var result = await client.ListMatchesAsync(session, minPlayers, maxPlayers, limit, authoritative, null, query);

        foreach (var match in result.Matches)
        {
           
        }
    }


}


