using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using Nakama.TinyJson;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class ChessMatchmaking : MonoBehaviour
{
    [SerializeField] GameObject MatchmakingPanel;
    IUserPresence hostPresence;
    IUserPresence SecondPresence;

    bool enteredWaitingPhase = false;
    float waitingTime = 0;
    string matchid;

    // Start is called before the first frame update
    void Start()
    {
        var mainThread = UnityMainThreadDispatcher.Instance();
        PassData.isocket.ReceivedMatchmakerMatched += match => mainThread.Enqueue(() => OnREceivedMatchmakerMatched(match));
        PassData.isocket.ReceivedMatchPresence += m => mainThread.Enqueue(() => OnRecivedMatchPresence(m));
 

    }


    public async void findMatch(int time)
    {
        PassData.SkillLevel = "beginner";

        enteredWaitingPhase = true;
        var stringProperties = new Dictionary<string, string>() {
    {"skill", PassData.SkillLevel}
};

        var numericProperties = new Dictionary<string, double>() {
    {"time", time}
};
        var query = "+properties.skill:" + PassData.SkillLevel + " +properties.time:" + time;

        await PassData.isocket.AddMatchmakerAsync(query, 2, 2, stringProperties, numericProperties);
        Debug.Log("searching");
    }



    private async void OnRecivedMatchPresence(IMatchPresenceEvent m)
    {
        foreach (var presence in m.Joins)
        {
 

        }
    }

    private async void OnREceivedMatchmakerMatched(IMatchmakerMatched matchmakerMatched)
    {
        var match = await PassData.isocket.JoinMatchAsync(matchmakerMatched);
        matchid = matchmakerMatched.MatchId;
        Debug.Log("Joined match " + match.Id);

        hostPresence = matchmakerMatched.Users.OrderBy(x => x.Presence.SessionId).First().Presence;
        SecondPresence = matchmakerMatched.Users.OrderBy(x => x.Presence.SessionId).Last().Presence;

        PassData.hostPresence = hostPresence.UserId;
        PassData.SecondPresence = SecondPresence.UserId;

        PassData.Match = match;

     
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
