using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using Nakama;
using Nakama.TinyJson;
using System.Threading.Tasks;
using ByteBrewSDK;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;


    //-----------------
    //Player properties
    //-----------------
    public Player playerWhite;
    public Player playerBlack;
    public Player currentPlayer;
    public Player turnPlayer;
    public Player playerWonRound;
    public Player MyPlayer;
    public Player OtherPlayer;


    //-----------------
    //UI elements
    //-----------------
    public Button undoButton;   
    public Button nextTurnButton;
    public Button rollButton;
    public Button doublebutton;
    public Image  firstDiceValueImage;
    public Image secondDiceValueImage;
    [SerializeField] ButtonController buttonController;
    public GameObject gameEndScreen;

    //rounds
    private const int ROUND_LIMIT = 3;
    private int currentRound = 1;

    //nakama components
    ISocket isocket;
    IClient iclient;
    ISession isession;

    //Board
    [SerializeField] GameObject Board;
    [SerializeField] ResizeSlots resizeSlots;
    [SerializeField] GameObject CameraBackground;
    [SerializeField] Text LevelText;
    [SerializeField] GameObject EndGamePanel;
    [SerializeField] public Image EndGameBackground;
    [SerializeField] public GameObject WinnerImage;
    [SerializeField] GameObject losserImage;
    [SerializeField] Sprite RedBackground;
    [SerializeField] public Sprite GreenBackground;
    [SerializeField] Sprite YellowBackground;
    [SerializeField] GameObject GameEndedTexts;


    [SerializeField] PlayerTimer playerTimer;
    [SerializeField] GameObject DoublePanel;
    [SerializeField] GameObject AcceptedPanel;
    [SerializeField] HelloVideoAgora VideoAgora;
    [SerializeField] Bet bet;

    [SerializeField] Text MyScoreText;
    [SerializeField] Text OponentScoreText;

    [SerializeField] public GameObject Reward;

    [SerializeField] InGameData inGameData;
    [SerializeField] Image MyChecker;
    [SerializeField] Image OponentChecker;
    [SerializeField] Sprite WhiteChecker;
    [SerializeField] Sprite BlackChecker;

    [SerializeField] GameObject RejectBetPanel;
    [SerializeField] GameObject RejectGamePanel;
    [SerializeField] GameObject NoMoveExistsPanel;
    [SerializeField] GameObject RejectReplayPanel;
    [SerializeField] GameObject OfflineOpponent;
    [SerializeField] GameObject UserOffline;
    [SerializeField] Text TimeoutDEbugger;
    [SerializeField] GameObject WeakNetworkPanel;
    [SerializeField] GameObject NoCoinBetText;
    [SerializeField] Button AcceptBetButton;
    [SerializeField] Text RematchUsername;

    public List<string> betTurns = new List<string>();


    public bool RecievedEndGame = false;
    bool InternetConnected;
    bool ReconnectFlag = false;
    public bool ReconnectSocket = false;

    [SerializeField] Slot BlackOutside;
    [SerializeField] Slot WhiteOutside;


    //Auto Roll 
    bool AutoRollDice = false;

    [SerializeField] Button AutoRollDiceButon;
    [SerializeField] Sprite ToggleOn;
    [SerializeField] Sprite ToggleOff;

    string AutoRollDiceActive;

    //play agaian
    [SerializeField] Button PlayAgainButton;
    [SerializeField] GameObject PlayAgain;
    HelloVideoAgora DisplaYVideo;



    float timeToEndGame = 0;

    bool OtherOfflineTrue = false;



    //Others
    int RollCounters = 1;


    

    
    #region Unity API

    private void Awake()
    {
 
 
        ISocketAdapter adatper;
       
        if (instance == null)
            instance = this;

        playerWhite = new Player { id = 0, pieceType = PieceType.White , UserId = PassData.hostPresence};
        playerBlack = new Player { id = 1, pieceType = PieceType.Black , UserId = PassData.SecondPresence};

 
        nextTurnButton.onClick.AddListener(OnNextTurnButtonClick);
        undoButton.onClick.AddListener(UndoPiece);
        rollButton.onClick.AddListener(RollDices);


        if(PassData.Match.Self.UserId == playerBlack.UserId)
        {
            resizeSlots.rotate();
            MyPlayer = playerBlack;
            OtherPlayer = playerWhite;

           

        }
        if (PassData.Match.Self.UserId == playerWhite.UserId)
        {
            MyPlayer = playerWhite;
            OtherPlayer = playerBlack;
        }
 
    }

 

    private void Start()
    {
        ByteBrew.NewCustomEvent("StartedGame", "type=Backgammon; Username=" + PassData.isession.Username + ";");
        InvokeRepeating("CheckOpponentAailabilty", 0.1f, 1);
        InvokeRepeating("OfflineReconnectAsync", 0.1f, 2);

        LevelText.text = PassData.RecivedLevel;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
 
        isocket = PassData.isocket;
        iclient = PassData.iClient;
        isession = PassData.isession;

        var mainThread = UnityMainThreadDispatcher.Instance();
        isocket.ReceivedMatchState += m => mainThread.Enqueue(async () => await OnReceivedMatchState(m));
        isocket.ReceivedMatchPresence += m => mainThread.Enqueue(() => OnRecivedMatchPresence(m));

        isocket.Closed += () => Connect();
    
        isocket.ReceivedError += e => Debug.Log("Socket error: " + e.Message);

        isocket.ReceivedStreamPresence += presenceEvent =>
        {
            foreach (var joined in presenceEvent.Joins)
            {
                Debug.Log("user joined");
            }
            foreach (var left in presenceEvent.Leaves)
            {
                Debug.Log("user leaved");
            }
        };

        isocket.ReceivedStreamState += stream =>
        {
            Debug.Log("stream state");
        };

 
        HideGameEndScreen();

        if (PassData.IsFirstRound == true)
        {
            currentPlayer = playerWhite;
            turnPlayer = currentPlayer;

        }
        else
        {
            currentPlayer = PassData.PlayerWonRound;
            turnPlayer = currentPlayer;
        }
   

        HideDiceValues();

        if (currentPlayer.UserId == PassData.Match.Self.UserId)
        {

            buttonController.EnableRollButton();

        }

        if(playerWhite.UserId == PassData.Match.Self.UserId)
        {
            MyChecker.sprite = WhiteChecker;
            OponentChecker.sprite = BlackChecker;

        } 
        else
        {
            MyChecker.sprite = BlackChecker;
            OponentChecker.sprite = WhiteChecker;
        }

      
    }

    private async void OnRecivedMatchPresence(IMatchPresenceEvent matchPresenceEvent)
    {
       
    }
    

