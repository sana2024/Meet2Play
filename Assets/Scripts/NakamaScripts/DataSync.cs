using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using Nakama.TinyJson;
using System.Threading.Tasks;
using System;
using UnityEngine.SceneManagement;
using ByteBrewSDK;
using System.Linq;
using UnityEngine.UI;
public class DataSync : MonoBehaviour
{
    ISocket isocket;

    public static DataSync Instance;
    ChessPieceType PieceType;

    // we will use these variables to remove the yellow hightlights of our opponent
  public int Opponentx0;
  public int Opponenty0;
  public int Opponentx1;
  public int Opponenty1;
 
    ChessPiece KingPiece;

    [SerializeField] Material CheckMaterial;
    [SerializeField] GameObject CameraBackground;
    [SerializeField] public GameObject RematchDialog;
    [SerializeField] Text RematchUsername;
    [SerializeField] GameObject RejectDialog;
    [SerializeField] Sprite WhiteCheckmate;
    [SerializeField] Sprite BlackCheckmate;
    [SerializeField] GameObject RejectDrawPanel;
    [SerializeField] GameObject RejectGamePanel;
    [SerializeField] Text EndGameDialog;
    public bool AppInBackground;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        if (Instance is not null && Instance != this)
        {
            Destroy(Instance);
            return;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        isocket = PassData.isocket;
        var mainThread = UnityMainThreadDispatcher.Instance();
        isocket.ReceivedMatchState += m => mainThread.Enqueue(async () => await OnReceivedMatchState(m));
    }

