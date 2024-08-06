using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchTheme : MonoBehaviour
{
    [SerializeField] Sprite GreenBoard;
    [SerializeField] Sprite GreyBoard;
    [SerializeField] Sprite BlueBoard;
    [SerializeField] Sprite BrownBoard;

    [SerializeField] SpriteRenderer Board;


    // Start is called before the first frame update
    void Start()
    {
        switch (PlayerPrefs.GetString("theme"))
        {
            case "green":

                Board.sprite = GreenBoard;

                break;

            case "grey":

                Board.sprite = GreyBoard;

                break;

            case "brown":

                Board.sprite = BrownBoard;

                break;

            case "blue":

                Board.sprite = BlueBoard;

                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
