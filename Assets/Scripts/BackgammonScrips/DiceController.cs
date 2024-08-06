using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Nakama;
using Nakama.TinyJson;
using System.Threading.Tasks;
using UnityEngine.UI;

[RequireComponent(typeof(GameManager))]
public class DiceController : MonoBehaviour
{
    public static DiceController instance;

    [Header("Prefabs")]
    [SerializeField]
    private Dice whiteDicePrefab;
    [SerializeField]
    private Dice blackDicePrefab;

    [Header("Vectors")]
    [SerializeField]
    private ThrowLocation throwLocations;

    [HideInInspector]
    public Sprite firstValueSprite;
    [HideInInspector]
    public Sprite secondValueSprite;

    [SerializeField] Image DiceImage1;
    [SerializeField] Image DiceImage2;

    // 2 white and 2 black dices
    private Dice[] dices = new Dice[4];
    private IEnumerable<Dice> rollingDices = null;
    public bool animationStarted = false;
    public bool animationFinished = false;
    public int[] values;

    [SerializeField] Sprite[] BlackCanvasDice;
    [SerializeField] Sprite[] WhiteCanvasDice;

    public bool OneOfTheDiceUsed = false;

    ISocket isocket;

    int BigDice;
    int smalldice;

 
    UnityMainThreadDispatcher mainThread;
    [SerializeField] GameManager gameManager;

 
    private IEnumerable<Dice> WhiteDices
    {
        get { return dices.Take(2); }
    }

    private IEnumerable<Dice> BlackDices
    {
        get { return dices.Skip(2).Take(2); }
    }

    #region Unity API

    private void Awake()
    {
        if (instance == null)
            instance = this;

        InitializeDices();
    }

    private void Start()
    {
        isocket = PassData.isocket;
        mainThread = UnityMainThreadDispatcher.Instance();
        try
        {
            isocket.ReceivedMatchState += m => mainThread.Enqueue(async () => await OnReceivedMatchState(m));
        }catch (Nakama.ApiResponseException ex)
        {
            Debug.Log("recived state " + ex);
        }
    }



    private async Task OnReceivedMatchState(IMatchState matchState)
    {
        var state = matchState.State.Length > 0 ? System.Text.Encoding.UTF8.GetString(matchState.State).FromJson<Dictionary<string, string>>() : null;

        switch (matchState.OpCode)
        {
 

            case 5:

                 SetRollingDices();
 

 


                foreach (var dice in rollingDices)
                 {
                     if(dice != null)
                    {
                     dice.gameObject.SetActive(true);

                    }

                 }

               // await Task.Delay(5000);

                /*

                foreach (var dice in rollingDices)
                {
                    if (dice != null)
                    {
                        dice.gameObject.SetActive(false);

                    }

                }

                */

                break;


            case 2:
 

                ThrowDicesRecive(state);

                break;


            case 17:
                

                int dice1 = int.Parse(state["DiceValue1"]);
                int dice2 = int.Parse(state["DiceValue2"]);

                values[0] = dice1;
                values[1] = dice2;

                if (state["DiceColor"] == "white")
                {
                    DiceImage1.sprite = WhiteCanvasDice[dice1-1];
                    DiceImage2.sprite = WhiteCanvasDice[dice2 - 1];
                }

                if (state["DiceColor"] == "black")
                {
                    DiceImage1.sprite = BlackCanvasDice[dice1-1];
                    DiceImage2.sprite = BlackCanvasDice[dice2 - 1];
                }



                
                break;
        }


    }

    public async void SendMatchState(long opCode, string state)
    {
        try
        {

            await isocket.SendMatchStateAsync(PassData.Match.Id, opCode, state, new[] { PassData.OtherPresence });
        }
        catch (Nakama.ApiResponseException ex)
        {
            Debug.Log("sending data " + ex);
        }
    }

    public void UpdateSocket()
    {
        isocket = PassData.isocket;
        isocket.ReceivedMatchState += m => mainThread.Enqueue(async () => await OnReceivedMatchState(m));
        Debug.Log("new socket recive created");
    }

 

