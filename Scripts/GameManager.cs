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
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;


    [Header("Player Properties-----")]
    //-----------------
    //Player properties
    //-----------------
      Player playerWhite;
      Player playerBlack;
      public Player currentPlayer;
      public Player turnPlayer;
      Player playerWonRound;
      Player MyPlayer;

    public string PlayerColor;
    public string OpponentColor;

    [Header("UI Elements-----")]
    //-----------------
    //UI elements
    //-----------------
    public Button undoButton;   
    public Button nextTurnButton;
    public Button rollButton;
    public Image  firstDiceValueImage;
    public Image secondDiceValueImage;

    [SerializeField] ButtonController buttonController;
    [SerializeField] Text LevelText;
    [SerializeField] Sprite RedBackground;
    [SerializeField] Sprite GreenBackground;
    [SerializeField] Image EndGameBackground;
    [SerializeField] Text MyScoreText;
    [SerializeField] Text OponentScoreText;
    [SerializeField] Image OponentChecker;
    [SerializeField] Sprite WhiteChecker;
    [SerializeField] Sprite BlackChecker;
    [SerializeField] Text TimeoutDEbugger;
    [SerializeField] Image MyChecker;


    [Header("GameObjects-----")]
    //-----------------
    // GameObjects
    //-----------------
    public GameObject gameEndScreen;
    [SerializeField] GameObject Board;
    [SerializeField] GameObject CameraBackground;
    [SerializeField] GameObject EndGamePanel;
    [SerializeField] GameObject WinnerImage;
    [SerializeField] GameObject losserImage;
    [SerializeField] GameObject DoublePanel;
    [SerializeField] GameObject AcceptedPanel;
    [SerializeField] GameObject RejectBetPanel;
    [SerializeField] GameObject RejectGamePanel;
    [SerializeField] GameObject NoMoveExistsPanel;
    [SerializeField] GameObject Reward;

    [Header("Classes-----")]
    //-----------------
    // Classes
    //-----------------
    [SerializeField] ResizeSlots resizeSlots;
    [SerializeField] PlayerTimer playerTimer;
    [SerializeField] HelloVideoAgora VideoAgora;
    [SerializeField] Bet bet;
    [SerializeField] InGameData inGameData;
    [SerializeField] private Slot[] slots = new Slot[26];
    [SerializeField] DiceController diceController;

    [Header("Conditions-----")]
    //-----------------
    // Conditions
    //-----------------
    bool InternetConnected;
    bool ReconnectFlag = false;
    bool click = false;
    public bool ReconnectSocket = false;

    public bool canPlayBig = false;
    public bool canPlaySmall = false;

   // public bool BigPlayed = false;
   // public bool SmallPlayed = false;

   //  bool bigOutside = false;
   // bool smallOutside = false;

    [Header("Rounds-----")]
    //rounds
    private const int ROUND_LIMIT = 3;
    private int currentRound = 1;

    [Header("Nakama Component-----")]
    //nakama components
    ISocket isocket;
    IClient iclient;
    ISession isession;

    [Header("Dices-----")]
    //dices
    int diceValue1 = 0;
    int diceValue2 = 0;

    public int BigDice = 0;
    public int SmallDice = 0;

    [Header("Others-----")]
    //Others
    int RollCounters = 1;
    float temps;
    Slot ClickedSlot;
  //   public int MoveCounter = 0;
    MoveActionTypes moveAction;

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

            PlayerColor = "black";
            OpponentColor = "white";

        }
        if (PassData.Match.Self.UserId == playerWhite.UserId)
        {
            MyPlayer = playerWhite;

            PlayerColor = "white";
            OpponentColor = "black";

            
        }
 
    }

 

    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        Debug.Log("my player" + MyPlayer.pieceType);


       
       

        isocket = PassData.isocket;
        iclient = PassData.iClient;
        isession = PassData.isession;

        var mainThread = UnityMainThreadDispatcher.Instance();
        isocket.ReceivedMatchState += m => mainThread.Enqueue(async () => await OnReceivedMatchState(m));

        HideGameEndScreen();
 
        currentPlayer = playerWhite;
        turnPlayer = currentPlayer;

        PlayerColor = "white";
        OpponentColor = "black";
 
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

             //   BigPlayed = false;
             //   SmallPlayed = false;
             //   MoveCounter = 0;


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
                    OpponentColor = "white";

                    Debug.Log(currentPlayer.id + " " + currentPlayer.UserId + " " + currentPlayer.pieceType);

                }

                if (state["Current_Player"] == "White")
                {
                    currentPlayer = playerWhite;
                    turnPlayer = currentPlayer;
                    OpponentColor = "black";

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
                

                lastMove.piece.PlaceOn(lastMove.from , false);

                // undo hit action
                if ((lastMove.action & MoveActionTypes.Hit) == MoveActionTypes.Hit)
                {
                    var enemyBar = Slot.GetBar(Piece.GetEnemyType(lastMove.piece.pieceType));
                    var enemyPiece = enemyBar.pieces.Last();
                    enemyPiece.PlaceOn(lastMove.to , false);
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

             /*
            case 18:
                
                
                int PieceId = int.Parse(state["PieceId"]);
                int ToId = int.Parse(state["slot"]);
                int FromId = int.Parse(state["FromId"]);

                Slot RecivedSlot = slots[ToId - 1];
                Slot fromSlot = slots[FromId - 1];

                Piece MovingPiece = fromSlot.pieces.Last();

                if(RecivedSlot.pieces.Any())
                {
                   if(RecivedSlot.pieces.Count == 1 && RecivedSlot.pieces.First().pieceType == MyPlayer.pieceType)
                    {

                        RecivedSlot.addPiece(MovingPiece, currentPlayer, fromSlot, true, MoveActionTypes.Hit);
                    }
                }
                else
                {

                    RecivedSlot.addPiece(MovingPiece, currentPlayer, fromSlot, true, MoveActionTypes.Move);


                }


                Debug.Log("from: "+fromSlot+ " to: "+RecivedSlot+" piece: "+PieceId);

                break;

                */

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

    IEnumerator ShowNoMovePanel()
    {
        NoMoveExistsPanel.SetActive(true);
        yield return new WaitForSeconds(3);
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

        if (Input.GetMouseButtonDown(0))
        {
            ClickedSlot = GetSlot();
            temps = Time.time;
            click = true;
        }

        if (click == true)
        {
            // get possable moves if slot pressed
            if ((Time.time - temps) > 0.2)
            {
                if(ClickedSlot.SlotColor ==  PlayerColor && currentPlayer.rolledDice && ClickedSlot.pieces.Count != 0)
                {
                     Possab(ClickedSlot);
                }

            }

        }
        // place piece on disired slot when slot clicked
        if (Input.GetMouseButtonUp(0))
        {
            resetPossab();
            click = false;

            if ((Time.time - temps) < 0.2)
            {
                if(currentPlayer.UserId == PassData.Match.Self.UserId && currentPlayer.rolledDice == true)
                {
                    if (!EventSystem.current.IsPointerOverGameObject())
                    {

                        DiceChecker();
                        Movements(GetSlot());
                        MovePiecesWithClick();


                    }
 
                }

            }
        }


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

        if (turnPlayer.IsMoveLeft() || turnPlayer.rolledDice == false)
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

        if (turnPlayer.IsMoveLeft())
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
        if (currentPlayer.IsPlayableMoveExist())
        {
               nextTurnButton.interactable = false;
        }

        if (!currentPlayer.IsPlayableMoveExist())
        {
            StartCoroutine(ShowNoMovePanel());
        }
        
        yield return new WaitForSeconds(3f);
        if (!currentPlayer.IsMoveLeft())
        {
            NextTurn();
        }

        else
        {
           foreach(var moves in currentPlayer.movesPlayed)
            {
                Debug.Log("moves "+moves);
            }

            nextTurnButton.interactable = true;
        }
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

        BigPlayed = false;
        SmallPlayed = false;
        MoveCounter = 0;

        //--------------------------------
        // reset current player's fields
        //--------------------------------
        // flush moves log
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
        if (currentPlayer.movesPlayed.Count == 0)
        {
            Debug.Log("You must have played a move for undo");
            return;
        }

        var lastMove = currentPlayer.movesPlayed.Last();

        Debug.Log("step "+lastMove.step+" from "+lastMove.from+ " to "+lastMove.to+ " current player moves" + currentPlayer.movesPlayed.Count + "piece " + currentPlayer.movesPlayed[0].piece);

        // undo move action
        lastMove.piece.PlaceOn(lastMove.from , false);

 

        Debug.Log(lastMove.from.ToString());
        var state = MatchDataJson.SetPieceStack(lastMove.piece.name, lastMove.from.name, lastMove.to.name, lastMove.step.ToString() , lastMove.action.ToString());
        SendMatchState(OpCodes.undo ,state);

        // undo hit action
        if ((lastMove.action & MoveActionTypes.Hit) == MoveActionTypes.Hit)
        {
            var enemyBar = Slot.GetBar(Piece.GetEnemyType(lastMove.piece.pieceType));
            var enemyPiece = enemyBar.pieces.Last();
            enemyPiece.PlaceOn(lastMove.to, false) ;
        }

        //undo bear action
        lastMove.piece.index -= 0.15f;

 

        currentPlayer.movesPlayed.Remove(lastMove);

        ConvertPieceOutside.instance.FromOutToSlot(lastMove.piece);
        lastMove.piece.IncreaseColliderRadius();

        if(lastMove.step == BigDice)
        {
            BigPlayed = false;
            MoveCounter--;
        }
        if(lastMove.step == SmallDice)
        {
            SmallPlayed = false;
            MoveCounter--;
        }

    }

    #region Slot Click And Possabilites

    // get the slot based on mouse postion click

    public Slot GetSlot()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -Camera.main.transform.position.z;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);


        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 0, Constants.LAYER_SLOT);

        if(hit.collider != null)
        {
            return hit.collider.gameObject.GetComponent<Slot>();
        }
        else
        {
         return null;
        }


    }
    // check which dice is bigger and playes first
    public void DiceChecker()
    {
        diceValue1 = diceController.values[0];
        diceValue2 = diceController.values[1];

        if (diceValue1 > diceValue2)
        {
            BigDice = diceValue1;
            SmallDice = diceValue2;

        //    Debug.Log(BigDice + " is bigger than " + SmallDice);
        }
        else
        {
            BigDice = diceValue2;
            SmallDice = diceValue1;

        //    Debug.Log(BigDice + " is bigger than " + SmallDice);
        }
    }

    // reverse movement direction based on player color
    public int MovementDirection(int SlotNum, int Dice)
    {
        if (currentPlayer.pieceType == PieceType.White)
        {
            return SlotNum - Dice;
        }
        if (currentPlayer.pieceType == PieceType.Black)
        {
            return SlotNum + Dice;
        }

        return 0;

    }

    // show player possablities to drag pieces to
    public void Possab(Slot ClickedSlot)
    {
        if (currentPlayer.IsMoveLeft())
        {
             

            var BigDicePosb = MovementDirection(ClickedSlot.slotId, BigDice);
            var smallDicePosb = MovementDirection(ClickedSlot.slotId, SmallDice);

            if (BigDicePosb - 1 >= 0)
            {

                Slot BigSlot = slots[BigDicePosb - 1];
                Slot SmallSlot = slots[smallDicePosb - 1];


                if (BigSlot.SlotColor != OpponentColor && BigPlayed == false || (BigSlot.SlotColor == OpponentColor && BigSlot.pieces.Count == 1))
                {
                    SpriteRenderer[] BigSprite = BigSlot.GetComponentsInChildren<SpriteRenderer>();

                    BigSprite[0].color = new Color(BigSprite[0].color.r, BigSprite[0].color.g, BigSprite[0].color.b, 0.7f);
                    BigSprite[1].color = new Color(BigSprite[1].color.r, BigSprite[1].color.g, BigSprite[1].color.b, 0.4f);
                    canPlayBig = true;

                }


                if (SmallSlot.SlotColor != OpponentColor && SmallPlayed == false || (SmallSlot.SlotColor == OpponentColor && SmallSlot.pieces.Count == 1))
                {
                    SpriteRenderer[] smallSprite = SmallSlot.GetComponentsInChildren<SpriteRenderer>();

                    smallSprite[0].color = new Color(smallSprite[0].color.r, smallSprite[0].color.g, smallSprite[0].color.b, 0.7f);
                    smallSprite[1].color = new Color(smallSprite[1].color.r, smallSprite[1].color.g, smallSprite[1].color.b, 0.4f);
                    canPlaySmall = true;

                }


            }
        }



    }

    // reset possabiliy color if player released mouse click
    public void resetPossab()
    {
        for (int i = 0; i < 24; i++)
        {
            SpriteRenderer [] SlotSprite = slots[i].GetComponentsInChildren<SpriteRenderer>();
            SlotSprite[0].color = new Color(SlotSprite[0].color.r, SlotSprite[0].color.g, SlotSprite[0].color.b, 0);
            SlotSprite[1].color = new Color(SlotSprite[1].color.r, SlotSprite[1].color.g, SlotSprite[1].color.b, 0);
        }
    }

    // Positions that player can move pieces to if clicked
    public void Movements(Slot ClickedSlot)
    {

        //check if big dice or white dice is playable on clicked slot
        var BigDicePosb = MovementDirection(ClickedSlot.slotId, BigDice);
        var smallDicePosb = MovementDirection(ClickedSlot.slotId, SmallDice);
 
        if (BigDicePosb - 1 >= 0)
        {

            Slot BigSlot = slots[BigDicePosb - 1];
            Slot SmallSlot = slots[smallDicePosb - 1];

            Debug.Log(BigSlot.SlotColor);
            Debug.Log(SmallSlot.SlotColor);
             
            //check if player can move Big dice on this slot
            if (BigSlot.SlotColor != OpponentColor || (BigSlot.SlotColor == OpponentColor && BigSlot.pieces.Count == 1))
            {
                Debug.Log("can play big");
                canPlayBig = true;

            }

            // check if player can move small dice on this slot
            if (SmallSlot.SlotColor != OpponentColor || (SmallSlot.SlotColor == OpponentColor && SmallSlot.pieces.Count == 1))
            {
                Debug.Log("can play small");
                canPlaySmall = true;

            }


        }



    }

    // perform move action on the pice
    public void MovePiecesWithClick()
    {
        if (currentPlayer.IsMoveLeft())
        {


            // this coondition will be true if slot can be played on both small and big dice
            if (canPlayBig == true && canPlaySmall == true)
            {


                MoveCounter++;


                //first move big dice
                if (BigPlayed == false)
                {
                    int bigMove = MovementDirection(GetSlot().slotId, BigDice);
                    var last = GetSlot().pieces.LastOrDefault();


                    if (slots[bigMove - 1].SlotColor != OpponentColor || slots[bigMove - 1].SlotColor == OpponentColor && slots[bigMove - 1].pieces.Count == 1)
                    {

                        HitMovement(slots[bigMove - 1]);
                        slots[bigMove - 1].addPiece(last, currentPlayer, GetSlot(), false , MoveActionTypes.Move);


                        GetSlot().pieces.Remove(last);



                        // check if its double dice
                        if (BigDice == SmallDice)
                        {
                            if (MoveCounter >= 4)
                            {
                                BigPlayed = true;
                                canPlayBig = false;
                                canPlaySmall = false;
                            }
                        }


                        else
                        {
                            BigPlayed = true;
                            canPlayBig = false;
                            canPlaySmall = false;
                        }


                    }


                }
                //then move small dice after big dice
                if (SmallPlayed == false && BigPlayed == true && MoveCounter == 2)
                {
                    int smallMove = MovementDirection(GetSlot().slotId, SmallDice);
                    var last = GetSlot().pieces.LastOrDefault();
                    HitMovement(slots[smallMove - 1]);
                    slots[smallMove - 1].addPiece(last, currentPlayer, GetSlot(), false , MoveActionTypes.Move);
                    GetSlot().pieces.Remove(last);

                    SmallPlayed = true;
                    canPlayBig = false;
                    canPlaySmall = false;
                }



            }

            //this codtion will be true if the slot can only be played on big dice
            if (canPlayBig == true && canPlaySmall == false)
            {
                if (BigPlayed == false)
                {
                    int bigMove = MovementDirection(GetSlot().slotId, BigDice);
                    var last = GetSlot().pieces.LastOrDefault();

                    HitMovement(slots[bigMove - 1]);
                    slots[bigMove - 1].addPiece(last, currentPlayer, GetSlot() , false , MoveActionTypes.Move);
                    GetSlot().pieces.Remove(last);

                    BigPlayed = true;
                    canPlayBig = false;
                    canPlaySmall = false;
                }

            }

            // this condtion will be true if slot can only be played on small dice
            if (canPlayBig == false && canPlaySmall == true)
            {
                if (SmallPlayed == false)
                {
                    int smallMove = MovementDirection(GetSlot().slotId, SmallDice);
                    var last = GetSlot().pieces.LastOrDefault();
                    HitMovement(slots[smallMove - 1]);
                    slots[smallMove - 1].addPiece(last, currentPlayer, GetSlot(), false , MoveActionTypes.Move);
                    GetSlot().pieces.Remove(last);

                    SmallPlayed = true;
                    canPlayBig = false;
                    canPlaySmall = false;
                }

            }

            foreach (var move in currentPlayer.movesPlayed)
            {
                Debug.Log(move.step);
            }

        }
       // Debug.Log(currentPlayer.movesPlayed.Sum(x => x.step));
    }

    //checks if the slot that contains enemy pieces has only one piece in it
    //moves enemy piece to bar
    public void HitMovement(Slot destination)
    {
        if (destination.pieces.Count == 1 && destination.pieces.First().pieceType != currentPlayer.pieceType)
        {
            var hittedPiece = destination.pieces.First();

            destination.pieces.Remove(hittedPiece);


            if (hittedPiece.pieceType == PieceType.Black)
            {
                slots[24].addPiece(hittedPiece , currentPlayer, GetSlot(), false , MoveActionTypes.Hit);
            }

            if (hittedPiece.pieceType == PieceType.White)
            {
                slots[25].addPiece(hittedPiece , currentPlayer, GetSlot(), false, MoveActionTypes.Hit);
            }


        }
    }

    #endregion



}
