using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using Nakama.TinyJson;
using System.Threading.Tasks;
using System;
using System.Collections;
using UnityEngine.Networking;

[RequireComponent(typeof(Collider2D))]
public class Piece : MonoBehaviour
{
    //----------------------------
    // identifier fields
    //----------------------------
    public int pieceId;
    public PieceType pieceType;
    [SerializeField] Sprite OutsideBlack;
    [SerializeField] Sprite OutsideWhite;

    GameObject gameManagerOb;
    GameManager gameManager;

    public List<Piece> Circles = new List<Piece>();
    public List<char> PieceHistory = new List<char>();

    public bool BearedOffWithDrag;

    float temps;
    bool click = false;

    //----------------------------
    //  Class Instance
    //----------------------------

    public static Piece instance;
    int MoveCounter;

    //----------------------------
    // Move With Click
    //----------------------------
    public float smoothTime = 0.2F;
    private Vector3 velocity = Vector3.one;
    Vector3 target;
    Vector3 target2;
    public Slot CurSlot = null;
    Slot preSlot = null;

    float offset = 1f; // Arbitrary number to choose based on what looks good
    float multiplier = 0.3f; // The higher this number, the less each item in list affects offset

    //related to possability fade in and fade out
    bool ReachedZero;
    bool ReachedOne;
    float FadeRate;

    //----------------------------
    // identifier properties
    //----------------------------

    public int Position {
        get {
            if (currentSlot == null)
                return -1;
            return currentSlot.slotId;
        }
    }
    public int DenormalizedPosition {
        get {
            return pieceType == PieceType.Black ? 24 - Position : Position;
        }
    }

    //----------------------------
    // for touch and drag
    //----------------------------

    private static bool multipleSelection = false;
 

    // for returning from invalid move
    private Vector2 startPos;
    private float offsetY;

    // flag indicating moving this piece
    public bool isBeingHeld = false;

   
    public Slot currentSlot = null;
    private Slot collisionSlot = null;

    private Slot UndoSlot;

    private CircleCollider2D circleCollider2D;

    GameObject SlotPos;
    Slot undoSlot;

  //  public float index;

   public float posY = 0;

    UnityMainThreadDispatcher mainThread;

 

   public  bool movedWithDrag = false;
   public bool MovedWithClick = false;

 

    //----------------------------
    // Nakama Elements
    //----------------------------
    ISocket isocket;



    // others
    public int currentMoveBig;
    public int currentMoveSmall;

    #region Unity API

    private void Awake()
    {
 
        if(instance == null)
        {
            instance = this;        }

        circleCollider2D = GetComponent<CircleCollider2D>();

 
         

    }

    private void Start()
    {
        target = transform.localPosition;
        gameManagerOb = GameObject.Find("Managers");
        gameManager = gameManagerOb.GetComponent<GameManager>();
         
        isocket = PassData.isocket;
        mainThread = UnityMainThreadDispatcher.Instance();
        isocket.ReceivedMatchState += m => mainThread.Enqueue(async () => await OnReceivedMatchState(m));

 
    }
    
    public bool IsTop()
    {
        var top =Slot.IsTopPiece(currentSlot,this);

        return top;
    }
  

    public async void SendMatchState(long opCode, string state)
    {
     
        await isocket.SendMatchStateAsync(PassData.Match.Id, opCode, state, new[] { PassData.OtherPresence });
    }

