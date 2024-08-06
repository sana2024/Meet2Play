using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    float distance = 1.09f;
    [SerializeField]
    public List<PieceAI> pieces = new List<PieceAI>();
    public int up;
    public int indx;
    [SerializeField] Sprite WhiteOutside;
    [SerializeField] Sprite BlackOutside;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


    }

    public PieceAI removePiece()
    {
        PieceAI p = pieces[pieces.Count - 1];
        pieces.RemoveAt(pieces.Count - 1);
        return p;
    }
    public void addPiece(PieceAI piece)
    {
        var from = piece.gameObject.GetComponentInParent<Tile>();

        if (indx == 27)
        {
            distance = 0.27f;
            piece.GetComponent<SpriteRenderer>().sprite = WhiteOutside;
        }

        if (indx == 28)
        {
            distance = 0.27f;
            piece.GetComponent<SpriteRenderer>().sprite = BlackOutside;
        }

        piece.transform.parent = transform;
        double add = -(0.1 * this.howManyPieces() + 1);
        piece.move(new Vector3(transform.position.x, transform.position.y + ((pieces.Count * distance) * up), (float)add), this , from);
        pieces.Add(piece);
 

    }

    public void addPieceStart(PieceAI piece)
    {
 
        piece.transform.parent = transform;
        double add = -(0.1 * this.howManyPieces() + 1);
        piece.moveStart(new Vector3(transform.position.x, transform.position.y + ((pieces.Count * distance) * up), (float)add));
        pieces.Add(piece);
 

    }
    //private void ShrinkList()
    //{
    //    float originalDistance = 1.0f;
    //    float targetDistance = 0.99f;

    //    float distanceFactor = targetDistance / originalDistance;

    //    float itemCount = pieces.Count;
    //    float shrinkFactor = Mathf.Clamp01((itemCount - 4) / 4f); // Calculate the shrink factor based on the excess items

    //    shrinkFactor -= 0.199f;
    //    Debug.Log("shrink factoir " + shrinkFactor);
    //    distanceFactor *= (1 - shrinkFactor); // Scale down the factor based on the shrink factor

    //    Debug.Log(distanceFactor);
    //    if (distanceFactor < 0.36201)
    //    {
    //        distanceFactor = 0.36201f;
    //    }
    //    for (int i = 0; i < pieces.Count; i++)
    //    {
    //        pieces[i].transform.localPosition = new Vector3(pieces[i].transform.localPosition.x, i * distanceFactor, pieces[i].transform.localPosition.z);
    //    }

    //}

    public int getIndx()
    {
        return indx;
    }

    public void setIndx(int newIndx)
    {
        indx = newIndx;
    }
    public int getColor()
    {
        if (pieces.Count == 0)
            return -1;
        return pieces[0].getPieceColor();
    }

    public int howManyPieces()
    {
        return pieces.Count;
    }

    public void deletePiecesInTile()
    {
        
        while (pieces.Count > 0)  
            this.removePiece().deletePiece();
    }

}
