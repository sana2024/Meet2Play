using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeepLink : MonoBehaviour
{
    [SerializeField] GameObject challangePanel;
    [SerializeField] GameObject playButton;
    [SerializeField] Transform challangePopupParent;

    public string deeplinkURL;
    public static DeepLink Instance { get; private set; }
    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
 
            Application.deepLinkActivated += onDeepLinkActivated;
          //  Debug.Log("deep link url "+Application.absoluteURL);
            if (!string.IsNullOrEmpty(Application.absoluteURL))
            {
                // Cold start and Application.absoluteURL not null so process Deep Link.
                onDeepLinkActivated(Application.absoluteURL);
            }
            // Initialize DeepLink Manager global variable.
            else deeplinkURL = "[none]";
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void onDeepLinkActivated(string url)
    {
        if (url.Contains("FreindRequest"))
        {
            StartCoroutine(GoToRequests());
        }

        foreach (Transform popup in challangePopupParent)
        {
            Destroy(popup.gameObject);
        }
        if (url.Contains("&"))
        {
        // Update DeepLink Manager global variable, so URL can be accessed from anywhere.
        deeplinkURL = url;
        string PlayerId = url.Split('+')[1];

        var matchId = url.Substring(url.IndexOf("?"), url.IndexOf("+") - url.IndexOf("?"));
        var Match = matchId.Split('?')[1];
         
        var matchWithoutType = Match.Split("&")[0];
        var type = Match.Split("&")[1];

        PassData.ChallangeType = type;

            Notifications.Instance.JoinedPlayers(PlayerId, "Challange Accepted", matchWithoutType);


        }
        else
        {
            var game = url.Split("?");

            if(game[1] == "backgammon")
            {
                Invoke("JoinBackgammonMatch", 3);
            }

            if(game[1].Contains("chess"))
            {
                var chessTime = game[1].Split("@");
                PlayerPrefs.SetInt("time", int.Parse(chessTime[1]));
                SelectChessTime.Instance.AutomaticChange();
                Invoke("JoinChessMatch", 3);
                Debug.Log("the game is chess");
            }
        }


        }


    void JoinBackgammonMatch()
    {

        Matchmaking.Instance.FindBackgammonMatch("paris");

    }

    void JoinChessMatch()
    {
        Matchmaking.Instance.FindChessMatch(100);
    }

    IEnumerator GoToRequests()
    {
        
        yield return new WaitForSeconds(3f);
        FriendSystem.instance.OnFriendButtonClicked();
        FriendSystem.instance.SelectRequests();

    }

}
