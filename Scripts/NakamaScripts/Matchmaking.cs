using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using System;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;
using System.Threading.Tasks;
using Nakama.TinyJson;
using UnityEngine.Networking;
 

public class Matchmaking : MonoBehaviour
{
    [SerializeField] NakamaConnection Nconnect;
    ISocket isocket;
    IClient iclient;
    ISession isession;
    private string ticket;
    static IMatch currentMatch;
    string OtherId;

    IUserPresence hostPresence;
    IUserPresence SecondPresence;

 

    [SerializeField] GameObject SearchingPanel;
    [SerializeField] UserProfile userProfile;
    [SerializeField] GameObject NoEnoughCoinPanel;
    [SerializeField] GameObject RejectPanel;
    [SerializeField] RawImage OpponentImage;
    [SerializeField] Text OponentUsername;
    [SerializeField] GameObject ProgressBar;
    [SerializeField] GameObject HiddenUser;
    [SerializeField] GameObject PlayButton;
    [SerializeField] Button CancleButton;
 



    // Start is called before the first frame update
    public async void Start()
    {
        isession = PassData.isession;
        iclient = PassData.iClient;
        isocket = PassData.isocket;
        var mainThread = UnityMainThreadDispatcher.Instance();
        isocket.ReceivedMatchmakerMatched += match => mainThread.Enqueue(() => OnREceivedMatchmakerMatched(match));
        isocket.ReceivedMatchPresence += m => mainThread.Enqueue(() => OnRecivedMatchPresence(m ));
        isocket.ReceivedMatchState += m => mainThread.Enqueue(async () => await OnReceivedMatchState(m));


        CancleButton.onClick.AddListener(RemoveTicket);

         

    }

    public async void SendMatchState(long opCode, string state)
    {
        await isocket.SendMatchStateAsync(PassData.Match.Id, opCode, state);
    }

    private async Task OnReceivedMatchState(IMatchState matchState)
    {
        var state = matchState.State.Length > 0 ? System.Text.Encoding.UTF8.GetString(matchState.State).FromJson<Dictionary<string, string>>() : null;

        switch (matchState.OpCode)
        {
            case 16:

                if (state["Leave"] == "Left")
                {
                    SearchingPanel.SetActive(false);
                    StartCoroutine(DisplayRejectPanel());
                    await PassData.isocket.LeaveMatchAsync(PassData.Match.Id);


                    HiddenUser.SetActive(true);
                    ProgressBar.SetActive(true);
                    StartCoroutine(GetTexture(""));
                    PlayButton.SetActive(false);
                    SearchingPanel.SetActive(false);
                }


                if (state["Leave"] == "Join")
                {
                    PassData.JoinedPlayers = 2;
                    Debug.Log("other player joined");
                }


                break;

        }
        }


    IEnumerator DisplayRejectPanel()
    {
        RejectPanel.SetActive(true);
        yield return new WaitForSeconds(3);
        RejectPanel.SetActive(false);
    }

    public async void FindMatch(string BoardName)
    {

        PassData.BoardType = BoardName;

        int wallet = Math.Abs(PassData.WalletMoney);
        int boardPrice = Math.Abs(PassData.BoardPrice);

 
        if (wallet >= boardPrice)
        {
        SearchingPanel.SetActive(true);

        var properites = new Dictionary<string, string>() {
          {"board", BoardName}
         };
        var query = "+properties.board:"+BoardName;

        var matchmakingTickets = await isocket.AddMatchmakerAsync(query, 2, 2, properites);

        ticket = matchmakingTickets.Ticket;

 

 

        }
        else
        {
            StartCoroutine(NoMoneyPanelTimer());
            Debug.Log("you dont have enough money");
        }

    }

    IEnumerator NoMoneyPanelTimer()
    {
        NoEnoughCoinPanel.SetActive(true);
        yield return new WaitForSeconds(2);
        NoEnoughCoinPanel.SetActive(false);
    }
 
 
    public async void RemoveTicket()
    {
        Debug.Log(ticket);
        await isocket.RemoveMatchmakerAsync(ticket);
        SearchingPanel.SetActive(false);

        int boardPrice = Math.Abs(PassData.BoardPrice);
        userProfile.updateWallet(boardPrice);

    }

    private async void OnREceivedMatchmakerMatched(IMatchmakerMatched matchmakerMatched)
    {
        var users = matchmakerMatched.Users;

  


        var match = await isocket.JoinMatchAsync(matchmakerMatched);

 

        hostPresence = matchmakerMatched.Users.OrderBy(x => x.Presence.SessionId).First().Presence;
        SecondPresence = matchmakerMatched.Users.OrderBy(x => x.Presence.SessionId).Last().Presence;

        PassData.hostPresence = hostPresence.UserId;
        PassData.SecondPresence = SecondPresence.UserId;

        Debug.Log("Our Match ID: " + match.Self.SessionId);
        currentMatch = match;
        PassData.Match = currentMatch;
        PlayerPrefs.SetString("matchID",currentMatch.Id);

        PlayerPrefs.SetString("MatchSelf", currentMatch.Self.UserId);
        foreach (var presence in match.Presences)
        {
            Debug.Log("we Joined A match");

            CancleButton.onClick.AddListener(RejectGame);

            if (presence.UserId != match.Self.UserId)
            {
                PassData.OtherUserId = presence.UserId;
                PassData.otherUsername = presence.Username;
 

                var ids = new[] { presence.UserId };
                var result = await iclient.GetUsersAsync(isession,  ids);

                foreach ( var u in result.Users)
                {
                    PassData.OpponentURL = u.AvatarUrl;
                    StartCoroutine(GetTexture(u.AvatarUrl));
                    OponentUsername.text = u.Username;
                }

            }


        }

    }

    private async void OnRecivedMatchPresence(IMatchPresenceEvent matchPresenceEvent)
    {
        foreach (var user in matchPresenceEvent.Joins)
        {

            CancleButton.onClick.AddListener(RejectGame);
 
            var UserId = PlayerPrefs.GetString("MatchSelf");

            if (user.UserId !=  UserId)
            {
                PassData.OtherUserId = user.UserId;
                PassData.otherUsername = user.Username;

                var ids = new[] { user.UserId };
                var result = await iclient.GetUsersAsync(isession, ids);

                foreach (var u in result.Users)
                {
                    PassData.OpponentURL = u.AvatarUrl;
                    StartCoroutine(GetTexture(u.AvatarUrl));
                    OponentUsername.text = u.Username;
                }

            }




        }

    }

    public void AcceptGame()
    {
        var state = MatchDataJson.SetLeaveMatch("Join");
        SendMatchState(OpCodes.Reject_Match, state);
        SceneManager.LoadScene("GameScene");

    }


    public async void RejectGame()
    {


        var state = MatchDataJson.SetLeaveMatch("Left");
        SendMatchState(OpCodes.Reject_Match, state);

        await PassData.isocket.LeaveMatchAsync(PassData.Match.Id);
 

        HiddenUser.SetActive(true);
        ProgressBar.SetActive(true);
        StartCoroutine(GetTexture(""));
        PlayButton.SetActive(false);
        SearchingPanel.SetActive(false);
 
    }


    IEnumerator GetTexture(string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;

            OpponentImage.texture = myTexture;
 
        }

    }







}
