using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BetAI : MonoBehaviour
{
    [SerializeField] Text BetAmountText;
    [SerializeField] SpriteRenderer DoubleDiceImage;
    [SerializeField] Sprite dice2;
    [SerializeField] Sprite dice4;
    [SerializeField] Sprite dice8;
    [SerializeField] Sprite dice16;
    [SerializeField] Sprite dice32;
    [SerializeField] Sprite dice64;
    [SerializeField] GameObject NoEnoughCoinsPanel;
    public static BetAI Instance;



    int betAmount;
    int diceValue = 1;
    public int nextBetAmount = 1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
 
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
