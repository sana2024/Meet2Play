using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Bet : MonoBehaviour
{
    [SerializeField] Text BetAmountText;
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

    int betAmount;
    int diceValue = 1;
    int nextBetAmount= 1;

    // Start is called before the first frame update
    void Start()
    {
  
        BetAmountText.text = PassData.betAmount.ToString();
        betAmount = PassData.betAmount;
    }

    public void Update()
    {
        NextBet();

        PassData.betAmount = betAmount;
        
 
    }


    public void SendDoubleRequest()
    {
        if(PassData.WalletMoney >= nextBetAmount)
        {

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

        var state = MatchDataJson.SetAccept("Accepted");
        gameManager.SendMatchState(OpCodes.Accept_Bet, state);
        IncreaseBet();
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
                BetAmountText.text = betAmount.ToString();
                break;

            case 4:
                DoubleDiceImage.sprite = dice4;
                betAmount *= 4;
                BetAmountText.text = betAmount.ToString();
                break;

            case 8:
                DoubleDiceImage.sprite = dice8;
                betAmount *= 8;
                BetAmountText.text = betAmount.ToString();
                break;

            case 16:
                DoubleDiceImage.sprite = dice16;
                betAmount *= 16;
                BetAmountText.text = betAmount.ToString();
                break;

            case 32:
                DoubleDiceImage.sprite = dice32;
                betAmount *= 32;
                BetAmountText.text = betAmount.ToString();
                break;

            case 64:
                DoubleDiceImage.sprite = dice64;
                betAmount *= 64;
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