    private void Update()
    {
           if(gameManager.ReconnectSocket == true)
        {
            UpdateSocket();
            gameManager.ReconnectSocket = false;
        }

        if (!animationStarted || animationFinished)
            return;

        var firstDice = rollingDices.First();
        var secondDice = rollingDices.Last();

        firstValueSprite = firstDice.GetComponent<SpriteRenderer>().sprite;
        secondValueSprite = secondDice.GetComponent<SpriteRenderer>().sprite;
 
        animationFinished = true;

        foreach (var dice in rollingDices)
            animationFinished &= dice.AnimationFinished;
       
  
    }

    #endregion

    #region Draw Methods

    private void ShowRollingDices()
    {
        foreach (var dice in rollingDices)
        {
            if(dice != null)
            {
              dice.gameObject.SetActive(true);
            }

        }


        var state = MatchDataJson.SetDiceVisability("true");
        SendMatchState(OpCodes.Show_dice, state);
    }

    public void HideRollingDices()
    {
        if(rollingDices  != null)
        {
          foreach (var dice in rollingDices)
        {
            if(dice != null)
            {
               dice.gameObject.SetActive(false);
            }

        }
        }

             rollingDices = null;
    }

    public void ThrowDices()
    {
        var throwLocation = GetThrowLocation();
        var direction = throwLocation.direction;
        var pos = throwLocation.transform.position;
        var speed = 3f;

        Roll();

        SetRollingDices();
        ShowRollingDices();

        var firstDice = rollingDices.First();
        var secondDice = rollingDices.Last();
 

        MoveClick.instance.setRollValue(0 , values[0]);
        MoveClick.instance.setRollValue(1, values[1]);

        // set direction
        firstDice.direction = new Vector2(direction.x, direction.y);
        secondDice.direction = new Vector2(direction.x + .25f, direction.y);

        // set move speed
        firstDice.moveSpeed = speed;
        secondDice.moveSpeed = speed;

        // throw from position
        firstDice.Throw(values[0], pos);
        secondDice.Throw(values[1], pos + Vector3.up * (direction.y > 0 ? 1 : -1));

        // start checking animation finish
        animationStarted = true;
        animationFinished = false;

        var state = MatchDataJson.SetDicePos(pos, firstDice.value , secondDice.value );
        SendMatchState(OpCodes.throw_Loc, state);

        var State = MatchDataJson.SetDiceCanvar(firstDice.DiceColor, firstDice.value.ToString(), secondDice.value.ToString());
        SendMatchState(OpCodes.Dice_Canvas, State);
    }


    public void ThrowDicesRecive(IDictionary <string, string> state )
    {
        var throwLocation = GetThrowLocation();
        var direction = throwLocation.direction;
        var pos = throwLocation.transform.position;
        var speed = 5f;
 
        var firstDice = rollingDices.First();
        var secondDice = rollingDices.Last();

        // set direction
        firstDice.direction = new Vector2(direction.x, direction.y);
        secondDice.direction = new Vector2(direction.x + .25f, direction.y);

        // set move speed
        firstDice.moveSpeed = speed;
        secondDice.moveSpeed = speed + Random.Range(.75f, 3f);

 

        if(firstDice.body2D != null && secondDice.body2D != null)
        {
 
            //rotation
            Vector3 randomRotation = new Vector3(Random.Range(30f, 60f), 0, 0);
            firstDice.body2D.AddTorque(4);
            secondDice.body2D.AddTorque(3);

            //position
            firstDice.body2D.position = new Vector3(pos.x -0.5f, pos.y, 0);
            secondDice.body2D.position = new Vector3(pos.x +0.5f, pos.y, 0);

            //velocity
            var VelocityPos = new Vector3(0f, 9f, 0);
            firstDice.body2D.velocity = VelocityPos ;
            secondDice.body2D.velocity = VelocityPos;




        }

 
    }







