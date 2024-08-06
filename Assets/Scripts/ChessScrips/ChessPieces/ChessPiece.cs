using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChessPieceType{
    None =0,
    Pawn = 1,
    Rook =2,
    Bishop = 4,    Knight = 3,
    Queen = 5,
    King = 6
}
public class ChessPiece : MonoBehaviour
{
    public int PiecesID;
    public int team;
    public int currentX;
    public int currentY;
    public ChessPieceType type;
    public Vector3 desiredPosition;
    public Vector3 desiredLocalPosition;
    private Vector3 desiredScale = new Vector3 (0.15f,0.15f,0.15f);


    public void rotatePiece()
    {
        transform.Rotate(0, 0, 180);
    }


    public virtual void SetPosition(Vector3 position, bool force = false){

        desiredPosition = position;
        if (force )
        {
            transform.position = desiredPosition;
        } 
   
    }

    public virtual void SetScale (Vector3 scale, bool force = false)

{
      desiredScale = scale;
        if (force )
        {
            transform.localScale = desiredScale;
        } 

}

public virtual List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX,int tileCountY) {

   List <Vector2Int> r = new List<Vector2Int>();
   r.Add( new Vector2Int(3,3));
   return r;
}

public virtual SpecialMove GetSpecialMoves(ref ChessPiece[,] board, ref List<Vector2Int[]> moveList,ref List<Vector2Int> availableMoves,int tileCountX,int tileCountY ){

    return SpecialMove.None;
}


    private void Update(){

        transform.position  = Vector3.Lerp(transform.position,desiredPosition, Time.deltaTime * 10);
        transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * 10 );
    }
    // Start is called before the first frame update
    void Start()
    {
        
 
    }

    // Update is called once per frame

}
