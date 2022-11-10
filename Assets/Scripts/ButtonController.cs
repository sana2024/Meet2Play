using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Nakama;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{

    //in game action buttons
    [SerializeField] public GameObject undoButton;
    [SerializeField] public GameObject rollButton;
    [SerializeField] public GameObject doneButton;
    [SerializeField] public GameObject DoubleButton;
    [SerializeField] public GameObject EndGamePanel;
    [SerializeField] GameObject RequestPlayAgainPanel;
    [SerializeField] GameObject LosserImage;
    [SerializeField] Image EndScreenBackground;
    [SerializeField] Sprite RedImg;
    //Menu button
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameManager gameManager;
    [SerializeField] HelloVideoAgora AgoraVideo;
    [SerializeField] PlayerTimer playerTimer;


    public static ButtonController Instance;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }


    // enable buttons 
    public void EnableUndoButton()
    {
        undoButton.SetActive(true);
    }

    public void EnableRollButton()
    {
        rollButton.SetActive(true);
    }

    public void EnableDoneButton()
    {
        doneButton.SetActive(true);
    }


    // disable buttons
    public void DissableRollButton()
    {
        rollButton.SetActive(false);
    }

    public void DisableUndoButton()
    {
        undoButton.SetActive(false);
    }

    public void DissableDoneButton()
    {
        doneButton.SetActive(false);
    }

    public void OnMenubuttonClicked()
    {
        menuPanel.SetActive(true);
    }

    public void OnMenuCancleClicked()
    {
        menuPanel.SetActive(false);
    }

    public async void OnLeaveClicked()
    {

        GameOver();
 
    }

    public void EnableDoubleButton()
    {
        DoubleButton.SetActive(true);
    }

    public void DisableDoubleButton()
    {
        DoubleButton.SetActive(false);
    }




    public async void GameOver()
    {

        var state = MatchDataJson.SetLeaveMatch("leave");
        gameManager.SendMatchState(OpCodes.Leave_match , state);
        EndGamePanel.SetActive(true);
        LosserImage.SetActive(true);
        EndScreenBackground.sprite = RedImg;
        GameManager.instance.playerWonRound = GameManager.instance.OtherPlayer;
        AgoraVideo.OnApplicationQuit();
        playerTimer.GameEnded();



    }
    public void AcceptReplay()
    {
        var state = MatchDataJson.SetRematch("AcceptReplay");
        GameManager.instance.SendMatchState(OpCodes.Play_Again, state);
        GameManager.instance.OnNextRoundButtonClick();
    }
 
    public void RejectReplay()
    {
        RequestPlayAgainPanel.SetActive(false);
        var state = MatchDataJson.SetRematch("RejectPlayAgain");
        GameManager.instance.SendMatchState(OpCodes.Play_Again, state);
        LeaveScene();


    }
    public async void LeaveScene()
    {
       await PassData.isocket.LeaveMatchAsync(PassData.Match.Id);
        SceneManager.LoadScene("Menu");
    }




}
