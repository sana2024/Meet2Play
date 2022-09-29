using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Nakama;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{

    //in game action buttons
    [SerializeField] GameObject undoButton;
    [SerializeField] GameObject rollButton;
    [SerializeField] GameObject doneButton;
    [SerializeField] GameObject DoubleButton;
    [SerializeField] GameObject EndGamePanel;
    [SerializeField] GameObject LosserImage;
    [SerializeField] Image EndScreenBackground;
    [SerializeField] Sprite RedImg;
    //Menu button
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameManager gameManager;
    [SerializeField] HelloVideoAgora AgoraVideo;
    [SerializeField] PlayerTimer playerTimer;


 
  
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
        await PassData.isocket.LeaveMatchAsync(PassData.Match.Id);
        EndGamePanel.SetActive(true);
        LosserImage.SetActive(true);
        EndScreenBackground.sprite = RedImg;

        AgoraVideo.OnApplicationQuit();




        playerTimer.GameEnded();



    }
 

    public void LeaveScene()
    {
        SceneManager.LoadScene("Menu");
    }




}
