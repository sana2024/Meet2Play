using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvertPieceOutside : MonoBehaviour
{
    //sprites
    [SerializeField] Sprite OutsideSpriteWhite;
    [SerializeField] Sprite OutsideSpriteBlack;
    [SerializeField] Sprite SlotSpriteWhite;
    [SerializeField] Sprite SlotSpriteBlack;
    SpriteRenderer SpriteWhite;
    SpriteRenderer SpriteBlack;

 
    //instance
    public static ConvertPieceOutside instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

    }


    public void FromSlotToOut(Piece piece)
    {
        if(piece.pieceType == PieceType.White)
        {
          SpriteWhite = piece.GetComponent<SpriteRenderer>();
          SpriteWhite.sprite = OutsideSpriteWhite;
        }

        if (piece.pieceType == PieceType.Black)
        {
            SpriteBlack = piece.GetComponent<SpriteRenderer>();
            SpriteBlack.sprite = OutsideSpriteBlack;
        }

    }

    public void FromOutToSlot(Piece piece)
    {
        if (piece.pieceType == PieceType.White)
        {
            SpriteWhite = piece.GetComponent<SpriteRenderer>();
            SpriteWhite.sprite = SlotSpriteWhite;
        }

        if (piece.pieceType == PieceType.Black)
        {
            SpriteBlack = piece.GetComponent<SpriteRenderer>();
            SpriteBlack.sprite = SlotSpriteBlack;
        }

    }
}
