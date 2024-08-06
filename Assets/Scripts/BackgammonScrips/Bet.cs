using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Bet : MonoBehaviour
{
    [SerializeField] Text BetAmountText;
    [SerializeField] Text AcceptBetAmountText;
    [SerializeField] GameManager gameManager;
    [SerializeField] HelloVideoAgora VideoAgora;
    [SerializeField] SpriteRenderer DoubleDiceImage;
    [SerializeField] Sprite dice2;
    [SerializeField] Sprite dice4;
    [SerializeField] Sprite dice8;
    [SerializeField] Sprite dice16;
    [SerializeField] Sprite dice32;
    [SerializeField] Sprite dice64;
    [SerializeField] GameObject WaitingPanel;
    [SerializeField] GameObject RequestBetPanel;
    [SerializeField] ButtonController buttonController;
    [SerializeField] GameObject NoEnoughCoinsPanel;
    [SerializeField] Button DoubleButton;
    [SerializeReference] GameObject AcceptBetPanel;
    [SerializeReference] GameObject RejectBetPanel;

    public static Bet Instance;

    int betAmount;
    int diceValue = 1;
   public int nextBetAmount= 1;

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

        ConvertNumberToPrefix(PassData.betAmount, BetAmountText);
        betAmount = PassData.betAmount;

    }

    void ConvertNumberToPrefix(float number, Text text)
    {
        string prefix = "";
        float absNumber = Mathf.Abs(number);
        if (absNumber >= 1000000000)
        {
            number /= 1000000000;
            prefix = "B";
        }
        else if (absNumber >= 1000000)
        {
            number /= 1000000;
            prefix = "M";
        }
        else if (absNumber >= 1000)
        {
            number /= 1000;
            prefix = "K";
        }



        text.text = number.ToString("0.##") + prefix;
    }

    public void Update()
    {
        NextBet();

        PassData.betAmount = betAmount;

   


    }

    public void SendDoubleRequestAI()
    {
        DoubleButton.interactable = false;
        StartCoroutine(DoubleAI());
    }

    IEnumerator DoubleAI()
    {
        yield return new WaitForSeconds(3);
        var pick = Random.Range(0, 2);

        if (pick == 1)
        {
            AcceptBetPanel.SetActive(true);
            yield return new WaitForSeconds(2);
            AcceptBetPanel.SetActive(false);
            IncreaseBet();
        }

        if (pick == 0)
        {
            RejectBetPanel.SetActive(true);
            yield return new WaitForSeconds(2);
            RejectBetPanel.SetActive(false);
            SceneManager.LoadScene("Menu");

        }
    }


    public void SendDoubleRequest()
    {
        if(PassData.WalletMoney >= nextBetAmount)
        {
        Debug.Log("i sent bet request");
        GameManager.instance.betTurns.Add("me");
        var state = MatchDataJson.SetDouble("true");
        gameManager.SendMatchState(OpCodes.Request_Bet , state );
        StartCoroutine(WaitingForResponse());
        buttonController.DisableDoubleButton();

        }
        else
        {
            StartCoroutine(NotEnoughCoins());
        }

    }

    public void AcceptBet()
    {
        IncreaseBet();

        var state = MatchDataJson.SetAccept("Accepted");

        gameManager.SendMatchState(OpCodes.Accept_Bet, state);

        RequestBetPanel.SetActive(false);
 
    }

    IEnumerator WaitingForResponse()
    {
        WaitingPanel.SetActive(true);
        yield return new WaitForSeconds(3);
        WaitingPanel.SetActive(false);
        

    }

    IEnumerator NotEnoughCoins()
    {
       NoEnoughCoinsPanel.SetActive(true);
        yield return new WaitForSeconds(3);
        NoEnoughCoinsPanel.SetActive(false);


    }



    public async void RejectBet()
    {


        var state = MatchDataJson.SetReject("Rejected");
        gameManager.SendMatchState(OpCodes.Reject_Bet, state);


        await PassData.isocket.LeaveMatchAsync(PassData.Match.Id);

        VideoAgora.OnApplicationQuit();
        SceneManager.LoadScene("Menu");
    }



    public void IncreaseBet()
    {

        diceValue *= 2;


        switch (diceValue)
        {
            case 2:
                DoubleDiceImage.sprite = dice2;
                betAmount *= 2;
                AcceptBetAmountText.text = betAmount.ToString();
                BetAmountText.text = betAmount.ToString();
                break;

            case 4:
                DoubleDiceImage.sprite = dice4;
                betAmount *= 2;
                AcceptBetAmountText.text = betAmount.ToString();
                BetAmountText.text = betAmount.ToString();
                break;

            case 8:
                DoubleDiceImage.sprite = dice8;
                betAmount *= 2;
                AcceptBetAmountText.text = betAmount.ToString();
                BetAmountText.text = betAmount.ToString();
                break;

            case 16:
                DoubleDiceImage.sprite = dice16;
                betAmount *= 2;
                AcceptBetAmountText.text = betAmount.ToString();
                BetAmountText.text = betAmount.ToString();
                break;

            case 32:
                DoubleDiceImage.sprite = dice32;
                betAmount *= 2;
                AcceptBetAmountText.text = betAmount.ToString();
                BetAmountText.text = betAmount.ToString();
                break;

            case 64:
                DoubleDiceImage.sprite = dice64;
                betAmount *= 2;
                AcceptBetAmountText.text = betAmount.ToString();
                BetAmountText.text = betAmount.ToString();
                break;
        }


    }

    public void NextBet()
    {
        switch (diceValue)
        {
            case 1:
                nextBetAmount = betAmount * 2;
                break;

            case 2:
                nextBetAmount = betAmount * 4;
                break;

            case 4:
                nextBetAmount = betAmount * 8;
                break;

            case 8:
                nextBetAmount = betAmount * 16;
                break;

            case 16:
                nextBetAmount = betAmount * 32;
                break;

            case 32:
                nextBetAmount = betAmount * 64;
                break;
 

        }

    }
}
