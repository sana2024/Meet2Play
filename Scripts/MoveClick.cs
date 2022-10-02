using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveClick : MonoBehaviour
{
    // All tiles including outsides
    [SerializeField]
    private Slot[] tiles = new Slot[26];

    //black bar and white bar
    [SerializeField]
    private Slot[] eaten = new Slot[2];
    // current slot
    [SerializeField]
    private Slot curTile = null;

    //automatic movement to outside
    [SerializeField]
    private Slot aboutToBeDeleted;

    //from , to and where the piece was eaten
    [SerializeField]
    private List<Slot> origin, destination, eatenOrigin;

    [SerializeField]
    private List<char> whichDie = null;
    float clickTime = 0;

    //black and white piece prefabs
    [SerializeField]
    private GameObject[] pieces = new GameObject[2];

    [SerializeField]
    private GameObject rollButton, undoButton, doneButton;
    [SerializeField]
    private GameObject BG;

    [SerializeField]
    private Dice[] dice = new Dice[2];


    [SerializeField]
    private int player;
    public int[] curMoves = new int[4];
    private int[] startPositions = { 2, 0, 0, 0, 0, 5, 0, 3, 0, 0, 0, 5, 5, 0, 0, 0, 3, 0, 5, 0, 0, 0, 0, 2 };
    private int[] startColors = { 0, -1, -1, -1, -1, 1, -1, 1, -1, -1, -1, 0, 1, -1, -1, -1, 0, -1, 0, -1, -1, -1, -1, 1 };
    private int[] removePositions = { 2, 3, 0, 1, 2, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 0, 0, 5, 3, 2 };
    private int[] removeColors = { 1, 1, -1, 1, 1, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 0, -1, -1, 0, 0, 0 };
    private int[] stuckPositions = { 1, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0 };
    private int[] stuckColors = { 0, 1, 1, 1, 1, 1, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 0, -1, -1, -1, -1 };
    [SerializeField]
    private int BCount = 0, MCount = 0, mCount = 0;
    [SerializeField]
    int notInHousePieces;

    [SerializeField]
    private char[] movesMap = new char[24];
    [SerializeField]
    private char eatenMovesMap;

    [SerializeField]
    private bool alreadyRolled = false;
    private bool alreadyMoved;
    private bool canDeletePieces;
    [SerializeField]
    private bool bigDieWasUsed;
    [SerializeField]
    private bool smallDieWasUsed;
    [SerializeField]
    private int[] piecesStillAlive = { 15, 15 };
    [SerializeField]
    private int checkIfCanRemove;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
