using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceAI : MonoBehaviour {
    public int color;

    public Vector3 target;
    public float duration = 1f;
    public float smoothTime = 0.3f;
    private Vector3 velocity = Vector3.zero;
    bool isMoving = false;
    Tile CurrentTile;
    Vector3 target2;
    float offset = 1f; // Arbitrary number to choose based on what looks good
    float multiplier = 0.15f; // The higher this number, the less each item in list affects offset
    Tile preTile = null;


    // Use this for initialization
    void Start () {
      // target = transform.position;
	}
	
	// Update is called once per frame
        private void Update()
        {
            if (isMoving)
            {
                transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, smoothTime, Mathf.Infinity, Time.deltaTime);
 

            if (CurrentTile != null )
            {
                if (CurrentTile.indx  <24)
                {
                    Debug.Log("current tile " + CurrentTile.indx);
                    if (CurrentTile.pieces.Count > 6)
                    {

                        foreach (PieceAI p in CurrentTile.pieces)
                        {

                            target2 = new Vector2(0, (offset / (CurrentTile.pieces.Count * multiplier) * CurrentTile.pieces.IndexOf(p) * CurrentTile.up));
                            p.transform.localPosition = target2;
                            p.GetComponent<SpriteRenderer>().sortingOrder = CurrentTile.pieces.IndexOf(p);

                        }
                    }
                }
            }
            if (preTile != null)
            {
                if (preTile.indx  <24)
                {
                    Debug.Log("pre tile "+preTile.indx);
                    if (preTile.pieces.Count > 5)
                    {

                        foreach (PieceAI p in preTile.pieces)
                        {


                            p.transform.localPosition = new Vector2(0, (offset / (preTile.pieces.Count * multiplier) * preTile.pieces.IndexOf(p) * preTile.up));
                            p.GetComponent<SpriteRenderer>().sortingOrder = preTile.pieces.IndexOf(p);



                        }
                    }
                }
            }


            if (Vector3.Distance(transform.position, target) < 0.001f)
                {
                    // Movement complete
                    isMoving = false;
                }
            }
        }
     

    public void move(Vector3 newPos , Tile currenttile , Tile PreTile)
    {
        // transform.position = newPos;
        target = newPos;
        isMoving = true;
        CurrentTile = currenttile;
        preTile = PreTile;
        

    }

    public void moveStart(Vector3 newPos)
    {

        transform.position = newPos;


    }

    public int getPieceColor()
    {
        return color;
    }

    public void deletePiece()
    {
        Destroy(this.gameObject);
    }
}
