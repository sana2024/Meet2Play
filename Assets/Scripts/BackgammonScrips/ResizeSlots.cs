using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResizeSlots : MonoBehaviour
{
    [SerializeField] GameObject Section1;
    [SerializeField] GameObject Section2;
    [SerializeField] GameObject Section3;
    [SerializeField] GameObject Section4;
    [SerializeField] GameObject Boards;

    [SerializeField] ThrowLocation DiceLocation;
    [SerializeField] GameObject ThrowLocation;
    [SerializeField] GameObject BetDice;
    [SerializeField] GameObject whiteSlot;
    [SerializeField] GameObject BlackSlot;

    [SerializeField] GameObject board;
    [SerializeField] GameObject boardBackground;
    [SerializeField] GameObject users;
    [SerializeField] GameObject autoRoll;
    [SerializeField] GameObject diceValues;
    [SerializeField] GameObject bet;
    [SerializeField] GameObject turnPlayer;
    [SerializeField] GameObject endgameButtons;
    [SerializeField] GameObject reward;
    [SerializeField] GameObject gameScore;
    //[SerializeField] GameObject CameraMask1;
    //[SerializeField] GameObject CameraMask2;
    //[SerializeField] GameObject CameraMask2local;



    private void Update()
    {
       
            //RectTransform mask1Rect = CameraMask1.GetComponent<RectTransform>();
            //RectTransform mask2Rect = CameraMask2.GetComponent<RectTransform>();
            //RectTransform mask2Rectlocal = CameraMask2local.GetComponent<RectTransform>();
        //resize board based on screen size
        if (Camera.main.aspect <= 1.6)
        {
            //ipad and tablet
            board.transform.localScale = new Vector2(0.868f, 0.868f);
            boardBackground.transform.localScale = new Vector2(0.44f, 0.417f);

            RectTransform UserRect = users.GetComponent<RectTransform>();
            RectTransform AutoRollRect = autoRoll.GetComponent<RectTransform>();
            RectTransform DiceValueRect = diceValues.GetComponent<RectTransform>();
            RectTransform BetRect = bet.GetComponent<RectTransform>();
            RectTransform TurnRect = turnPlayer.GetComponent<RectTransform>();
            RectTransform EndGameRect = endgameButtons.GetComponent<RectTransform>();
            RectTransform rewardRect= reward.GetComponent<RectTransform>();
            RectTransform scoreRect = gameScore.GetComponent<RectTransform>();

            UserRect.anchoredPosition = new Vector2(0,-29);
            users.transform.localPosition = new Vector3(0.94f, 0.94f, 0.94f);
            AutoRollRect.anchoredPosition = new Vector2(-593, 33);
            DiceValueRect.anchoredPosition = new Vector2(-55, 435);
            BetRect.anchoredPosition = new Vector2(-500, 6);
            TurnRect.anchoredPosition = new Vector2(5, -45);
            turnPlayer.transform.localScale = new Vector3(2.1f, 0.54f, 1.3f);
            EndGameRect.anchoredPosition = new Vector2(0,99);
            rewardRect.anchoredPosition = new Vector2(9, 13);
            scoreRect.anchoredPosition = new Vector2(0, 88);
            BetDice.transform.position = new Vector2(4.5f, 0.003f);


            //mask1Rect.anchoredPosition = new Vector2(mask1Rect.anchoredPosition.x, -60);
            //mask2Rect.anchoredPosition = new Vector2(mask2Rect.anchoredPosition.x, -60);
            //mask2Rectlocal.anchoredPosition = new Vector2(mask2Rectlocal.anchoredPosition.x, -60);


            if (SceneManager.GetActiveScene().name == "GameScene")
            {
                Camera.main.fieldOfView = 63;
            }
        }

        if(Camera.main.aspect > 1.6 && Camera.main.aspect < 2)
        {
            // 16:9 screen size
            board.transform.localScale = new Vector3(0.9763f, 0.9763f, 0.9763f);
            boardBackground.transform.localScale = new Vector3(0.5f,0.47f, 0.5f);

            //mask1Rect.anchoredPosition = new Vector2(mask1Rect.anchoredPosition.x, -87);
            //mask2Rect.anchoredPosition = new Vector2(mask2Rect.anchoredPosition.x, -87);

        }

        if (Camera.main.aspect >= 2)
        {
              

        }


    }

    public void rotate()
    {
         Boards.transform.Rotate(180, 0, 0);
         ThrowLocation.transform.position = new Vector2(2.85f, -3);

      //  whiteSlot.transform.position = new Vector2(4.11f,-3.41f);
      //  BlackSlot.transform.position = new Vector2(5.06f, 4.46f);




    }

    public void Resize()
    {
        Section1.transform.position = new Vector3(Section1.transform.position.x, 0.1f, Section1.transform.position.z);
        Section2.transform.position = new Vector3(Section2.transform.position.x, 0.1f, Section2.transform.position.z);
        Section3.transform.position = new Vector3(Section3.transform.position.x, -0.1f, Section3.transform.position.z);
        Section4.transform.position = new Vector3(Section4.transform.position.x, -0.1f, Section3.transform.position.z);
    }



   

}