    #endregion

    private void InitializeDices()
    {
        int index = 0;
        Vector2 postion = new Vector2(2.85f,-3.32f);

        if (Camera.main.aspect <= 1.6)
        {
            whiteDicePrefab.transform.localScale = new Vector2(0.14f, 0.14f);
            blackDicePrefab.transform.localScale = new Vector2(0.11f, 0.11f);

        }
        if (Camera.main.aspect >= 2)
        {
            whiteDicePrefab.transform.localScale = new Vector2(0.18f, 0.18f);
            blackDicePrefab.transform.localScale = new Vector2(0.13f, 0.13f);

        }

        // instantiate 2 white dices
        for (int i = 0; i < 2; i++, index++)
        {
            dices[index] = Instantiate(whiteDicePrefab, postion, Quaternion.identity);
            dices[index].DiceID = index + 1;
            dices[index].DiceID = index - 1;
            dices[index].DiceColor = "white";
        }

        // instantiate 2 black dices
        for (int i = 0; i < 2; i++, index++)
        {
            dices[index] = Instantiate(blackDicePrefab, postion, Quaternion.identity);
            dices[index].DiceID = index - 1;
            dices[index].DiceColor = "black";
        }

        foreach (var dice in dices)
            dice.gameObject.SetActive(false);
    }

    private ThrowLocation GetThrowLocation()
    {
        return throwLocations;
    }

    private void SetRollingDices()
    {
        rollingDices = GameManager.instance.turnPlayer.pieceType == PieceType.White 
            ? WhiteDices
            : BlackDices;

 
    }

    #region Dice Functions

#if TEST_VALUES
    static int counter = 0;
#endif
    public void Roll()
    {
#if !TEST_VALUES
        values[0] = UnityEngine.Random.Range(1, 7);
        values[1] = UnityEngine.Random.Range(1, 7);
#else
        if ((counter & 1) == 0)
        {
            values[0] = 6;
            values[1] = 6;
        }
        else
        {
            values[0] = 3;
            values[1] = 3;
        }   
        counter++;
#endif

        SortValues();
    }

    private void SortValues()
    {
        System.Array.Sort(values);
    }

    public bool IsDoubleMove()
    {
        return values[0] == values[1];
    }

    public int GetWeight()
    {
        var sum = values.Sum();
        return IsDoubleMove() ? sum * 2 : sum;
    }

    public IEnumerable<int> GetMovesList()
    {
        if (!IsDoubleMove())
            return values;

        return new int[] { values[0], values[0], values[0], values[0] };
    }

    public IEnumerable<int> GetMovesLeftList(IEnumerable<int> playedSteps)
    {
        var list = GetMovesList().ToList();
        foreach (var step in playedSteps)
        {

            if (step > 0)
            {
               // Debug.Log("played stepts " + playedSteps.Count());

                if (list.Contains(step))
                {
                    if (MoveClick.instance.alreadyRolled)
                    {

                        list.RemoveAt(list.FindIndex(x => x == step));



                        if (!IsDoubleMove())
                        {
                            if (OneOfTheDiceUsed == true)
                            {
                                if (step == MoveClick.instance.curMoves[1])
                                {
                                    Debug.Log("big was used ");
                                    MoveClick.instance.bigDieWasUsed = true;

                                }

                                if (step == MoveClick.instance.curMoves[0])
                                {
                                    Debug.Log("small was used ");
                                    MoveClick.instance.smallDieWasUsed = true;
                                }

                                OneOfTheDiceUsed = false;
                            }
                            
                        }
                        else
                        {
                            
                            if(MoveClick.instance.curMoves[1] == MoveClick.instance.curMoves[0])
                            {
                                
                                
                                if (list.Count == 0)
                                {
                                    MoveClick.instance.smallDieWasUsed = MoveClick.instance.bigDieWasUsed = true;
                                }
                                
                            }
                            
                          
                        }

                    }
 
                }
 

            }


        }
 
        return list;

    }

}




#endregion