    private async Task  OnReceivedMatchState(IMatchState matchState)
    {
        var state = matchState.State.Length > 0 ? System.Text.Encoding.UTF8.GetString(matchState.State).FromJson<Dictionary<string, string>>() : null;

        switch (matchState.OpCode)
        {
            case OpCode.Turn:

                     
                if (state["Turn"] == "White")
                {

                    ChessBoard.Instance.isWhiteTurn = true;
 

                }


                if (state["Turn"] == "Black")
                {

                    ChessBoard.Instance.isWhiteTurn = false;

                }


                break;


            case OpCode.Postion:

                //change the recevied postion to int to pass it to the current postion
                int x = int.Parse(state["PosX"]);
                int y = int.Parse(state["PosY"]);

                //get back a gameobject in the scene that has the name of the received piece
                GameObject pieceOb = GameObject.Find(state["CurrentlyDragging"]);
                ChessPiece piece = pieceOb.GetComponent<ChessPiece>();
                ChessBoard.Instance.moveList.Add(new Vector2Int[] { new Vector2Int(piece.currentX, piece.currentY), new Vector2Int(x, y) });
             
                //clear the old tile that had the peice in it
                ChessBoard.Instance.chessPieces[piece.currentX, piece.currentY] = null;
 
                //move the received piece to the position
                piece.SetPosition(ChessBoard.Instance.getTileCenter(x,y));

                piece.currentX = x;
                piece.currentY = y;

                //update the chesspiece list to include the new piece
                ChessBoard.Instance.chessPieces[piece.currentX, piece.currentY] = piece;
                break;


            case OpCode.Hit:

                int Current_X = int.Parse(state["x"]);
                int Current_Y = int.Parse(state["y"]);


                ChessPiece HittedPiece = ChessBoard.Instance.chessPieces[Current_X , Current_Y];
 
                if (HittedPiece.team == 0)
                {
                    if (HittedPiece.type == ChessPieceType.King)
                    {
                       ChessBoard.Instance.CheckMate(0);
                    }
                    ChessBoard.Instance.didLastMoveCapture = true;

                    ChessBoard.Instance.capturedPiece = true;



                    ChessBoard.Instance.deadWhites.Add(HittedPiece);
                    ChessBoard.Instance.deadWhites = ChessBoard.Instance.deadWhites.Distinct().ToList();





                    HittedPiece.SetScale(new Vector3(0.07f, 0.07f, 0.07f));


                    if(ChessBoard.Instance.deadWhites.Count <= 8)
                    {
                        Vector3 NewPostion = new Vector3(9.17f, ChessBoard.Instance.eatenfirstRow , 0) + (Vector3.right * 0.5f) * ChessBoard.Instance.deadWhites.Count;
                        HittedPiece.SetPosition(NewPostion);
                    }

                    if (ChessBoard.Instance.deadWhites.Count > 8)
                    {
                        Vector3 NewPostion = new Vector3(9.17f, ChessBoard.Instance.eatenSecondRow , 0) + (Vector3.right * 0.5f) * (ChessBoard.Instance.deadWhites.Count - 8);
                        HittedPiece.SetPosition(NewPostion);


                    }
                  
                }
                else
                {
                    if (HittedPiece.type == ChessPieceType.King)
                    {
                        ChessBoard.Instance.CheckMate(1);
                    }
                    ChessBoard.Instance.didLastMoveCapture = true;
                    ChessBoard.Instance.capturedPiece = true;

                    ChessBoard.Instance.deadBlacks.Add(HittedPiece);
                    ChessBoard.Instance.deadBlacks = ChessBoard.Instance.deadBlacks.Distinct().ToList();

                    HittedPiece.SetScale(new Vector3(0.07f, 0.07f, 0.07f));

                    if (ChessBoard.Instance.deadBlacks.Count <= 8)
                    {
                        HittedPiece.SetPosition(new Vector3(-5.8f, ChessBoard.Instance.eatenfirstRow, 0) + (Vector3.right * 0.5f) * ChessBoard.Instance.deadBlacks.Count);
                    }

                    if (ChessBoard.Instance.deadBlacks.Count > 8)
                    {
                        HittedPiece.SetPosition(new Vector3(-5.8f, ChessBoard.Instance.eatenSecondRow, 0) + (Vector3.right * 0.5f) * (ChessBoard.Instance.deadBlacks.Count - 8));
                    }
                     
                }

 

                break;


            case OpCode.Promotion:

                int LastMove_x = int.Parse(state["LastMove_x"]);
                int LastMove_y = int.Parse(state["LastMove_Y"]);
                int team = int.Parse(state["Team"]);
 
                Enum.TryParse<ChessPieceType>(state["Type"] , out PieceType);

                ChessBoard.Instance.SelectPromotedPiece(PieceType , team , LastMove_x , LastMove_y);
                break;


            case OpCode.HighLight:

                int x0 = int.Parse(state["x0"]);
                int y0 = int.Parse(state["y0"]);
                int x1 = int.Parse(state["x1"]);
                int y1 = int.Parse(state["y1"]);


                  Opponentx0 = x0;
                  Opponenty0 = y0;
                  Opponentx1 = x1;
                  Opponenty1 = y1;

                ChessBoard.Instance.AddHighLight(x0,y0,x1,y1);
                ChessBoard.Instance.RemoveLastMoveYellowHighlight();
 
                break;


            case OpCode.check:
 
                if(ChessBoard.Instance.PlayerType == PlayerType.white)
                {
                    GameObject KingOb = GameObject.Find("whiteking4");
                    ChessPiece KingPiece = KingOb.GetComponent<ChessPiece>();

                   // Debug.Log(KingPiece);

                    ChessBoard.Instance.tiles[KingPiece.currentX ,KingPiece.currentY].layer = 17; 
                }

                if (ChessBoard.Instance.PlayerType == PlayerType.black)
 
                {
                    GameObject KingOb = GameObject.Find("Blackking4");
                    ChessPiece KingPiece = KingOb.GetComponent<ChessPiece>();
                   // Debug.Log(ChessBoard.Instance.tiles[KingPiece.currentX, KingPiece.currentY].gameObject.name);
                    ChessBoard.Instance.tiles[KingPiece.currentX, KingPiece.currentY].layer =17;
                }



                break;


            case OpCode.CheckMate:

                ChessBoard.Instance.IsCheckMate = true;
                int winner = int.Parse(state["Winner"]);
                ByteBrew.NewCustomEvent("FinishedGame", "Username=" + PassData.isession.Username + ";");
                // ChessBoard.Instance.DisplayVictory(winner);

                if (ChessBoard.Instance.PlayerType == PlayerType.white)
                {
                    GameEndResult.Instance.LooserResult(WhiteCheckmate);
                }
                if (ChessBoard.Instance.PlayerType == PlayerType.black)
                {
                    GameEndResult.Instance.LooserResult(BlackCheckmate);
                }



                break;


            case OpCode.Notation:
 
                ChessBoard.Instance.ReviecedNotation(state["Notation"]);
 

                break;


              case OpCodes.Camera_Background:

                if (state["Camera_Background"] == "True"){

                    CameraBackground.SetActive(true);
                }


                if (state["Camera_Background"] == "False")
                {

                    CameraBackground.SetActive(false);
                }

                break;


            case OpCodes.Leave_match:
                if(ChessTimer.Instance.OpponentSlider.fillAmount != 0 && ChessTimer.Instance.MySlider.fillAmount != 0 && !GameEndResult.Instance.EndScreenPanel.activeSelf)
                {
                    EndGameDialog.text = PassData.OtherPresence.Username + " left the game.";
                }

                GameEndResult.Instance.WinnerResult();


                break;


            case OpCodes.Play_Again:

                if (state["RequsetRematch"] == "RequestReplay")
                {
                    RematchUsername.text = PassData.otherUsername;
                    RematchDialog.SetActive(true);
                }

                if (state["RequsetRematch"] == "RejectPlayAgain")
                {
                    StartCoroutine(MatchRejected());
                }

                if (state["RequsetRematch"] == "AcceptReplay")
                {
                     
                    SceneManager.LoadScene("Chess");
                }
                break;


            case OpCode.Draw:


                GameEndResult.Instance.DrawResult(state["Draw"]);


                break;



            case OpCode.Castling:

               // Debug.Log("castling");
              int OldX = int.Parse(state["OldX"]);
              int OldY = int.Parse(state["OldY"]);
              int NewX = int.Parse(state["NewX"]);
              int NewY = int.Parse(state["NewY"]);

               // Debug.Log("new x new y " + NewX + "  " + NewY);
                ChessPiece rook = ChessBoard.Instance.chessPieces[OldX, OldY];
                rook.currentX = NewX;
                rook.currentY = NewY;
                ChessBoard.Instance.chessPieces[NewX , NewY] = rook;
                rook.SetPosition(ChessBoard.Instance.getTileCenter(NewX, NewY));
               // ChessBoard.Instance.PositionSinglePiece(NewX, NewY);
                ChessBoard.Instance.chessPieces[OldX, OldY] = null;

                break;


            case OpCode.OfferDraw:

                 if(state["DrawOffer"] == "O")
                {
                    GameEndResult.Instance.RequestedDrawPanel.SetActive(true);
                }
                if (state["DrawOffer"] == "A")
                {
                    GameEndResult.Instance.OfferDrawPanel.SetActive(false);
                    GameEndResult.Instance.DrawResult("Draw By Agreement");
                }
                if (state["DrawOffer"] == "R")
                {
                    GameEndResult.Instance.OfferDrawPanel.SetActive(false);
                    StartCoroutine(GameEndResult.Instance.DrawRejected());

                }


                break;


            case OpCodes.Reject_Match:

                if (state["Leave"] == "Left")
                {

                    StartCoroutine(ShowRejectGamePanel());

                }

                break;


            case OpCodes.Android_Background:
                if (state["backgroundState"] == "Background")
                {
                    AppInBackground = true;
                    Debug.Log("opponent app in background");

                    if (ChessBoard.Instance.victoryScreen.activeSelf == false)
                    {
                       Connections.Instance.OppoenentOffile.SetActive(true);
                       Connections.Instance.leaveInvokeStarted = true;
                       Connections.Instance.WinInvoke();

                    }
                }

                if (state["backgroundState"] == "Forground")
                {
                    AppInBackground = false;
                    Debug.Log("opponent app in forground");
                    Connections.Instance.OppoenentOffile.SetActive(false);
                    Connections.Instance.leaveInvokeTimer = 15f;
                    Connections.Instance.leaveInvokeStarted = false;
                    Connections.Instance.CancelAllInvokes();
                }

                    break;

        }

       
        }

    public async void SendMatchState(long opCode, string state)
    {
 
            await isocket.SendMatchStateAsync(PassData.Match.Id ,opCode, state);
  
    }


    IEnumerator MatchRejected()
    {
        RejectDialog.SetActive(true);
        yield return new WaitForSeconds(3);

        RejectDialog.SetActive(true);
    }


    IEnumerator ShowRejectGamePanel()
    {
        RejectGamePanel.SetActive(true);
        yield return new WaitForSeconds(3);
        HelloVideoAgora.instance.OnApplicationQuit();
        SceneManager.LoadScene("Menu");
    }





}
