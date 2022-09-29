using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Slot : MonoBehaviour
{
    public int slotId = -1;
    public SlotType slotType = SlotType.Board;
    public List<Piece> pieces;
    public string SlotColor;

    // the distance between pieces in a slot when insantiated
    float distance = 0.7f;
    //checks whatever the piece is on tiop side of board or botton side
    public int up;

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
        return slot.pieces.Last() == piece;
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
    public void addPiece(Piece piece, Player currentPlayer , Slot from , bool RecivedMove , MoveActionTypes actionTypes)
    {

 
       if(RecivedMove == false){
        piece.PlaceOn(this , true);
        if ( currentPlayer.UserId == PassData.Match.Self.UserId)
        {
            var movesLeft = DiceController.instance.GetMovesLeftList(currentPlayer.movesPlayed.Select(x => x.step));
            int stepPlayed = -1;

            // loop through dice values
            foreach (var step in movesLeft)
            {
                stepPlayed = step;
 
 
            }

               /// piece.OnSuccessfulMove(this, MoveActionTypes.Move, stepPlayed);

                 var movesPlayedList = GameManager.instance.currentPlayer.movesPlayed;
                 movesPlayedList.Add(new Move { piece =  piece, from = from, to = this, step = stepPlayed , action = actionTypes});

                  var state = MatchDataJson.SetPieceStack(piece.name, from.name, this.name, stepPlayed.ToString() ,actionTypes.ToString() );
                  GameManager.instance.SendMatchState(OpCodes.stack, state);

            }
                  // var state = MatchDataJson.SetMoveClick(piece.pieceId ,this.slotId, from.slotId ,true);
                  // GameManager.instance.SendMatchState(OpCodes.Move_Click, state);

 
        }
        else
        {
            piece.PlaceOn(this, true);
        }

       


    }

    public int howManyPieces()
    {
        return pieces.Count;
    }

    #endregion
}