#if !UNITY_EDITOR
    private void OnApplicationFocus(bool focus)
    {
        Connect();
    }
#endif
  

    async void OfflineReconnectAsync()
    {
        var account = await iclient.GetAccountAsync(isession);
        var user = account.User;

        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            if (user.Online == false)
            {
                UserOffline.SetActive(true);
                Debug.Log("user offline reconnection");
                Connect();
            }

        }
    }

    async void CheckOpponentAailabilty()
    {
        var ids = new[] { PassData.OtherUserId};
        var result = await iclient.GetUsersAsync(isession, ids);

        foreach(var user in result.Users)
        {
            if (user.Online)
            {
                OfflineOpponent.SetActive(false);
                timeToEndGame = 0;
            }

            if(user.Online == false)
            {
                OfflineOpponent.SetActive(true);
            }
        }

       
    }

    

    public async void SendMatchState(long opCode, string state)
    {
 
        try
        {
            Debug.Log(PassData.OtherPresence.Username);
            await isocket.SendMatchStateAsync(PassData.Match.Id, opCode, state, new[] { PassData.OtherPresence });
        }catch(Nakama.ApiResponseException ex)
        {
            Debug.Log("send game manager " + ex);
        }
    }


    private async Task OnReceivedMatchState(IMatchState matchState)
    {

        
        var state = matchState.State.Length > 0 ? System.Text.Encoding.UTF8.GetString(matchState.State).FromJson<Dictionary<string, string>>() : null;

        switch (matchState.OpCode)
        {
            case 5:

           ShowDiceValues();


                break;

            case 6:

                DiceController.instance.HideRollingDices();
                

                MoveClick.instance.endTurn();

                Debug.Log("player "+MoveClick.instance.player);

                 buttonController.EnableRollButton();

                 

                if (RollCounters > 1)
                {
                 buttonController.EnableDoubleButton();
                }
 

                if(state["Current_Player"] == "Black")
                {
                    currentPlayer = playerBlack;
                    turnPlayer = currentPlayer;



                }
                if (state["Current_Player"] == "Black" && AutoRollDiceActive == "True")
                {
                    MoveClick.instance.player = (MoveClick.instance.player + 1) % 2;
                    currentPlayer = playerBlack;
                    turnPlayer = currentPlayer;


                }

                if (state["Current_Player"] == "White")
                {
                    currentPlayer = playerWhite;
                    turnPlayer = currentPlayer;

                    Debug.Log(currentPlayer.id + " " + currentPlayer.UserId + " " + currentPlayer.pieceType);
                }

                if (state["Current_Player"] == "White" && AutoRollDiceActive == "True")
                {
                    MoveClick.instance.player = (MoveClick.instance.player + 1) % 2;
                    currentPlayer = playerWhite;
                    turnPlayer = currentPlayer;
 
                }

                break;

            case 7:
                
                GameObject pieceOb = GameObject.Find(state["PeiceID"]);
                Piece piece = pieceOb.GetComponent<Piece>();

                GameObject from = GameObject.Find(state["From"]);
                Slot FromSlot = from.GetComponent<Slot>();

                FromSlot.pieces.Remove(piece);

                GameObject to = GameObject.Find(state["To"]);
                Slot ToSlot = to.GetComponent<Slot>();

                int steps = int.Parse(state["Steps"]);

                MoveActionTypes ActionType = (MoveActionTypes)Enum.Parse(typeof(MoveActionTypes), state["ActionType"]);

               currentPlayer.movesPlayed.Add(new Move
                {
                    piece = piece,
                    from = FromSlot,
                    to = ToSlot,
                    step = steps,
                    action = ActionType,
                });


                var lastMove = currentPlayer.movesPlayed.Last();

                var PieceSprite = lastMove.piece.gameObject.GetComponent<SpriteRenderer>();


                ConvertPieceOutside.instance.FromOutToSlot(lastMove.piece);
                     piece.IncreaseColliderRadius();
                

                lastMove.piece.PlaceOn(lastMove.from);

                // undo hit action
                if ((lastMove.action & MoveActionTypes.Hit) == MoveActionTypes.Hit)
                {
                    var enemyBar = Slot.GetBar(Piece.GetEnemyType(lastMove.piece.pieceType));
                    var enemyPiece = enemyBar.pieces.Last();
                    enemyPiece.PlaceOn(lastMove.to);
                }

                currentPlayer.movesPlayed.Remove(lastMove);

                if (lastMove.piece.pieceType == PieceType.Black)
                {
                    PieceSprite.sprite = BlackChecker;
                }
                if (lastMove.piece.pieceType == PieceType.White)
                {
                    PieceSprite.sprite = WhiteChecker;
                }


                break;


             


            case 9:

                if (state["Camera_Background"] == "True"){

                    CameraBackground.SetActive(true);
                }


                if (state["Camera_Background"] == "False")
                {

                    CameraBackground.SetActive(false);
                }

                break;

 

            case 11:
                Debug.Log("recived level");
                LevelText.text = state["Level"];

                break;



            case 12:

                if (state["Double"] == "true")
                {
                    Debug.Log("user sent double request");
                    betTurns.Add("opponent");
                    DoublePanel.SetActive(true);

                    if (PassData.WalletMoney >= Bet.Instance.nextBetAmount)
                    {
                    }
                    else
                    {
                        NoCoinBetText.SetActive(true);
                        AcceptBetButton.interactable = false;

                    }

                }
                break;


            case 13:

                StartCoroutine(ShowAcceptPanel());
                bet.IncreaseBet();

                break;


            case 14:

                await PassData.isocket.LeaveMatchAsync(PassData.Match.Id);
                StartCoroutine(ShowRejectBetPanel());
                VideoAgora.OnApplicationQuit();


                break;


            case 15:

                RecievedEndGame = true;
                Debug.Log("other player left the game");
                gameEndScreen.SetActive(true);
                WinnerImage.SetActive(true);
                EndGameBackground.sprite = GreenBackground;
                playerWonRound = MyPlayer;
                Reward.SetActive(true);
                inGameData.updateWallet(PassData.betAmount , 25);
                InGameData.Instance.UpdateXP(25);
                PassData.wins++;
                inGameData.WriteWinsAndLosses(PassData.level, PassData.wins, PassData.losses);
                playerTimer.GameEnded();
                VideoAgora.OnApplicationQuit();
                ByteBrew.NewCustomEvent("Won", "Game=Backgammon; Type=; Username=" + isession.Username + ";");
                AppFlyer.Instance.AppsFlyerEvent("Won_Backgammon_Online", isession.Username);

                break;


            case 16:

                if (state["Leave"] == "Left")
                {

                    StartCoroutine(ShowRejectGamePanel());

                }
                 

                    break;


            case 19:

                if (state["RequsetRematch"] == "RequestReplay")
                {
                    StartCoroutine(ShowPlayAgainRequest());
                }

                if (state["RequsetRematch"] == "RejectPlayAgain")
                {
                    ShowRejectReplay();
                }

                if (state["RequsetRematch"] == "AcceptReplay")
                {
                    OnNextRoundButtonClick();
                }

             


                break;

        }


    }

    // enumerator functions 

    IEnumerator ShowAcceptPanel()
    {
        AcceptedPanel.SetActive(true);
        yield return new WaitForSeconds(3);
        AcceptedPanel.SetActive(false);
    }


    IEnumerator ShowRejectBetPanel()
    {
        RejectBetPanel.SetActive(true);
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("Menu");
    }

    IEnumerator ShowRejectGamePanel()
    {
        RejectGamePanel.SetActive(true);
        yield return new WaitForSeconds(3);
        VideoAgora.OnApplicationQuit();
        SceneManager.LoadScene("Menu");
    }

    public void ShowRejectReplay()
    {
        RejectReplayPanel.SetActive(true);
    }

    public IEnumerator ShowNoMovePanel()
    {
        NoMoveExistsPanel.SetActive(true);
        yield return new WaitForSeconds(2);
        NoMoveExistsPanel.SetActive(false);
    }
 
    private async void Connect()
    {
        try
        {      
                var keepAliveIntervalSec = 30;
                await isocket.ConnectAsync(isession, true, keepAliveIntervalSec);
                PassData.isocket = isocket;
                SystemSettings.instance.ConnectionPanel.SetActive(false);
                Debug.Log("socket reconnected ");

                if (isocket.IsConnected)
                {
                     await isocket.JoinMatchAsync(PassData.Match.Id);
                    Debug.Log("socket reconnected ");
                UserOffline.SetActive(false);
 
                }
            }


           catch (TaskCanceledException e) {
        
            Debug.Log("task canceled "+e.Task);
            var retryConfiguration = new Nakama.RetryConfiguration(1, 5, delegate { });

            // Configure the retry configuration globally.
            iclient.GlobalRetryConfiguration = retryConfiguration;
            await isocket.JoinMatchAsync(PassData.Match.Id);

        }
    }

    public void InVokeConnect()
    {
        if(Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
        {
            Debug.Log("user is using Mobile Data");
            Invoke("Connect", 10);
        }

        if(Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
        {
            Debug.Log("user is using WIFI");
        }

    }

    private void Update()
    {
        if (betTurns.Any())
        {
            Debug.Log("bet turn count "+betTurns.Count);
            Debug.Log("turn "+ betTurns.Last());
            if (betTurns.Last() == "me")
            {
                Debug.Log("was my turn ");
                doublebutton.gameObject.SetActive(false);
            }
         
        }
 
         
        if (HelloVideoAgora.instance.NetworkQuality == 5)
        {
            WeakNetworkPanel.SetActive(true);
        }
        else
        {
            WeakNetworkPanel.SetActive(false);
        }

        if (OfflineOpponent.activeSelf)
        {
            WeakNetworkPanel.SetActive(false);
            timeToEndGame += Time.deltaTime;

            if (timeToEndGame > 50)
            {
                HelloVideoAgora.instance.OnApplicationQuit();
                InGameData.Instance.updateWallet(PassData.betAmount , 25);
                EndGamePanel.SetActive(true);
                EndGameBackground.sprite = GreenBackground;
                WinnerImage.SetActive(true);
                GameEndedTexts.SetActive(true);
            }
        }

        if (SystemSettings.instance.ConnectionPanel.activeSelf)
        {
            timeToEndGame += Time.deltaTime;

            if (timeToEndGame > 50)
            {
                HelloVideoAgora.instance.OnApplicationQuit();
                EndGamePanel.SetActive(true);
                EndGameBackground.sprite = RedBackground;
                losserImage.SetActive(true);
                GameEndedTexts.SetActive(true);

            }
        }


        if (UserOffline.activeSelf)
        {
            SystemSettings.instance.ConnectionPanel.SetActive(false);
            OfflineOpponent.SetActive(false);
            WeakNetworkPanel.SetActive(false);

            timeToEndGame += Time.deltaTime;

            if (timeToEndGame > 50)
            {
                losserImage.SetActive(true);
                HelloVideoAgora.instance.OnApplicationQuit();
                EndGamePanel.SetActive(true);
                EndGameBackground.sprite = RedBackground;
                GameEndedTexts.SetActive(true);

            }
        }


        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            ReconnectFlag = true;
        }
        else
        {
            if (ReconnectFlag == true)
            {
                Debug.Log("reconnected ");
                Connect();
                ReconnectFlag = false;
            }


        }

        if (currentPlayer.id == 0)
        {
            MoveClick.instance.player = 1;
        }
        if (currentPlayer.id == 1)
        {
            MoveClick.instance.player = 0;
        }
 

        if (currentPlayer.UserId == PassData.Match.Self.UserId)
        {
          // AutoRollDiceActive = PlayerPrefs.GetString("AutoRollDice");


            if (AutoRollDiceActive == "True")
            {
                Debug.Log("auto roll");

                AutoRollDice = true;

                RollDices();
                buttonController.DissableRollButton();
                buttonController.EnableUndoButton();
                buttonController.DisableDoubleButton();


            }
        }

            if (DiceController.instance.animationStarted && !DiceController.instance.animationFinished)
        {
            ShowDiceValues();
        }

        if (turnPlayer.IsMoveLeft()&& MoveClick.instance.NoMovePass == false || turnPlayer.rolledDice == false)
        {
 
            nextTurnButton.gameObject.SetActive(false);
             
        }
        else
        {
            nextTurnButton.gameObject.SetActive(true);

        }
    
 
    }

    private void FixedUpdate()
    {
        if (turnPlayer.UserId == PassData.Match.Self.UserId)
        {
            playerTimer.OtherPlayerTimer.fillAmount = 1;
            playerTimer.playerTimer();

        }
        else
        {
            playerTimer.OponentTimer();
        }
    }

#endregion

#region UI
    private const string UI_TEXT_ROUND = "RoundText";
    private const string UI_PANEL_SCORE = "GameScore";
    private const string UI_PANEL_SCORE_PLAYER_WHITE = "PlayerWhite";
    private const string UI_PANEL_SCORE_PLAYER_BLACK = "PlayerBlack";
    private const string UI_TEXT_SCORE = "Score";
    private const string UI_BUTTON_NEXT_ROUND = "NextRoundButton";

    private void OnNextTurnButtonClick()
    {
        if (!turnPlayer.rolledDice)
        {
            Debug.Log("you have to roll the dice");
 
            return;
        }

        if (turnPlayer.IsMoveLeft() && MoveClick.instance.NoMovePass == false)
        {
 
            Debug.Log("You have to move");
            return;
        }

 
        playerTimer.restart();

        NextTurn();
    }
 
    private void UpdateGameEndScreen()
    {

        if (IsAnyPlayerWon())
        {
            if(playerWonRound.UserId == PassData.Match.Self.UserId)
            {
                EndGamePanel.SetActive(true);
 
            }
            if(playerWonRound.UserId != PassData.Match.Self.UserId)
            {
                EndGamePanel.SetActive(true);
 

            }
        }

    }

    private bool IsAnyPlayerWon()
    {
        var potentialWeight = (ROUND_LIMIT - currentRound) * 2;

        // if round is equal to limit
        if (currentRound == ROUND_LIMIT)
            return true;

        // if white player is winning, 
        // and if black player + potential still less than white player,
        // white player won
        if (playerWhite.score > playerBlack.score &&
            playerBlack.score + potentialWeight < playerWhite.score)
            return true;

        // if black player is winning, 
        // and if white player + potential still less than black player,
        // black player won
        if (playerBlack.score > playerWhite.score &&
            playerWhite.score + potentialWeight < playerBlack.score)
            return true;

        return false;
    }

    private void ShowGameEndScreen()
    {
        // update game end screen
        UpdateGameEndScreen();

        // enable game end screen
        gameEndScreen.SetActive(true);
    }

    private void HideGameEndScreen()
    {
        // disable game end screen
        gameEndScreen.SetActive(false);
    }

    private void RollDices()
    {

         if(currentPlayer.UserId == PassData.Match.Self.UserId)
            {
        if (!currentPlayer.rolledDice)
        {
            RollCounters ++;
            DiceController.instance.ThrowDices();
            currentPlayer.rolledDice = true;
            StartCoroutine(AfterRolledDice());

                buttonController.EnableDoneButton();
                buttonController.EnableUndoButton();
            }
     
 
   
        }

              }
 
    private IEnumerator AfterRolledDice()
    {
         nextTurnButton.gameObject.SetActive(false);

        if (!currentPlayer.IsMoveLeft())
        {
            NoMoveExistsPanel.SetActive(true);
            buttonController.DissableDoneButton();
            buttonController.DisableDoubleButton();
            buttonController.DisableUndoButton();
            MoveClick.instance.endTurn();
            OnNextTurnButtonClick();
        }
        else
        {
            nextTurnButton.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(1.5f);

        NoMoveExistsPanel.SetActive(false);

    }
    //this functions manages the dice values on the UI (right corner of the screen
    private void HideDiceValues()
    {
        firstDiceValueImage.gameObject.SetActive(false);
        secondDiceValueImage.gameObject.SetActive(false);
    }

    private void ShowDiceValues()
    {
        firstDiceValueImage.gameObject.SetActive(true);
        secondDiceValueImage.gameObject.SetActive(true);

        firstDiceValueImage.sprite = DiceController.instance.firstValueSprite;
        secondDiceValueImage.sprite = DiceController.instance.secondValueSprite;

       

    }
 
#endregion

#region Public

    public async Task CheckRoundFinishAsync()
    {
        if (IsFinished())
        {
            var score = CalculateScore();
            MalsUserBet();

            Debug.Log("score " + score);
            // increment won round of player
            playerWonRound.score += score;

            ByteBrew.NewCustomEvent("FinishedGame", "Username=" + PassData.isession.Username + ";");

            if (playerWonRound.UserId == PassData.Match.Self.UserId)
            {
                ByteBrew.NewCustomEvent("Won", "Game=Backgammon; Type=; Username=" + isession.Username + ";");
                AppFlyer.Instance.AppsFlyerEvent("Won_Backgammon_Online", isession.Username);
                WinnerImage.SetActive(true);
                EndGameBackground.sprite = GreenBackground;
                MyScoreText.text = score.ToString();
                Reward.SetActive(true);
                inGameData.updateWallet(PassData.betAmount , 25);
                VideoAgora.OnApplicationQuit();
                playerTimer.GameEnded();

                PassData.wins++;

                inGameData.WriteWinsAndLosses(PassData.level , PassData.wins , PassData.losses);
               // InGameData.Instance.UpdateXP(25);
            }
            else
            {
                ByteBrew.NewCustomEvent("Lost", "Game=Backgammon; Type=; Username=" + isession.Username + ";");
                AppFlyer.Instance.AppsFlyerEvent("Lost_Backgammon_Online", isession.Username);
                losserImage.SetActive(true);
                EndGameBackground.sprite = RedBackground;
                OponentScoreText.text = score.ToString();
                VideoAgora.OnApplicationQuit();
                playerTimer.GameEnded();

                inGameData.updateWallet(- PassData.betAmount, 25);
                PassData.losses++;
                inGameData.WriteWinsAndLosses(PassData.level, PassData.wins, PassData.losses);
            }

                ShowGameEndScreen();
        }
    }
 
    public void MalsUserBet()
    {
        var enemyOutside = (Piece.GetEnemyType(playerWonRound.pieceType) == PieceType.White) ?
        BoardManager.instance.whiteOutside.GetComponent<Slot>() :
        BoardManager.instance.blackOutside.GetComponent<Slot>();

        if(enemyOutside.pieces.Count == 0)
        {
            bet.IncreaseBet();
        }
    }




#endregion

    private int CalculateScore()
    {
        var enemyOutside = (Piece.GetEnemyType(playerWonRound.pieceType) == PieceType.White) ?
            BoardManager.instance.whiteOutside.GetComponent<Slot>() :
            BoardManager.instance.blackOutside.GetComponent<Slot>();

        return (enemyOutside.pieces.Count == 0) ? 2 : 1;
    }

    public void NextTurn()
    {
       
         foreach(Slot slot in MoveClick.instance.slots)
        {
            foreach(Piece piece in slot.pieces)
            {
                piece.PieceHistory.Clear();
            }
        }

        DiceController.instance.HideRollingDices();

        //--------------------------------
        // reset current player's fields
        //--------------------------------
        // flush moves log

        Debug.Log("moves played " + turnPlayer.movesPlayed.Count);
        turnPlayer.movesPlayed.Clear();

        // reset dice
        ResetDice();
        //--------------------------------
        // turn the set to the next player
        //--------------------------------
        if (turnPlayer.pieceType == PieceType.White)
        {
            turnPlayer = playerBlack;
            currentPlayer = turnPlayer;

            Debug.Log(currentPlayer.id + " "+currentPlayer.UserId+" "+currentPlayer.pieceType);

            var state = MatchDataJson.SetCurrentPlayer("Black");
              SendMatchState(OpCodes.current_player, state);

            if(HelloVideoAgora.instance.NetworkQuality == 5 || HelloVideoAgora.instance.NetworkQuality == 4 || HelloVideoAgora.instance.NetworkQuality == 3 || HelloVideoAgora.instance.NetworkQuality == 6 || OfflineOpponent.activeSelf || UserOffline.activeSelf)
            {
                for(int i =0; i < 3; i++)
                {
                    Debug.Log("retry send");
                    SendMatchState(OpCodes.current_player, state);
                }
            }
                MoveClick.instance.NoMovePass = false;


            return;
        }
        if (turnPlayer.pieceType == PieceType.Black)
        {
            playerWhite.UserId = PassData.hostPresence;
            turnPlayer = playerWhite;
            currentPlayer = turnPlayer;

 
            Debug.Log(currentPlayer.id + " " + currentPlayer.UserId + " " + currentPlayer.pieceType);

            var state = MatchDataJson.SetCurrentPlayer("White");
            SendMatchState(OpCodes.current_player, state);

            if (HelloVideoAgora.instance.NetworkQuality == 5 || HelloVideoAgora.instance.NetworkQuality == 4 || HelloVideoAgora.instance.NetworkQuality == 3 || HelloVideoAgora.instance.NetworkQuality == 6 || OfflineOpponent.activeSelf || UserOffline.activeSelf)
            {
                for (int i = 0; i < 3; i++)
                {
                    Debug.Log("retry send");
                    SendMatchState(OpCodes.current_player, state);
                }
            }

            MoveClick.instance.NoMovePass = false;

            return;
        }

 
 

    }

    private bool IsFinished()
    {
        var whiteFinished = Slot.GetOutside(PieceType.White).pieces.Count == 15;
        var blackFinished = Slot.GetOutside(PieceType.Black).pieces.Count == 15;

        if (whiteFinished)
            playerWonRound = playerWhite;
            

        if (blackFinished)
            playerWonRound = playerBlack;

        if (whiteFinished || blackFinished)
            return true;

        return false;
    }

    private void RestartBoard()
    {
        ResetDice();

        BoardManager.instance.ResetBoard();

        // reset pieces
        BoardManager.instance.PlacePiecesOnBoard();
    }

    private void ResetDice()
    {
        turnPlayer.rolledDice = false;
        HideDiceValues();
    }

    private void UndoPiece()
    {
        Debug.Log("piece history " + currentPlayer.movesPlayed.Last().piece.PieceHistory.Last());

            if (currentPlayer.movesPlayed.Last().piece.PieceHistory.Last() == 'd')
            {
              currentPlayer.movesPlayed.Last().piece.MovedWithClick = false;
                if (currentPlayer.movesPlayed.Count == 0)
                {
                    Debug.Log("You must have played a move for undo");
                    return;
                }

                var lastMove = currentPlayer.movesPlayed.Last();

                if (lastMove.step == MoveClick.instance.curMoves[0])
                {
                    MoveClick.instance.smallDieWasUsed = false;
                }

                if (lastMove.step == MoveClick.instance.curMoves[1])
                {
                    MoveClick.instance.bigDieWasUsed = false;
                }

                Debug.Log("last move step " + lastMove.step);

                // undo move action
                lastMove.piece.PlaceOn(lastMove.from);

                Debug.Log(lastMove.from.ToString());
                var state = MatchDataJson.SetPieceStack(lastMove.piece.name, lastMove.from.name, lastMove.to.name, lastMove.step.ToString(), lastMove.action.ToString(), "undo");
                SendMatchState(OpCodes.undo, state);

                // undo hit action
                if ((lastMove.action & MoveActionTypes.Hit) == MoveActionTypes.Hit)
                {
                    var enemyBar = Slot.GetBar(Piece.GetEnemyType(lastMove.piece.pieceType));
                    var enemyPiece = enemyBar.pieces.Last();
                    enemyPiece.PlaceOn(lastMove.to);
                }

                //undo bear action
             //   lastMove.piece.index -= 0.15f;


                currentPlayer.movesPlayed.Last().piece.PieceHistory.Remove(currentPlayer.movesPlayed.Last().piece.PieceHistory.Last());

                currentPlayer.movesPlayed.Remove(lastMove);



            ConvertPieceOutside.instance.FromOutToSlot(lastMove.piece);
                lastMove.piece.IncreaseColliderRadius();

            return;

            }

        if (currentPlayer.movesPlayed.Last().piece.PieceHistory.Last() == 'c')
        {
            currentPlayer.movesPlayed.Last().piece.PieceHistory.Remove(currentPlayer.movesPlayed.Last().piece.PieceHistory.Last());
            MoveClick.instance.undo();
 
        } 

        

    }

    public void AutoRoll()
    {
        AutoRollDice = !AutoRollDice;
        PlayerPrefs.SetString("AutoRollDice", AutoRollDice.ToString());

        AutoRollDiceActive = PlayerPrefs.GetString("AutoRollDice");
        if (AutoRollDiceActive == "True")
        {

            AutoRollDiceButon.image.sprite = ToggleOn;
           // buttonController.DissableRollButton();
             // RollDices();
            // Debug.Log("AutoRolled");
        }

        if (AutoRollDiceActive == "False")
        {

            AutoRollDiceButon.image.sprite = ToggleOff;
            // buttonController.EnableRollButton();


        }

    }

    

    public void OnNextRoundButtonClick()
    {
        PassData.IsFirstRound = false;
        SceneManager.LoadScene(Constants.SCENE_GAME);

        // increment current round
        currentRound++;

        // reset players
        Player.ResetForNextRound(playerWhite);
        Player.ResetForNextRound(playerBlack);

        // who wins the round starts first
        PassData.PlayerWonRound = playerWonRound;
 
            HelloVideoAgora.instance.InitEngine();
            HelloVideoAgora.instance.JoinChannel();
            inGameData.updateWallet(PassData.BoardPrice , 0);
            Debug.Log("turn player " + playerWonRound);
            RestartBoard();
            HideGameEndScreen();
            playerTimer.restart();
           ChangeBoard.instance.changeBoard();
        ByteBrew.NewCustomEvent("rematch", "Game=backgammon; Type=Online; Username=" + isession.Username + ";");
    }

    public void SendPlay_AgainRequest()
    {

        var state = MatchDataJson.SetRematch("RequestReplay");
        SendMatchState(OpCodes.Play_Again, state);
        StartCoroutine(PlayAgainButtonClicked());
    }
 

    IEnumerator ShowPlayAgainRequest()
    {
        RematchUsername.text = PassData.otherUsername;
        PlayAgain.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);


    }

    //this is for desabiling and enabling the play again button as an effect 
    IEnumerator PlayAgainButtonClicked()
    {
        PlayAgainButton.interactable = false;
        yield return new WaitForSeconds(3);
        PlayAgainButton.interactable = false;
    }






}