    private async Task OnReceivedMatchState(IMatchState matchState)
    {
        var state = matchState.State.Length > 0 ? System.Text.Encoding.UTF8.GetString(matchState.State).FromJson<Dictionary<string, string>>() : null;

        switch (matchState.OpCode)
        {
            case 1:
  
                      

                if (pieceId == int.Parse(state["PeiceID"]))
                {
                    if (this != null)
                    {
                        transform.position = new Vector2(float.Parse(state["Pos_x"]), -float.Parse(state["pos_y"]));
                        Debug.Log("postion " + state["Pos_x"]);
                    }

                }

                break;


            case 8:


                GameObject pieceOb = GameObject.Find(state["PeiceID"]);
                Piece piece = pieceOb.GetComponent<Piece>();

                GameObject from = GameObject.Find(state["From"]);
                Slot FromSlot = from.GetComponent<Slot>();

                FromSlot.pieces.Remove(piece);

                GameObject to = GameObject.Find(state["To"]);
                Slot ToSlot = to.GetComponent<Slot>();

                int steps = int.Parse(state["Steps"]);

                MoveActionTypes ActionType = (MoveActionTypes)Enum.Parse(typeof(MoveActionTypes), state["ActionType"]);

                if (ToSlot.slotType != SlotType.Outside)
                {
                    ToSlot.pieces.Add(piece);
                    piece.PlaceOn(ToSlot);
                }


                var movesPlayedList = GameManager.instance.currentPlayer.movesPlayed;
                movesPlayedList.Add(new Move { piece = this, from = currentSlot, to = ToSlot, step = steps, action = ActionType });

                if (pieceId == piece.pieceId)
                {
                    if (this != null)
                    {
                        Debug.Log("piece " + piece + "from "+currentSlot+" to " + movesPlayedList.Last().to + " action " + movesPlayedList.Last().action + " step " + movesPlayedList.Last().step);
                        OnReciveMove(piece ,movesPlayedList.Last().to, movesPlayedList.Last().action, movesPlayedList.Last().step);
                    }

                }
 
                break;
 

 
        }


        }

    public void UpdateSocket()
    {
        isocket = PassData.isocket;
        isocket.ReceivedMatchState += m => mainThread.Enqueue(async () => await OnReceivedMatchState(m));
        Debug.Log("piece recived new piece");

    }

    void Update()
    {
        if (CurSlot != null)
 
        if (MovedWithClick)
        {
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, target, ref velocity, smoothTime);


            if (CurSlot != null && CurSlot.slotType != SlotType.Outside)
            {
                if (CurSlot.pieces.Count > 5)
                {

                    foreach (Piece p in CurSlot.pieces)
                    {

                            target2 = new Vector2(0, (offset / (CurSlot.pieces.Count * multiplier) * CurSlot.pieces.IndexOf(p) * CurSlot.up));
                            p.transform.localPosition = target2;
                            p.GetComponent<SpriteRenderer>().sortingOrder = CurSlot.pieces.IndexOf(p);


                            if(p.transform.localPosition == target2)
                            {
                                MovedWithClick = false;
                            }
                    }
                }
            }



                if (preSlot != null)
                {
                    if (preSlot.slotType != SlotType.Outside)
                    {
                        if (preSlot.pieces.Count > 4)
                        {

                            foreach (Piece p in preSlot.pieces)
                            {


                                p.transform.localPosition = new Vector2(0, (offset / (preSlot.pieces.Count * multiplier) * preSlot.pieces.IndexOf(p) * preSlot.up));
                                p.GetComponent<SpriteRenderer>().sortingOrder = preSlot.pieces.IndexOf(p);



                            }
                        }
                    }
                }


        }
         if(transform.localPosition == target)
         {
           MovedWithClick = false;
 
          }
          

        if (gameManager.ReconnectSocket == true)
        {
            UpdateSocket();
            gameManager.ReconnectSocket = false;
        }


        if (Input.GetButtonDown("Fire1") && IsMouseOverThis() && IsCurrentPlayerTurn() && IsCurrentPlayerRolled() && IsCurrentPlayerPiece() && IsCurrentPlayerMoveLeft())
        {
            if (!IsBarEmpty())
            {
                /*
                if(gameManager.currentPlayer.pieceType == PieceType.White)
                {
                    CurSlot = MoveClick.instance.slots[26];
                }

                if (gameManager.currentPlayer.pieceType == PieceType.Black)
                {
                    CurSlot = MoveClick.instance.slots[25];
                }
                */
                CurSlot = currentSlot;
                if (CurSlot.slotType == SlotType.Bar)
                {
                    OnPieceClick();
                }
            }
            else
            {
                OnPieceClick();
            }

        }


        else if (isBeingHeld && Input.GetButtonUp("Fire1"))
        {
            temps = 0;
            OnPieceRelease();

           // var state = MatchDataJson.SetPeicePos(pieceId, transform);
          // SendMatchState(OpCodes.Peice_Pos, state);

        }

