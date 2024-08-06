using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Nakama;
using System.Threading.Tasks;
using Nakama.TinyJson;

[RequireComponent(typeof(Collider2D))]
public class Slot : MonoBehaviour
{
    public int slotId = -1;
    public SlotType slotType = SlotType.Board;
    public List<Piece> pieces;
    public string SlotColor;

    public bool MovedWithClick = false;

    public int indx;

    int smallDice;
    int BigDice;

    ISocket isocket;

    UnityMainThreadDispatcher mainThread;

    [SerializeField] Sprite BlackOut;
    [SerializeField] Sprite WhiteOut;

 

    
    // the distance between pieces in a slot when insantiated
    float distance = 0.68f;
    //checks whatever the piece is on tiop side of board or botton side
    public int up;

    public static Slot instance;
 

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        isocket = PassData.isocket;
        mainThread = UnityMainThreadDispatcher.Instance();
        isocket.ReceivedMatchState += m => mainThread.Enqueue(async () => await OnReceivedMatchState(m));

        if(PassData.Match.Self.UserId == GameManager.instance.playerBlack.UserId)
        {
      
            if(this.slotType == SlotType.AboutTobeDeleted)
            {
        
                slotId = 25;
            }
        }
    }

    private async Task OnReceivedMatchState(IMatchState matchState)
    {
        var state = matchState.State.Length > 0 ? System.Text.Encoding.UTF8.GetString(matchState.State).FromJson<Dictionary<string, string>>() : null;

        switch (matchState.OpCode)
        {
            case 18:


                GameObject ClickPieceGB = GameObject.Find(state["PeiceID"]);
                Piece ClickPiece = ClickPieceGB.GetComponent<Piece>();

                GameObject fromSlotGB = GameObject.Find(state["From"]);
                Slot fromSlot = fromSlotGB.GetComponent<Slot>();

                // fromSlot.pieces.Remove(ClickPiece);

                GameObject toSlotGB = GameObject.Find(state["To"]);
                Slot toSlot = toSlotGB.GetComponent<Slot>();

                int Steps = int.Parse(state["Steps"]);

                ConvertPieceOutside.instance.FromOutToSlot(ClickPiece);

                var moveType = "";
                if(state["MoveType"] == "hit")
                {
                    moveType = "hit";
                }
                else if (state["MoveType"] == "move")
                {
                    moveType = "move";
                }

                toSlot.addPiece(ClickPiece, moveType , true);
                fromSlot.pieces.Remove(ClickPiece);
               // toSlot.pieces.Add(ClickPiece);

               // MoveActionTypes moveActionTypes = (MoveActionTypes)Enum.Parse(typeof(MoveActionTypes), state["ActionType"]);



                break;
        }

    }
    private void Update()
    {
 
        foreach(var piece in pieces)
        {
            if(piece.pieceType == PieceType.White)
            {
                SlotColor = "white";

            }

            if (piece.pieceType == PieceType.Black)
            {
                SlotColor = "black";
            }
        }

      
    }

    public int GetPieceTypeCount(PieceType type)
    {
        return pieces.Where(x => x.pieceType == type).Count();
    }

    #region Static Methods


    public static bool IsTopPiece(Slot slot, Piece piece)
    {
        if (slot.pieces.Count > 0)
        {
 
          return slot.pieces.LastOrDefault() == piece;
        }
        else
        {
            return false;
        }

        
       

    }

    public static int GetRequiredStepCount(Slot from, Slot to)
    {
        if (from == null ||
            to == null
            )
            return -1;

        return Math.Abs(to.slotId - from.slotId);
    }

    public static Slot GetBar(PieceType type)
    {
        var slotObject = (type == PieceType.White) ?
            BoardManager.instance.whiteBar :
            BoardManager.instance.blackBar;

        return slotObject.GetComponent<Slot>();
    }

    public static Slot GetOutside(PieceType type)
    {
        var slotObject = (type == PieceType.White) ?
            BoardManager.instance.whiteOutside :
            BoardManager.instance.blackOutside;

        return slotObject.GetComponent<Slot>();
    }

    public static IEnumerable<Slot> GetHomeSlots(PieceType type)
    {
        var slots = BoardManager.instance.slotArray.Select(x => x.GetComponent<Slot>());

        if (type == PieceType.Black)
            return slots.Where(x => x.slotId >= 19 && x.slotId <= 24);

        return slots.Where(x => x.slotId >= 1 && x.slotId <= 6);
    }

    public static Slot GetLastSlotThatHasPiece(PieceType type)
    {
        var slots = BoardManager.instance.slotArray.Select(x => x.GetComponent<Slot>());

        if (type == PieceType.Black)
            slots = slots.Reverse();

        return slots.Last(x => x.GetPieceTypeCount(type) >= 1);
    }

    public bool IsTopSlot()
    {
        return slotId >= 13 && slotId <= 24;
    }

    public static IEnumerable<Piece> GetAbovePieces(Slot from, Piece piece)
    {
        return from.pieces.SkipWhile(x => x != piece);
    }

    #endregion

    #region Move Piece With Click

    // adds pieces to the slot
    public void addPiece(Piece piece, string MoveType, bool recive)
    {
        var currentMove = MoveClick.instance.CurentMove;
        // Debug.Log("current move " + currentMove);
        var move = MoveActionTypes.Move;

        if (this.indx == 27)
        {
            piece.gameObject.GetComponent<SpriteRenderer>().sprite = WhiteOut;
        }

        else

        if (this.indx == 28)
        {
            piece.gameObject.GetComponent<SpriteRenderer>().sprite = BlackOut;
        }


        if (piece.gameObject.GetComponentInParent<Slot>() != this)
        {

            var from = piece.gameObject.GetComponentInParent<Slot>();

            if (from.slotType == SlotType.Bar)
            {
                if (this.pieces.Count > 0 && this.getColor() != MoveClick.instance.player)
                {
                    this.pieces.Remove(this.pieces.First());
                }

            }

            double add = -(0.1 * this.howManyPieces() + 1);
            piece.move(this.transform, this, from);
            pieces.Add(piece);

            GameManager.instance.CheckRoundFinishAsync();


            if (from.slotType == SlotType.AboutTobeDeleted)
            {
                move = MoveActionTypes.Bear;

            }

            var movesPlayedList = GameManager.instance.currentPlayer.movesPlayed;

            foreach (var moves in GameManager.instance.currentPlayer.movesPlayed)

                MovedWithClick = true;


            if (MoveType == "move")
            {
                piece.PieceHistory.Add('c');
                movesPlayedList.Add(new Move { piece = piece, from = from, to = this, step = currentMove, action = move });

                if (recive == false)
                {
                    var state = MatchDataJson.SetPieceStack(piece.name, from.name, this.name, currentMove.ToString(), move.ToString(), "move");
                    GameManager.instance.SendMatchState(OpCodes.Move_Click, state);

                }

            }

            if (MoveType == "undo")
            {
                if (recive == false)
                {
                    var state = MatchDataJson.SetPieceStack(piece.name, from.name, this.name, currentMove.ToString(), move.ToString(), "move");
                    GameManager.instance.SendMatchState(OpCodes.Move_Click, state);
                    

                }

            }


            else if (MoveType == "hit")
            {
                move = MoveActionTypes.Hit;
                var step = 0;

                if (recive == false)
                {

                    var state = MatchDataJson.SetPieceStack(piece.name, from.name, this.name, step.ToString(), MoveActionTypes.Move.ToString(), "move");
                    GameManager.instance.SendMatchState(OpCodes.Move_Click, state);

                }
                // Debug.Log("piece" + piece + " from " + from + " to" + this + " steps " +  step + " action type " + MoveActionTypes.Move + "move type " + MoveType);

            }

        }




    }
    public int howManyPieces()
    {
        return pieces.Count;
    }

    public Piece removePiece()
    {
        Piece p = pieces[pieces.Count - 1];
        pieces.RemoveAt(pieces.Count - 1);
        return p;
    }

    public int getIndx()
    {
        return indx;
    }

    public int getColor()
    {
        if (pieces.Count == 0)
            return -1;

        else if (pieces.First().pieceType == PieceType.White)
        {
            return 1;

        }
        else
        {
            return 0;
        }
    }

    public void deletePiecesInTile()
    {

        while (pieces.Count > 0)
            this.removePiece().deletePiece();
    }

    #endregion
}
