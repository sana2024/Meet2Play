using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class MoveClick : MonoBehaviour
{
    // All tiles including outsides
    [SerializeField]
    public Slot[] slots = new Slot[26];

    //black bar and white bar
    [SerializeField]
    private Slot[] hitted = new Slot[2];
    // current slot
    [SerializeField]
    private Slot curslot = null;

    //automatic movement to outside
    [SerializeField]
    private Slot aboutToBeDeleted;

    //from , to and where the piece was eaten
    [SerializeField]
    private List<Slot> origin, destination, eatenOrigin;

    [SerializeField]
    private List<char> whichDie = null;
    float clickTime = 0;

    Player currentPlayer;

    public int[] curMoves = new int[4];
   [SerializeField]
    private int BCount = 0, MCount = 0, mCount = 0;
    [SerializeField] public
    int notInHousePieces;

    [SerializeField]
    private char[] movesMap = new char[24];
    [SerializeField]
    private char eatenMovesMap;

    [SerializeField]
    public bool alreadyRolled = false;
    private bool alreadyMoved;
    private bool canDeletePieces;
    [SerializeField]
    public bool bigDieWasUsed;
    [SerializeField]
    public bool smallDieWasUsed;
    [SerializeField]
    private int[] piecesStillAlive = { 15, 15 };
    [SerializeField]
    private int checkIfCanRemove;

    public static MoveClick instance;

    public int CurentMove;

    [SerializeField] Text Player;

    //players in int values
    public int player;
    int oppontent;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

        player = 0;
        prepareTurn();
    }
    // Update is called once per frame
    void Update()
    {

        if (aboutToBeDeleted.howManyPieces() > 0)
        {
            piecesStillAlive[player] -= aboutToBeDeleted.howManyPieces();
            // aboutToBeDeleted.deletePiecesInTile();
            if (player == 1)
            {
                while (aboutToBeDeleted.pieces.Count > 0)
                    slots[27].addPiece(aboutToBeDeleted.removePiece(), "move" , false);

            }


            if (player == 0)
            {

                while (aboutToBeDeleted.pieces.Count > 0)
                    slots[28].addPiece(aboutToBeDeleted.removePiece(), "move", false);

            }

        }


        Player.text ="player "+ player.ToString();
 
        if (curMoves[0] == -1 && curMoves[1] == -1)
        {
 
        }
 
        else if (!alreadyRolled)
        {
 

            adjustDice();
            movesMap = makeMovesMap();
            if (hitted[player].howManyPieces() > 0)
            {
                eatenMovesMap = makeEatenMovesMap();
                makeAutomaticMovesForEaten();
            }
            if (notInHousePieces == 0)
                makeAutomaticRemovalOfPieces();
            organizeMakeAutomaticMoves();
            alreadyRolled = true;
            if (notInHousePieces == 0)
                checkIfCanRemove = canRemoveAPiece();
            if ((smallDieWasUsed && bigDieWasUsed) || (BCount + mCount + MCount == 0 && checkIfCanRemove == -1))
            {
                endTurn();
            }
        }
        else
        {
            checkIfCanRemove = -1;
            if (notInHousePieces == 0)
                checkIfCanRemove = canRemoveAPiece();
            if ((smallDieWasUsed && bigDieWasUsed) || (BCount + mCount + MCount == 0 && checkIfCanRemove == -1))
            {
                GameManager.instance.nextTurnButton.interactable = true;
            }


 
            if (curMoves[0] == 0 && curMoves[1] == 0 && curMoves[2] == 0 && curMoves[3] == 0)
            {
                prepareTurn();
            }
            else if (Input.GetMouseButtonDown(0))
            {
 
                clickTime = Time.time;
                alreadyMoved = false;

            }
            else if (Input.GetMouseButton(0))
            {
                if (alreadyMoved == false)
                {
                    if (Time.time - clickTime > 0.5f)
                    {
                        if(PassData.Match.Self.UserId == GameManager.instance.currentPlayer.UserId)
                        makeMove(makeTargetIndx(false) );
                        alreadyMoved = true;
                    }
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (alreadyMoved == false)
                {
                    makeMove(makeTargetIndx(true) );
                    alreadyMoved = true;
                }
            }
        }

    }

    public void setRollValue(int die, int rollValue)
    {
        curMoves[die] = rollValue;
    }

    public int makeTargetIndx(bool shortClick)
    {
        curslot = getTile(player);
        if (!curslot || (hitted[player].howManyPieces() > 0 && curslot != hitted[player]) || (bigDieWasUsed && smallDieWasUsed) || curslot.getColor() != player)
            return -1;
        int longIndx, shortIndx;
        char currMoveMap;

        if (hitted[player].howManyPieces() > 0)
        {
            currMoveMap = eatenMovesMap;
            int adjust = 0;
            if (player == 1)
                adjust = -23;
            longIndx = Mathf.Abs(curMoves[1] + adjust - 1);
            shortIndx = Mathf.Abs(curMoves[0] + adjust - 1);
        }
        else
        {
            currMoveMap = movesMap[curslot.getIndx()];
            longIndx = makeTileIndx(curslot.getIndx(), Mathf.Max(curMoves[1], curMoves[0]));
            shortIndx = makeTileIndx(curslot.getIndx(), Mathf.Min(curMoves[1], curMoves[0]));
        }
        if (notInHousePieces == 0 && shortClick)
            if (makeTargetIndxWhenAllInHouse(true, curslot) == 24)
                return 24;
        switch (currMoveMap)
        {
            case 'B':
                if ((shortClick && !bigDieWasUsed) || (!shortClick && smallDieWasUsed))
                {
                    updateRolls(curMoves, 'M');
                    if (alreadyRolled)
                        whichDie.Add('M');
                    return longIndx;
                }
                else if (!smallDieWasUsed)
                {
                    updateRolls(curMoves, 'm');
                    if (alreadyRolled)
                        whichDie.Add('m');
                    return shortIndx;
                }
                break;
            case 'M':
                if (!bigDieWasUsed)
                {
                    updateRolls(curMoves, 'M');
                    if (alreadyRolled)
                        whichDie.Add('M');
                    return longIndx;
                }
                break;
            case 'm':
                if (!smallDieWasUsed)
                {
                    updateRolls(curMoves, 'm');
                    if (alreadyRolled)
                        whichDie.Add('m');
                    return shortIndx;
                }
                break;
        }
        if (notInHousePieces == 0)
            if (makeTargetIndxWhenAllInHouse(false, curslot) == 24)
                return 24;
        return -1; // case 'n'
    }

    public int makeTargetIndxWhenAllInHouse(bool onlyBig, Slot curTile)
    {
        bool canMoveBigDie = false, canMoveSmallDie = false;
        int longIndx = makeTileIndx(curTile.getIndx(), Mathf.Max(curMoves[1], curMoves[0]));
        int shortIndx = makeTileIndx(curTile.getIndx(), Mathf.Min(curMoves[1], curMoves[0]));
        int adjust = 0;
        if (player == 0)
            adjust = 23;
        if (longIndx > 23 || longIndx < 0)
            canMoveBigDie = true;
        if (shortIndx > 23 || shortIndx < 0)
            canMoveSmallDie = true;
        if (!onlyBig && canMoveSmallDie && !smallDieWasUsed && (curMoves[0] == Mathf.Abs(adjust - curTile.getIndx()) + 1 || canMoveToAboutToBeDeleted(curTile.getIndx())))
        {
            updateRolls(curMoves, 'm');
            if (alreadyRolled)
                whichDie.Add('m');
            return 24;
        }
        else if (canMoveBigDie && !bigDieWasUsed && (curMoves[1] == Mathf.Abs(adjust - curTile.getIndx()) + 1 || canMoveToAboutToBeDeleted(curTile.getIndx())))
        {
            updateRolls(curMoves, 'M');
            if (alreadyRolled)
                whichDie.Add('M');
            return 24;
        }
        return -1;
    }

    //information functions
    public Slot getTile(int player)
    {
        Slot t;
        var hit = Physics2D.Raycast(GetMousePos(), Vector2.zero, 0, Constants.LAYER_SLOT);
        if (hit.collider != null)
        {
 
            t = hit.transform.GetComponent<Slot>();

 
            return t;
        }
        return null;
    }

    private Vector3 GetMousePos()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -Camera.main.transform.position.z;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        return worldPos;
    }


    public void endTurn()
    {
        curMoves[0] = curMoves[1] = curMoves[2] = curMoves[3] = 0;
 
    }

    public void updateRolls(int[] rolls, char tar)
    {
        if (rolls[3] != 0)
            rolls[3] = 0;
        else if (rolls[2] != 0)
            rolls[2] = 0;
        else if (tar == 'M')
            bigDieWasUsed = true;
        else
            smallDieWasUsed = true;
        if (bigDieWasUsed && smallDieWasUsed)
        {
          //  ButtonController.Instance.doneButton.SetActive(true);
        }

    }

    public void organizeMakeAutomaticMoves()
    {
        if (smallDieWasUsed && bigDieWasUsed)
        {

        }
        else
        {
            int t = 0;
            if (!smallDieWasUsed)
                t++;
            if (!bigDieWasUsed)
                t++;
            if (curMoves[2] != 0)
                t++;
            if (curMoves[3] != 0)
                t++;
            makeAutomaticMoves(t);
        }
    }

    private void makeAutomaticRemovalOfPieces()
    {
        int i = canRemoveAPiece();
        while (i != -1)
        {
            makeRemoveAPiece(i);
            i = canRemoveAPiece();
        }
    }

    private void makeAutomaticMoves(int times)
    {
        alreadyRolled = true;
        if (curMoves[0] == curMoves[1])
            makeAutomaticMovesForDouble(times);
        else if (times == 2)
            makeAutomaticMovesForTwoDice();
        else if (times == 1)
            makeAutomaticMovesForOneDie();
    }

    public void makeAutomaticMovesForDouble(int times)
    {
        Debug.Log("automatic move for double");
        int adjust = 1;
        if (player == 1)
            adjust = -1;
        int counter = 0, t = times;
        while (BCount <= t && BCount != 0)
        {
            curslot = slots[getIndxFromMoveMap('B')];
            if (!bigDieWasUsed)
            {
                updateRolls(curMoves, 'M');
                whichDie.Add('M');
            }
            else
            {
                updateRolls(curMoves, 'm');
                whichDie.Add('m');
            }
            makeMove(curslot.getIndx() + (curMoves[0] * adjust) );
            counter++;
            t--;
        }
        if (BCount != 0)
        {
            for (int i = 0; i < counter; i++)
                undo();
            return;
        }
        bigDieWasUsed = smallDieWasUsed = true;
    }

    public void makeAutomaticMovesForOneDie()
    {
        Debug.Log("automatic move of one dice");
        int adjust = 1;
        if (player == 1)
            adjust = -1;
        if (!bigDieWasUsed)
        {
            if (BCount == 1 && MCount == 0 && mCount == 0)
            {
                curslot = slots[getIndxFromMoveMap('B')];
                makeMove(curslot.getIndx() + (curMoves[1] * adjust) );
                bigDieWasUsed = true;
            }
            if (BCount == 0 && MCount == 1 && mCount == 0)
            {
                curslot = slots[getIndxFromMoveMap('M')];
                makeMove(curslot.getIndx() + (curMoves[1] * adjust) );
                bigDieWasUsed = true;
            }

        }
        if (!smallDieWasUsed)
        {
            if (BCount == 1 && mCount == 0 && MCount == 0)
            {
                curslot = slots[getIndxFromMoveMap('B')];
                makeMove(curslot.getIndx() + (curMoves[0] * adjust) );
                smallDieWasUsed = true;
            }
            if (BCount == 0 && mCount == 1 && MCount == 0)
            {
                curslot = slots[getIndxFromMoveMap('m')];
                makeMove(curslot.getIndx() + (curMoves[0] * adjust) );
                smallDieWasUsed = true;
            }
        }
    }


    public void makeAutomaticMovesForTwoDice()
    {
        Debug.Log("automatic move of two dices");
        int adjust = 1, check = -1, solutionCount = 0, whatMoveToMake = 0;
        if (player == 1)
            adjust = -1;
        if (BCount == 0 && MCount == 1 && mCount == 1)
        {
            whatMoveToMake = 32;
            solutionCount++;
            curslot = slots[getIndxFromMoveMap('m')];
            makeMove(curslot.getIndx() + (curMoves[0] * adjust) );
            smallDieWasUsed = true;
            whichDie.Add('m');
            if (notInHousePieces == 0)
                if (canRemoveAPiece() != 1)
                {
                    solutionCount++;
                    whatMoveToMake = 41;
                }
            if (BCount == 1 || MCount == 1)
            {
                solutionCount++;
                whatMoveToMake = 42;
            }
            undo();
            smallDieWasUsed = false;
            curslot = slots[getIndxFromMoveMap('M')];
            makeMove(curslot.getIndx() + (curMoves[1] * adjust) );
            bigDieWasUsed = true;
            whichDie.Add('M');
            if (notInHousePieces == 0)
                if (canRemoveAPiece() == 0)
                {
                    solutionCount++;
                    whatMoveToMake = 31;
                }
            if (BCount == 1 || mCount == 1)
            {
                solutionCount++;
                whatMoveToMake = 32;
            }
            undo();
            bigDieWasUsed = false;
            if (solutionCount == 1)
                executeMove(whatMoveToMake);
        }
        else if (BCount == 1 && MCount == 0 && mCount == 1)
        {
            whatMoveToMake = 22;
            solutionCount++;
            curslot = slots[getIndxFromMoveMap('B')];
            makeMove(curslot.getIndx() + (curMoves[0] * adjust) );
            smallDieWasUsed = true;
            whichDie.Add('m');
            if (notInHousePieces == 0)
                if (canRemoveAPiece() != -1)
                {
                    solutionCount++;
                    whatMoveToMake = 11;
                }
            if (BCount == 1 || MCount == 1)
            {
                solutionCount++;
                whatMoveToMake = 12;
            }
            undo();
            smallDieWasUsed = false;
            curslot = slots[getIndxFromMoveMap('B')];
            makeMove(curslot.getIndx() + (curMoves[1] * adjust) );
            bigDieWasUsed = true;
            whichDie.Add('M');
            if (notInHousePieces == 0)
                if (canRemoveAPiece() == 0)
                {
                    solutionCount++;
                    whatMoveToMake = 21;
                }
            if (BCount == 1 || mCount == 2)
            {
                solutionCount++;
                whatMoveToMake = 22;
            }
            undo();
            bigDieWasUsed = false;
            curslot = slots[getIndxFromMoveMap('m')];
            makeMove(curslot.getIndx() + (curMoves[0] * adjust) );
            smallDieWasUsed = true;
            whichDie.Add('m');
            if (notInHousePieces == 0)
                if (canRemoveAPiece() != 1)
                {
                    solutionCount++;
                    whatMoveToMake = 41;
                }
            if (BCount == 1 || MCount == 1)
            {
                solutionCount++;
                whatMoveToMake = 42;
            }
            undo();
            smallDieWasUsed = false;
            if (solutionCount == 1)
                executeMove(whatMoveToMake);
        }
        else if (BCount == 1 && MCount == 1 && mCount == 0)
        {
            whatMoveToMake = 12;
            solutionCount++;
            curslot = slots[getIndxFromMoveMap('B')];
            makeMove(curslot.getIndx() + (curMoves[0] * adjust) );
            smallDieWasUsed = true;
            whichDie.Add('m');
            if (notInHousePieces == 0)
                if (canRemoveAPiece() != -1)
                {
                    solutionCount++;
                    whatMoveToMake = 11;
                }
            if (BCount == 1 || MCount == 2)
            {
                solutionCount++;
                whatMoveToMake = 12;
            }
            undo();
            smallDieWasUsed = false;
            curslot = slots[getIndxFromMoveMap('B')];
            makeMove(curslot.getIndx() + (curMoves[1] * adjust) );
            bigDieWasUsed = true;
            whichDie.Add('M');
            if (notInHousePieces == 0)
                if (canRemoveAPiece() == 0)
                {
                    solutionCount++;
                    whatMoveToMake = 21;
                }
            if (BCount == 1 || mCount == 1)
            {
                solutionCount++;
                whatMoveToMake = 22;
            }
            undo();
            bigDieWasUsed = false;
            curslot = slots[getIndxFromMoveMap('M')];
            makeMove(curslot.getIndx() + (curMoves[1] * adjust) );
            bigDieWasUsed = true;
            whichDie.Add('M');
            if (notInHousePieces == 0)
                if (canRemoveAPiece() == 0)
                {
                    solutionCount++;
                    whatMoveToMake = 31;
                }
            if (BCount == 1 || mCount == 1)
            {
                solutionCount++;
                whatMoveToMake = 32;
            }
            undo();
            bigDieWasUsed = false;
            if (solutionCount == 1)
                executeMove(whatMoveToMake);
        }
        else if (BCount == 1 && MCount == 0 && mCount == 0)
        {
            curslot = slots[getIndxFromMoveMap('B')];
            makeMove(curslot.getIndx() + (curMoves[0] * adjust) );
            smallDieWasUsed = true;
            whichDie.Add('m');
            if (notInHousePieces == 0)
                if (canRemoveAPiece() != -1)
                {
                    solutionCount++;
                    whatMoveToMake = 11;
                }
            if (BCount == 1 || MCount == 1)
            {
                solutionCount++;
                whatMoveToMake = 12;
            }
            undo();
            smallDieWasUsed = false;
            curslot = slots[getIndxFromMoveMap('B')];
            makeMove(curslot.getIndx() + (curMoves[1] * adjust) );
            bigDieWasUsed = true;
            whichDie.Add('M');
            if (notInHousePieces == 0)
                if (canRemoveAPiece() == 0)
                {
                    solutionCount++;
                    whatMoveToMake = 21;
                }
            if (BCount == 1 || mCount == 1)
            {
                solutionCount++;
                whatMoveToMake = 22;
            }
            undo();
            bigDieWasUsed = false;
            if (solutionCount == 1)
                executeMove(whatMoveToMake);
            return;

        }
        else if (BCount == 0 && MCount == 1 && mCount == 0)
        {
            alreadyRolled = false;
            curslot = slots[getIndxFromMoveMap('M')];
            makeMove(curslot.getIndx() + (curMoves[1] * adjust) );
            bigDieWasUsed = true;
            alreadyRolled = true;
            if (notInHousePieces == 0)
                check = canRemoveAPiece();
            if ((mCount == 1 || BCount == 1) && check != -1)
            {
                return;
            }
            else if (mCount == 1)
            {
                curslot = slots[getIndxFromMoveMap('m')];
                makeMove(curslot.getIndx() + (curMoves[0] * adjust) );
            }
            else if (BCount == 1)
            {
                curslot = slots[getIndxFromMoveMap('B')];
                makeMove(curslot.getIndx() + (curMoves[0] * adjust) );
            }
            else
                makeRemoveAPiece(check);
            smallDieWasUsed = true;
        }
        else if (BCount == 0 && mCount == 1 && MCount == 0)
        {
            alreadyRolled = false;
            curslot = slots[getIndxFromMoveMap('m')];
            makeMove(curslot.getIndx() + (curMoves[0] * adjust) );
            smallDieWasUsed = true;
            alreadyRolled = true;
            if (notInHousePieces == 0)
                check = canRemoveAPiece();
            if ((MCount == 1 || BCount == 1) && check != -1)
            {
                return;
            }
            else if (MCount == 1)
            {
                curslot = slots[getIndxFromMoveMap('M')];
                makeMove(curslot.getIndx() + (curMoves[1] * adjust) );
            }
            else if (BCount == 1)
            {
                curslot = slots[getIndxFromMoveMap('B')];
                makeMove(curslot.getIndx() + (curMoves[1] * adjust) );
            }
            else
                makeRemoveAPiece(check);
            bigDieWasUsed = true;
        }

    }

    public void executeMove(int moveCode)
    {
        int adjust = 1;
        if (player == 1)
            adjust = -1;
        if (moveCode / 10 == 1)
        {
            curslot = slots[getIndxFromMoveMap('B')];
            makeMove(curslot.getIndx() + (curMoves[0] * adjust) );
            smallDieWasUsed = true;

            if (moveCode % 10 == 1)
                makeRemoveAPiece(canRemoveAPiece());
            else if (moveCode % 10 == 2)
            {
                curslot = slots[getIndxFromMoveMap('M')];
                makeMove(curslot.getIndx() + (curMoves[1] * adjust) );
            }
            bigDieWasUsed = true;
        }
        if (moveCode / 10 == 2)
        {
            curslot = slots[getIndxFromMoveMap('B')];
            makeMove(curslot.getIndx() + (curMoves[1] * adjust) );
            bigDieWasUsed = true;
            if (moveCode % 10 == 1)
                makeRemoveAPiece(canRemoveAPiece());
            else if (moveCode % 10 == 2)
            {
                curslot = slots[getIndxFromMoveMap('m')];
                makeMove(curslot.getIndx() + (curMoves[0] * adjust) );
            }
            smallDieWasUsed = true;
        }
        if (moveCode / 10 == 3)
        {
            curslot = slots[getIndxFromMoveMap('B')];
            makeMove(curslot.getIndx() + (curMoves[1] * adjust) );
            bigDieWasUsed = true;
            if (moveCode % 10 == 1)
                makeRemoveAPiece(canRemoveAPiece());
            else if (moveCode % 10 == 2)
            {
                curslot = slots[getIndxFromMoveMap('m')];
                makeMove(curslot.getIndx() + (curMoves[0] * adjust) );
            }
            smallDieWasUsed = true;
        }
        if (moveCode == 32)
        {
            curslot = slots[getIndxFromMoveMap('M')];
            makeMove(curslot.getIndx() + (curMoves[1] * adjust) );
            curslot = slots[getIndxFromMoveMap('m')];
            makeMove(curslot.getIndx() + (curMoves[0] * adjust) );
            smallDieWasUsed = bigDieWasUsed = true;
        }
    }


    public void undo()
    {
        if (curMoves[0] == curMoves[1])
        {
            if (whichDie.Count == 1)
                curMoves[3] = curMoves[0];
            else if (whichDie.Count == 2)
                curMoves[2] = curMoves[0];
            else if (whichDie.Count == 3)
                smallDieWasUsed = false;
            else
                bigDieWasUsed = false;
        }
        else
        {
            if (whichDie[whichDie.Count - 1] == 'm')
                smallDieWasUsed = false;
            else
                bigDieWasUsed = false;
        }


        origin[origin.Count - 1].addPiece(destination[destination.Count - 1].removePiece(), "undo", false);
        if (eatenOrigin[eatenOrigin.Count - 1])
            eatenOrigin[eatenOrigin.Count - 1].addPiece(hitted[(player + 1) % 2].removePiece(),  "hit", false);

        if (destination[destination.Count - 1].getIndx() != 24)
        {
            updateTheCount(movesMap[destination[destination.Count - 1].getIndx()], -1);
            movesMap[destination[destination.Count - 1].getIndx()] = availableMoves(destination[destination.Count - 1]);
        }
        if (origin[origin.Count - 1].getIndx() < 24)
        {
            movesMap[origin[origin.Count - 1].getIndx()] = availableMoves(origin[origin.Count - 1]);
            updateTheCount(movesMap[origin[origin.Count - 1].getIndx()], 1);
        }

        if (player == 0 && origin[origin.Count - 1].getIndx() < 18 && destination[destination.Count - 1] != hitted[0] && destination[destination.Count - 1].getIndx() > 17)
            notInHousePieces++;
        if (player == 1 && origin[origin.Count - 1].getIndx() > 5 && destination[destination.Count - 1] != hitted[1] && destination[destination.Count - 1].getIndx() < 6)
            notInHousePieces++;

        whichDie.RemoveAt(whichDie.Count - 1);
        origin.RemoveAt(origin.Count - 1);
        destination.RemoveAt(destination.Count - 1);
        eatenOrigin.RemoveAt(eatenOrigin.Count - 1);

        /*
        ButtonController.Instance.doneButton.SetActive(false);
         if (whichDie.Count == 0)
            ButtonController.Instance.undoButton.SetActive(false);

        */
        var lastMove = GameManager.instance.currentPlayer.movesPlayed.Last();

        GameManager.instance.currentPlayer.movesPlayed.Remove(lastMove);


        var state = MatchDataJson.SetPieceStack(lastMove.piece.name, lastMove.to.name , lastMove.from.name, lastMove.step.ToString(), lastMove.action.ToString() , "undo");
        GameManager.instance.SendMatchState(OpCodes.Move_Click, state);

    }


    //Maps

    //making movements map
    public char[] makeMovesMap()
    {
        notInHousePieces = hitted[player].howManyPieces();
        char[] output = new char[24];
        for (int i = 0; i < 24; i++)
        {
            output[i] = availableMoves(slots[i]);
            if (output[i] == 'B')
                BCount += slots[i].howManyPieces();
            else if (output[i] == 'M')
            {
                MCount += slots[i].howManyPieces();
            }
            else if (output[i] == 'm')
                mCount += slots[i].howManyPieces();
            if (player == 0 && i < 18 && slots[i].getColor() == player)
                notInHousePieces += slots[i].howManyPieces();
            if (player == 1 && i > 5 && slots[i].getColor() == player)
                notInHousePieces += slots[i].howManyPieces();
        }
        return output;
    }

    public char makeEatenMovesMap()
    {
        curslot = getSlot(player);
        int adjust = 0;
        if (player == 1)
            adjust = -23;
        if (isAvailable(slots[Mathf.Abs(curMoves[1] + adjust - 1)]))
        {
            if (isAvailable(slots[Mathf.Abs(curMoves[0] + adjust - 1)]))
                return 'B';
            else
                return 'M';
        }
        else
        {
            if (isAvailable(slots[Mathf.Abs(curMoves[0] + adjust - 1)]))
                return 'm';
        }
        return 'n';
    }

    

    public int getIndxFromMoveMap(char target)
    {
        int counter = 0;
        while (movesMap[counter] != target)
            counter++;
        return counter;
    }

    //information functions
    public Slot getSlot(int player)
    {
        Slot t;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
        if (hit.collider != null)
        {
            t = hit.transform.GetComponent<Slot>();
            return t;
        }
        return null;
    }



    //prepare board for moving
    public void prepareTurn()
    {
        alreadyRolled = false;
        player = (player + 1) % 2;
        curMoves[0] = -1;
        curMoves[1] = -1;
        checkIfCanRemove = -1;
        bigDieWasUsed = false;
        smallDieWasUsed = false;
        BCount = MCount = mCount = 0;
        while (origin.Count > 0)
            origin.RemoveAt(origin.Count - 1);
        while (destination.Count > 0)
            destination.RemoveAt(destination.Count - 1);
        while (whichDie.Count > 0)
            whichDie.RemoveAt(whichDie.Count - 1);
        while (eatenOrigin.Count > 0)
            eatenOrigin.RemoveAt(eatenOrigin.Count - 1);
    }

    public void adjustDice()
    {
        if (curMoves[0] == curMoves[1])
            curMoves[2] = curMoves[3] = curMoves[0];
        if (curMoves[0] > curMoves[1])
        {
            int temp = curMoves[0];
            curMoves[0] = curMoves[1];
            curMoves[1] = temp;
        }
    }

    public void makeAutomaticMovesForEaten()
    {
        Debug.Log("make automatic moves for eaten");
        if (eatenMovesMap == 'n' && hitted[player].howManyPieces() > 0)
            smallDieWasUsed = bigDieWasUsed = true;
        if (bigDieWasUsed && smallDieWasUsed)
        {
           
        }
        else
        {
            int adjust = 0;
            if (player == 1)
                adjust = -23;
            if (hitted[player].howManyPieces() > 0 && eatenMovesMap != 'n' && curMoves[0] == curMoves[1])
            {
                curMoves[3] = 0;
                curslot = hitted[player];
                makeMove(Mathf.Abs(curMoves[1] + adjust - 1) );
            }
            if (hitted[player].howManyPieces() > 0 && eatenMovesMap != 'n' && curMoves[0] == curMoves[1])
            {
                curMoves[2] = 0;
                curslot = hitted[player];
                makeMove(Mathf.Abs(curMoves[1] + adjust - 1) );
            }
            if (hitted[player].howManyPieces() > 1 && eatenMovesMap == 'B')
            {
 
                curslot = hitted[player];
                makeMove(Mathf.Abs(curMoves[1] + adjust - 1) );
                curslot = hitted[player];
                makeMove(Mathf.Abs(curMoves[0] + adjust - 1) );
                smallDieWasUsed = bigDieWasUsed = true;
            }
            else if (hitted[player].howManyPieces() == 1 && eatenMovesMap == 'B' && curMoves[0] == curMoves[1])
            {
         
                curslot = hitted[player];
                makeMove(Mathf.Abs(curMoves[0] + adjust - 1));
                bigDieWasUsed = true;
            }
            else if (hitted[player].howManyPieces() > 0 && eatenMovesMap == 'M')
            {
    
                curslot = hitted[player];
                makeMove(Mathf.Abs(curMoves[1] + adjust - 1) );
                bigDieWasUsed = true;
                if (hitted[player].howManyPieces() > 0)
                    smallDieWasUsed = true;
            }
            else if (hitted[player].howManyPieces() > 0 && eatenMovesMap == 'm')
            {
 
                curslot = hitted[player];
                makeMove(Mathf.Abs(curMoves[0] + adjust - 1) );
                smallDieWasUsed = true;
                if (hitted[player].howManyPieces() > 0)
                    bigDieWasUsed = true;
            }
        }
    }

    public void makeMove(int targetIndx)
    {
 
        int otherPlayer = ((player + 1) % 2), check = -1;
        if (targetIndx != -1)
        {
            if (slots[targetIndx].getColor() == otherPlayer)
            {
                hitted[otherPlayer].addPiece(slots[targetIndx].removePiece(), "hit", false);
                if (alreadyRolled)
                    eatenOrigin.Add(slots[targetIndx]);
 
            }
            else
                eatenOrigin.Add(null);
                slots[targetIndx].addPiece(curslot.removePiece(), "move", false);
 
            if (alreadyRolled)
            {
                ButtonController.Instance.undoButton.SetActive(true);
                origin.Add(curslot);
                destination.Add(slots[targetIndx]);
            }
            if (player == 0 && targetIndx > 17 && curslot != hitted[0] && curslot.getIndx() < 18)
                notInHousePieces--;
            if (player == 1 && targetIndx < 6 && curslot != hitted[1] && curslot.getIndx() > 5)
                notInHousePieces--;
            if (targetIndx != 24)
            {
                movesMap[targetIndx] = availableMoves(slots[targetIndx]);
                updateTheCount(movesMap[targetIndx], 1);
            }
            if (curslot.getIndx() < 24)
            {
                updateTheCount(movesMap[curslot.getIndx()], -1);
                movesMap[curslot.getIndx()] = availableMoves(curslot);
            }
            if (notInHousePieces == 0)
                check = canRemoveAPiece();
            if ((mCount + BCount == 0 && bigDieWasUsed && check != 0) || (MCount + BCount == 0 && smallDieWasUsed && check == -1) || (mCount + MCount + BCount == 0 && check == -1))
            {
       
            }

        }
        curslot = null;
    }

    public char availableMoves(Slot s)
    {

        if (s.getColor() != player)
            return 'n';

        int bigTileIndx = makeTileIndx(s.getIndx(), Mathf.Max(curMoves[1], curMoves[0])), smallTileIndx = makeTileIndx(s.getIndx(), Mathf.Min(curMoves[1], curMoves[0]));

        if (bigTileIndx > 23 || bigTileIndx < 0)
            bigTileIndx = -1;
        if (smallTileIndx > 23 || smallTileIndx < 0)
            smallTileIndx = -1;
        if (bigTileIndx == -1 && smallTileIndx == -1)
            return 'n';
        if (bigTileIndx == -1)
        {
            if (isAvailable(slots[smallTileIndx]))
                return 'm';
            else
                return 'n';
        }
        else
        {
            if (isAvailable(slots[bigTileIndx]))
            {
                if (isAvailable(slots[smallTileIndx]))
                    return 'B';
                else
                    return 'M';
            }
            if (isAvailable(slots[smallTileIndx]))
                return 'm';
            return 'n';
        }



    }

    public int makeTileIndx(int cur, int die)
    {
        if (player == 0)
        {
            return cur + die;
        }
        else
        {
            return cur - die;
        }
    }


    public void updateTheCount(char change, int sign)
    {

        if (change == 'm')
            mCount += sign;
        else if (change == 'M')
            MCount += sign;
        else if (change == 'B')
            BCount += sign;
    }


    private int canRemoveAPiece()
    {
        int targetIndx, adjust = 0;
        if (player == 0)
            adjust = 25;
        if (!smallDieWasUsed)
        {
            targetIndx = Mathf.Abs(adjust - curMoves[0]) - 1;
            if (canMoveToAboutToBeDeleted(targetIndx))
                return 0;
        }
        if (!bigDieWasUsed)
        {
            targetIndx = Mathf.Abs(adjust - curMoves[1]) - 1;
            if (canMoveToAboutToBeDeleted(targetIndx))
                return 1;
        }
        return -1;
    }

    private void makeRemoveAPiece(int move)
    {
        if (move != -1)
        {
            CurentMove = curMoves[move];
            int adjust = 0, startTileIndx;
            if (player == 0)
                adjust = 25;
            startTileIndx = Mathf.Abs(adjust - curMoves[move]) - 1;
            curslot = findRightIndxToRemove(startTileIndx);
            if (move == 0)
            {
                updateRolls(curMoves, 'm');
                if (alreadyRolled)
                    whichDie.Add('m');
            }
            else
            {
                updateRolls(curMoves, 'M');
                if (alreadyRolled)
                    whichDie.Add('M');
            }

            makeMove(24);
        }
    }


    private Slot findRightIndxToRemove(int startTileIndx)
    {
        int adjust = 1, pl = startTileIndx;
        if (player == 1)
            adjust = -1;
        while (pl > -1 && pl < 24)
        {
            if (slots[pl].getColor() == player)
                return slots[pl];
            pl += adjust;
        }
        return null;
    }

    public bool isAvailable(Slot s)
    {
        if (s.getColor() == player || s.getColor() == -1)
            return true;
        if (s.getColor() != player && s.howManyPieces() == 1)
            return true;
        return false;
    }

    public bool canMoveToAboutToBeDeleted(int curTileIndx)
    {
        int i = 0;
        if (player == 0)
        {
            while (18 + i < curTileIndx)
            {
                if (slots[18 + i].howManyPieces() > 0 && slots[18 + i].getColor() == 0)
                    return false;
                i++;
            }
        }
        else
        {
            while (5 - i > curTileIndx)
            {
                if (slots[5 - i].howManyPieces() > 0 && slots[5 - i].getColor() == 1)
                    return false;
                i++;
            }
        }

        return true;
    }
}