        if (Input.GetMouseButtonUp(0))
        {
            MoveClick.instance.ResetPossabilities();
        }

        if (isBeingHeld)
        {
            temps++;

            if (temps > 7) {

                if (FadeRate <= 0)
                {
                    ReachedZero = true;
                    ReachedOne = false;
                }

                if (FadeRate >= 0.8f)
                {
                    ReachedOne = true;
                    ReachedZero = false;
                }

                if (ReachedZero == true)
                {
                    FadeRate += 0.015f;
                }

                if (ReachedOne == true)
                {
                    FadeRate -= 0.015f;
                }



            MoveClick.instance.Possabilities(FadeRate);
            MoveClick.instance.IsBeingHeld = true;
            click = false;
            var mousePos = GetMousePos();
            gameObject.transform.position = new Vector3(mousePos.x, mousePos.y + (currentSlot.IsTopSlot() ? -offsetY : offsetY), 0);

            // var state = new Dictionary<string, string> { { "PeiceID", pieceId.ToString() }, { "PeiceX", gameObject.transform.position.x.ToString() }, { "PeiceY", gameObject.transform.position.y.ToString() } }.ToJson();
            // SendMatchState(1, state);
            // Debug.Log(gameObject.transform.position);

           // var state = MatchDataJson.SetPeicePos(pieceId, transform);
           // SendMatchState(OpCodes.Peice_Pos, state);

        }
        }
    }

    public void deletePiece()
    {
        Destroy(this.gameObject);
    }



    private void OnTriggerStay2D(Collider2D collision)
    {
        // check collision with slot
        var slot = collision.gameObject.GetComponent<Slot>();
        if (slot != null)
        {
            //Debug.Log($"Piece #{ pieceId } in Slot #{ slot.slotId }");
            collisionSlot = slot;
        }
    }

    #endregion

    #region Draw Methods

    private void DecreaseColliderRadius()
    {
        circleCollider2D.radius = .1f;
    }

    public void IncreaseColliderRadius()
    {
        circleCollider2D.radius = 4.019942f;
    }

    private float GetOffsetMultiplier(SlotType type)
    {
        switch(type)
        {
            case SlotType.Board:
                return .7f;
            case SlotType.Bar:
                return .1f;
            case SlotType.Outside:
                return .06f;
        };
        return 0f;
    }

    public void PlaceOn(Slot slot)

    {
        var slotPos = slot.transform;
        //-------------------------------------------------
        // calculate offset of y value
        //-------------------------------------------------
 

        // if piece reached the last slot length

 
            posY = slot.pieces.Count * GetOffsetMultiplier(slot.slotType);

        Debug.Log("Piece counts " +slot.pieces.Count);


        // if slot is on top region
        if (slot.slotId >= 13 && slot.slotId <= 24 || 
            (slot.slotType == SlotType.Bar && pieceType == PieceType.Black) || (slot.slotType == SlotType.Outside && pieceType == PieceType.Black))
            posY *= -1;

        //-------------------------------------------------
        // change slot
        //-------------------------------------------------
        // if slot has been in another slot
        if (currentSlot != null)
            currentSlot.pieces.Remove(this);
        var prevSlot = currentSlot;

        // change current slot
        currentSlot = slot;

        // add piece to slot
        if (!slot.pieces.Contains(this))
            slot.pieces.Add(this);


 

        //-------------------------------------------------
        // update ui of piece
        //-------------------------------------------------
        transform.parent = slotPos;
 
        transform.localPosition = new Vector3(0, posY, 0);

        /*
        if (slot.slotType == SlotType.Outside && BearedOffWithDrag)
        {

            posY = slot.pieces.Count * GetOffsetMultiplier(slot.slotType);

            if (slot.slotId >= 13 && slot.slotId <= 24 ||
    (slot.slotType == SlotType.Bar && pieceType == PieceType.Black) || (slot.slotType == SlotType.Outside && pieceType == PieceType.Black))
                posY *= -1;


            transform.localPosition = new Vector3(0, posY, 0);

            BearedOffWithDrag = false;
        }
        */

        if (slot != null && slot.slotType != SlotType.Outside)
        {
            if (slot.pieces.Count > 5)
            {

                Debug.Log("pices are bigger than 5");
                foreach (Piece p in slot.pieces)
                {


                    p.transform.localPosition = new Vector2(0, (offset / (slot.pieces.Count * multiplier) * slot.pieces.IndexOf(p) * slot.up));
                    p.GetComponent<SpriteRenderer>().sortingOrder = slot.pieces.IndexOf(p);
                    p.IncreaseColliderRadius();



                }

            }


        }
 
            if (prevSlot != null && prevSlot.slotType != SlotType.Outside)
        {
                if (prevSlot.pieces.Count > 4)
                {
                Debug.Log("is bigger than 4");

                    foreach (Piece p in prevSlot.pieces)
                    {


                        p.transform.localPosition = new Vector2(0, (offset / (prevSlot.pieces.Count * multiplier) * prevSlot.pieces.IndexOf(p) * prevSlot.up));
                        p.GetComponent<SpriteRenderer>().sortingOrder = prevSlot.pieces.IndexOf(p);



                    }
                }
            }
        
        this.GetComponent<SpriteRenderer>().sortingOrder = slot.pieces.Count;
 
    }


        private void ResetToOldPosition()
    {
        transform.position = new Vector3(startPos.x, startPos.y, 0);

        var state = MatchDataJson.SetPeicePos(pieceId, transform);
        SendMatchState(OpCodes.Peice_Pos, state);
 
    }


    public void move(Transform parentTrans, Slot slot ,Slot prevSlot)
    {
        Debug.Log("moved with click");
        MovedWithClick = true;
        transform.parent = parentTrans;
 
        var posY = slot.pieces.Count * GetOffsetMultiplier(slot.slotType) * slot.up;
 
 
 
        target = new Vector3(0, posY, 0);
        movedWithDrag = false;
        CurSlot = slot;
        currentSlot = slot;
        preSlot = prevSlot;
 

    }

    #endregion

    private bool IsCurrentPlayerMoveLeft()
    {
        return GameManager.instance.currentPlayer.IsMoveLeft();
    }

    public IEnumerable<Slot> GetForwardSlots()
    {
        if (IsOutside())
            return null;

        var list = new List<Slot>();
        var allSlots = BoardManager.instance.slotArray;
        var outside = Slot.GetOutside(pieceType);

        foreach (var slot in allSlots.Select(x => x.GetComponent<Slot>()))
        {
            if (Rule.IsMovingToHome(this, slot))
                list.Add(slot);
        }

        if (pieceType == PieceType.White)
            list.Reverse();

        list.Add(outside);

        return list;
    }

    private bool IsOutside()
    {
        var list = pieceType == PieceType.White ?
            BoardManager.instance.whiteOutside.GetComponent<Slot>() :
            BoardManager.instance.blackOutside.GetComponent<Slot>();

        return list.pieces.Contains(this);
    }

    private bool IsCurrentPlayerRolled()
    {
        return GameManager.instance.currentPlayer.rolledDice;
    }

    private bool IsCurrentPlayerPiece()
    {
        return GameManager.instance.currentPlayer.pieceType == pieceType;
    }

    private bool IsCurrentPlayerTurn()
    {
        return GameManager.instance.currentPlayer == GameManager.instance.turnPlayer;
    }

    private void OnPieceClick()
    {
        Debug.Log("on piece click");
 
        // if current player does not rolled the dice yet
        if (!IsCurrentPlayerRolled())
        {

            BeforeRelease();
        }

        // if there is piece on bar, it must be placed on first
        else if (!IsBarEmpty() && currentSlot.slotType != SlotType.Bar)
        {

            BeforeRelease();
        }

        else if (currentSlot.slotType == SlotType.Outside)
        {

            BeforeRelease();
        }

            if (!Slot.IsTopPiece(currentSlot, this))
            {
                if (!DiceController.instance.IsDoubleMove())
                {
 
                    BeforeRelease();
                }

                // TODO: if current dice has value to move above pieces
                //look if above pieces can be moved ?
                // yes => move above pieces at same time

            
        }

        else
        {
            Hold();
        }
    }

    private void Hold()
    {
        Debug.Log("hold");
        // hold the piece
        isBeingHeld = true;

        // store current position for invalid move
        startPos.x = transform.position.x;
        startPos.y = transform.position.y;

        // store offset
        offsetY = Mathf.Abs(GetMousePos().y - this.transform.position.y);

        // for easing placing
         DecreaseColliderRadius();

 
    }

    private void BeforeRelease()
    {
        // reset holding flag
        isBeingHeld = false;
 


        // for easing placing
        IncreaseColliderRadius();
    }

     IEnumerator AfterRelease()
    {
        // reset current position
        startPos.x = 0;
        startPos.y = 0;

 

        // reset offset of hold
        offsetY = 0;

        yield return new WaitForSeconds(1f);

        MoveClick.instance.IsBeingHeld = false;

    }

    private bool IsBarEmpty()
    {
        var barList = (GameManager.instance.currentPlayer.pieceType == PieceType.White) ?
            BoardManager.instance.whiteBar.GetComponent<Slot>().pieces :
            BoardManager.instance.blackBar.GetComponent<Slot>().pieces;

        if (barList.Count != 0)
            return false;

        return true;
    }

    private void OnPieceRelease()
    {
        BeforeRelease();

        Debug.Log("conlision slot " + collisionSlot);
        // if collision not happen
        if (collisionSlot == null)
        {
            // reset the position
            ResetToOldPosition();
        }
        else
        {
            // current player
            var currentPlayer = GameManager.instance.currentPlayer;
            // get moves left

            if (currentPlayer.UserId == PassData.Match.Self.UserId)
            {
                var movesLeft = DiceController.instance.GetMovesLeftList(currentPlayer.movesPlayed.Select(x => x.step));



                MoveActionTypes action = MoveActionTypes.Move;
                MoveError error = MoveError.Unknown;
                int stepPlayed = -1;

                // loop through dice values
                foreach (var step in movesLeft)
                {
                    stepPlayed = step;
                    error = Rule.ValidateMove(this, collisionSlot, step, out action);

                    // if the move valid, do not continue
                    if (error == MoveError.NoError)
                        break;
                }



                // move to place if move was valid,
                if (error == MoveError.NoError)
                {
                    OnSuccessfulMove(action, stepPlayed);
                }
                // else try combining dice values to get there
                else
                {
                    ICollection<Move> movesPlayed;

                    error = Rule.ValidateCombinedMove(this, collisionSlot, movesLeft, out movesPlayed);

                    // if there are any combined move, move
                    if (error == MoveError.NoError)
                    {
                        foreach (var move in movesPlayed)
                        {
                            OnSuccessfulMove(move.to, move.action, move.step);
                        }
                    }
                    // roll back to the position you were before
                    else
                    {
                        OnFailedMove(error);
 
                    }

 

                }
            }
        }

        StartCoroutine(AfterRelease());
    }

    private void OnFailedMove(MoveError error)
    {
        ResetToOldPosition();
        Debug.Log(error);
    }

    private void OnSuccessfulMove(MoveActionTypes action, int stepPlayed)
    {
        OnSuccessfulMove(collisionSlot, action, stepPlayed);
    }

    public void OnSuccessfulMove(Slot to, MoveActionTypes action, int stepPlayed)
    {
        PieceHistory.Add('d');
        MovedWithClick = false;
        movedWithDrag = true;
        MoveCounter++;
        var movesPlayedList = GameManager.instance.currentPlayer.movesPlayed;

 


        movedWithDrag = false;



        //---------------------------------------
        // action events
        //---------------------------------------
        // move enemy to bar
        if ((action & MoveActionTypes.Hit) == MoveActionTypes.Hit)
        {
            var enemyPiece = to.GetComponent<Slot>().pieces.Last();
            var enemyBar = Slot.GetBar(Piece.GetEnemyType(pieceType));

            enemyPiece.PlaceOn(enemyBar.GetComponent<Slot>());
            var step = 0;
            var HitStated  = MatchDataJson.SetPieceStack(enemyPiece.name, to.name, enemyBar.name, step.ToString(), MoveActionTypes.Move.ToString(), "move");
            GameManager.instance.SendMatchState(OpCodes.Move_Click, HitStated);


        }

        // log played moves for undo
        movesPlayedList.Add(new Move { piece = this, from = currentSlot, to = to, step = stepPlayed, action = action });
        var state = MatchDataJson.SetPieceStack(this.name, currentSlot.name, to.name, stepPlayed.ToString(), action.ToString(), "move");
        SendMatchState(OpCodes.Move_Click, state);



        // move yourself to outside
        if ((action & MoveActionTypes.Bear) == MoveActionTypes.Bear)
        {

            ConvertPieceOutside.instance.FromSlotToOut(this);

            var slotOutside = Slot.GetOutside(pieceType).GetComponent<Slot>();

           // index += 0.15f;

            PlaceOn(slotOutside);



            // check round finish
            GameManager.instance.CheckRoundFinishAsync();
        }
        // place on new slot
        else
            PlaceOn(to);

        if (to.slotType != SlotType.Outside)
        {
            MoveClick.instance.movesMap[to.indx] = MoveClick.instance.availableMoves(to);

        }
        DiceController.instance.OneOfTheDiceUsed = true;

 
    }


    private void OnReciveMove(Piece piece, Slot to, MoveActionTypes action, int stepPlayed)
    {
        var movesPlayedList = GameManager.instance.currentPlayer.movesPlayed;

        var lastMove = movesPlayedList.Last();


        //moves enemy to bar
        if ((action & MoveActionTypes.Hit) == MoveActionTypes.Hit)
        {
            var enemyPiece = to.GetComponent<Slot>().pieces.First();
            var enemyBar = Slot.GetBar(Piece.GetEnemyType(pieceType));

            enemyPiece.PlaceOn(enemyBar.GetComponent<Slot>());


            lastMove.piece.PlaceOn(enemyBar.GetComponent<Slot>());
        }

        // move yourself to outside
        if ((action & MoveActionTypes.Bear) == MoveActionTypes.Bear)
        {
            BearedOffWithDrag = true;
            //  DecreaseColliderRadius();
            ConvertPieceOutside.instance.FromSlotToOut(this);

            var slotOutside = Slot.GetOutside(pieceType);
            PlaceOn(slotOutside);

            // check round finish
            GameManager.instance.CheckRoundFinishAsync();
        }
        // place on new slot
        else

            lastMove.piece.PlaceOn(lastMove.to);

    }

    private bool IsMouseOverThis()
    {
        var hit = Physics2D.Raycast(GetMousePos(), Vector2.zero, 0, Constants.LAYER_PIECE);
        if (hit.collider != null)
        {
            print(hit.collider.name);

                if (currentSlot.slotType == SlotType.Bar && Slot.IsTopPiece(currentSlot, this))
                    return true;
            
            if (hit.collider.name == name)
                return true;
        }
        return false;
    }
 

    private Vector3 GetMousePos()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -Camera.main.transform.position.z;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

       return worldPos;
    }

    #region Static Methods

    public static Piece CreateFromPrefab(GameObject prefab, int id, PieceObject pieceObject)
    {
        var go = Instantiate(prefab);

        // set sprite
        var spriteRenderer = go.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = pieceObject.sprite;

        // set piece parameters
        var piece = go.GetComponent<Piece>();
        piece.name = $"Piece #{ id }";
        piece.pieceId = id;
        piece.pieceType = pieceObject.pieceType;

        return piece;
    }


    public static PieceType GetEnemyType(PieceType you)
    {
        if (you == PieceType.White)
            return PieceType.Black;

        return PieceType.White;
    }

    public static Piece CreateEmpty()
    {
        var go = new GameObject("Piece Empty");
        go.AddComponent<BoxCollider2D>();
        return go.AddComponent<Piece>();
    }

    public void ShowPieceShadowHint(float faderate)
    {
        
        var spriteShadow = this.GetComponent<SpriteRenderer>();
        spriteShadow.color = new Color(1, 1, 1, faderate);
        Debug.Log(FadeRate);
    }

    public void HidePieceShadowHint()
    {
        var spriteShadow = this.GetComponent<SpriteRenderer>();
        spriteShadow.color = new Color(1, 1, 1, 1);
    }

    #endregion
}
