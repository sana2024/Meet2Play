using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceAI : MonoBehaviour
{

    // Array of dice sides sprites to load from Resources folder
    [SerializeField]
    public Sprite[] WhiteDiceSides = new Sprite[6];
    [SerializeField]
    public Sprite[] BlackDiceSides = new Sprite[6];
    [SerializeField]
    private BackgammonAI GM;
    [SerializeField]
    public int dieNum;
    // Reference to sprite renderer to change sprites
    private SpriteRenderer rend;
    public int value;


    [SerializeField] GameObject Source;
    [SerializeField] GameObject Destination;

    public float duration = 1f;
    public float smoothTime = 0.3f;
    private Vector3 velocity = Vector3.zero;
    bool IsDiceMoving;
    public static DiceAI Instance;

    public int randomDiceSide = 0;
    // Use this for initialization
    private void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        // Assign Renderer component
        rend = GetComponent<SpriteRenderer>();

    }

    // If you left click over the dice then RollTheDice coroutine is started
    public void rollTheDice()
    {
        StartCoroutine(RollTheDice());
    }

    // Coroutine that rolls the dice
    private IEnumerator RollTheDice()
    {
        IsDiceMoving = true;
        // Variable to contain random dice side number.
        // It needs to be assigned. Let it be 0 initially
 

        // Final side or value that dice reads in the end of coroutine
       int finalSide;

        // Loop to switch dice sides ramdomly
        // before final side appears. 20 itterations here.
        for (int i = 0; i <= 5; i++)
        {
            // Pick up random value from 0 to 5 (All inclusive)
            randomDiceSide = Random.Range(0, 6);

            // Set sprite to upper face of dice from array according to random value
            if(BackgammonAI.Instance.player == 1)
            {
            rend.sprite = WhiteDiceSides[randomDiceSide];
            }

            if (BackgammonAI.Instance.player == 0)
            {
                rend.sprite = BlackDiceSides[randomDiceSide];
            }


            // Pause before next itteration
            yield return new WaitForSeconds(0.5f);
        }

        // Assigning final side so you can use this value later in your game
        // for player movement for example
        finalSide = randomDiceSide + 1;
        value = finalSide;
        GM.setRollValue(dieNum, finalSide);

        // Show final dice value in Console
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
       if (IsDiceMoving)
        {
            Rotate_move_dice();
        } 
    }

    public void Rotate_move_dice()
    {
        if (IsDiceMoving)
        {
            var speed = 3;

            if (transform.position != Destination.transform.position)
            {
                transform.Rotate(Vector3.forward * speed);
            }

            transform.position = Vector3.SmoothDamp(transform.position, Destination.transform.position, ref velocity, smoothTime, Mathf.Infinity, Time.deltaTime);

        }
    }

    public void ResetDice()
    {
        IsDiceMoving = false;
        transform.position = Source.transform.position;
    }

    public int GetValue()
    {
        return value;
    }
}
