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
using Nakama.Ninja.WebSockets;

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


    //-----------------
    //UI elements
    //-----------------
    public Button undoButton;   
    public Button nextTurnButton;
    public Button rollButton;
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
    [SerializeField] Image EndGameBackground;
    [SerializeField] GameObject WinnerImage;
    [SerializeField] GameObject losserImage;
    [SerializeField] Sprite RedBackground;
    [SerializeField] Sprite GreenBackground;
 
    [SerializeField] PlayerTimer playerTimer;
    [SerializeField] GameObject DoublePanel;
    [SerializeField] GameObject AcceptedPanel;
    [SerializeField] HelloVideoAgora VideoAgora;
    [SerializeField] Bet bet;

    [SerializeField] Text MyScoreText;
    [SerializeField] Text OponentScoreText;

    [SerializeField] GameObject Reward;

    [SerializeField] InGameData inGameData;
    [SerializeField] Image MyChecker;
    [SerializeField] Image OponentChecker;
    [SerializeField] Sprite WhiteChecker;
    [SerializeField] Sprite BlackChecker;

    [SerializeField] GameObject RejectBetPanel;
    [SerializeField] GameObject RejectGamePanel;
    [SerializeField] GameObject NoMoveExistsPanel;

    [SerializeField] Text TimeoutDEbugger;

    bool InternetConnected;
    bool ReconnectFlag = false;
    public bool ReconnectSocket = false;

    [SerializeField] Slot BlackOutside;
    [SerializeField] Slot WhiteOutside;




    //Others
    int RollCounters = 1;


    #region Unity API

    private void Awake()
    {
 
      
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

           

        }
        if (PassData.Match.Self.UserId == playerWhite.UserId)
        {
            MyPlayer = playerWhite;
        }
 
    }

 

    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

  


       
       

        isocket = PassData.isocket;
        iclient = PassData.iClient;
        isession = PassData.isession;

        var mainThread = UnityMainThreadDispatcher.Instance();
        isocket.ReceivedMatchState += m => mainThread.Enqueue(async () => await OnReceivedMatchState(m));

        HideGameEndScreen();
 
        currentPlayer = playerWhite;
        turnPlayer = currentPlayer;
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

    public async void SendMatchState(long opCode, string state)
    {
        await isocket.SendMatchStateAsync(PassData.Match.Id, opCode, state);
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

                 buttonController.EnableRollButton();
                 

                if (RollCounters > 1)
                {
                 buttonController.EnableDoubleButton();
                }
 
                Debug.Log(state["Current_Player"]);

                if(state["Current_Player"] == "Black")
                {
                    currentPlayer = playerBlack;
                    turnPlayer = currentPlayer;

                    Debug.Log(currentPlayer.id + " " + currentPlayer.UserId + " " + currentPlayer.pieceType);

                }

                if (state["Current_Player"] == "White")
                {
                    currentPlayer = playerWhite;
                    turnPlayer = currentPlayer;

                    Debug.Log(currentPlayer.id + " " + currentPlayer.UserId + " " + currentPlayer.pieceType);
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


            case 10:

              int time=  int.Parse(state["Timer"]);
                Debug.Log(time);

                break;


            case 11:

                LevelText.text = state["Level"];

                break;



            case 12:

                if(state["Double"]== "true")
                {
                    DoublePanel.SetActive(true);
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

                Debug.Log("other player left the game");
                gameEndScreen.SetActive(true);
                WinnerImage.SetActive(true);
                EndGameBackground.sprite = GreenBackground;
                Reward.SetActive(true);
                inGameData.updateWallet(PassData.betAmount);
                PassData.wins++;
                inGameData.WriteWinsAndLosses(PassData.level, PassData.wins, PassData.losses);
                playerTimer.GameEnded();
                VideoAgora.OnApplicationQuit();

                break;


            case 16:

                if (state["Leave"] == "Left")
                {

                    StartCoroutine(ShowRejectGamePanel());

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

   public IEnumerator ShowNoMovePanel()
    {
        NoMoveExistsPanel.SetActive(true);
        yield return new WaitForSeconds(2);
        NoMoveExistsPanel.SetActive(false);
    }

    IEnumerator Reconnect()
    {
        yield return new WaitForSeconds(3);

        CreateNewSocket();
    }

    public async void CreateNewSocket()
    {
        if (!isession.IsExpired)
        {
            Debug.Log("reconnected");
            isocket = iclient.NewSocket();
            int connectionTimeout = 30;
            await isocket.ConnectAsync(isession, true, connectionTimeout);
            PassData.isocket = isocket;

            if (isocket.IsConnected || isocket != null)
            {
                await isocket.JoinMatchAsync(PassData.Match.Id);
                ReconnectSocket = true;
                Debug.Log("re joined the match");

                Debug.Log("my id" + PassData.Match.Self.UserId);
            }

       
        }
    }


    private async void Update()
    {

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("internet dissconected");
            ReconnectFlag = true;
        }
        else
        {
            if (ReconnectFlag == true)
            {
                StartCoroutine(Reconnect());
                ReconnectFlag = false;
            }


        }


        if (DiceController.instance.animationStarted && !DiceController.instance.animationFinished)
        {
            ShowDiceValues();
        }

        if (turnPlayer.IsMoveLeft()&& MoveClick.instance.NoMoveAfterHit == false || turnPlayer.rolledDice == false)
        {
            nextTurnButton.interactable = false;
        }
        else
        {
            nextTurnButton.interactable = true;
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

        if (turnPlayer.IsMoveLeft() && MoveClick.instance.NoMoveAfterHit == false)
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
        RollCounters ++;
         if(currentPlayer.UserId == PassData.Match.Self.UserId)
            {
        if (!currentPlayer.rolledDice)
        {
 
            DiceController.instance.ThrowDices();
            currentPlayer.rolledDice = true;
            StartCoroutine(AfterRolledDice());

                buttonController.EnableDoneButton();
                buttonController.EnableUndoButton();
        }
        else
        {
            Debug.Log("Current player rolled the dice");
        }
        }

              }
 
    private IEnumerator AfterRolledDice()
    {
         nextTurnButton.interactable = false;

        if (!currentPlayer.IsMoveLeft())
        {
        }
        else
        {
            nextTurnButton.interactable = true;
        }

        yield return new WaitForSeconds(0);
   

    }

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

    public void CheckRoundFinish()
    {
        if (IsFinished())
        {
            var score = CalculateScore();
            MalsUserBet();

            Debug.Log("score " + score);
            // increment won round of player
            playerWonRound.score += score;



            if (playerWonRound.UserId == PassData.Match.Self.UserId)
            {
                WinnerImage.SetActive(true);
                EndGameBackground.sprite = GreenBackground;
                MyScoreText.text = score.ToString();
                Reward.SetActive(true);
                inGameData.updateWallet(PassData.betAmount);
                VideoAgora.OnApplicationQuit();
                playerTimer.GameEnded();

                PassData.wins++;

                inGameData.WriteWinsAndLosses(PassData.level , PassData.wins , PassData.losses);
            }
            else
            {
                losserImage.SetActive(true);
                EndGameBackground.sprite = RedBackground;
                OponentScoreText.text = score.ToString();
                VideoAgora.OnApplicationQuit();
                playerTimer.GameEnded();

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
        Debug.Log(currentPlayer.id + " " + currentPlayer.UserId + " " + currentPlayer.pieceType);


        //--------------------------------
        // reset current player's fields
        //--------------------------------
        // flush moves log

        Debug.Log("moves played " + turnPlayer.movesPlayed.Count);
        turnPlayer.movesPlayed.Clear();

        // reset dice
        ResetDice();
        var timer = MatchDataJson.SetTimer(60);
        SendMatchState(OpCodes.Player_Timer, timer);
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
            MoveClick.instance.NoMoveAfterHit = false;


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
            MoveClick.instance.NoMoveAfterHit = false;

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
        if(currentPlayer.movesPlayed.Last().piece.movedWithDrag == true) { 

            if (currentPlayer.movesPlayed.Count == 0)
            {
                Debug.Log("You must have played a move for undo");
                return;
            }

            var lastMove = currentPlayer.movesPlayed.Last();

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
            lastMove.piece.index -= 0.15f;



            currentPlayer.movesPlayed.Remove(lastMove);

            ConvertPieceOutside.instance.FromOutToSlot(lastMove.piece);
            lastMove.piece.IncreaseColliderRadius();

        }
          else
        {
            MoveClick.instance.undo();
        }

    }



}
