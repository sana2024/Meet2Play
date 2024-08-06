using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeBoard : MonoBehaviour
{

    [SerializeField] SpriteRenderer BoardBackground;
    [SerializeField] Sprite Oslo;
    [SerializeField] Sprite tokyo;
    [SerializeField] Sprite boston;
    [SerializeField] Sprite london;
    [SerializeField] Sprite paris;
    [SerializeField] Sprite newyork;
    [SerializeField] Sprite berlin;
    [SerializeField] Sprite dubai;
    [SerializeField] Sprite moscow;
    [SerializeField] Sprite roma;
    public static ChangeBoard instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        changeBoard();
         
    }


    public void changeBoard()
    {
        Debug.Log("Board Type "+ PassData.BoardType);
        switch (PassData.BoardType)
        {
            case "oslo":

                BoardBackground.sprite = Oslo;
                break;

            case "tokyo":
                BoardBackground.sprite = tokyo;

                break;

            case "boston":
                BoardBackground.sprite = boston;

                break;

            case "london":
                BoardBackground.sprite = london;

                break;

            case "paris":
                BoardBackground.sprite = paris;

                break;

            case "newyork":
                BoardBackground.sprite = newyork;

                break;

            case "berlin":
                BoardBackground.sprite = berlin;

                break;

            case "dubai":
                BoardBackground.sprite = dubai;

                break;

            case "moscow":
                BoardBackground.sprite = moscow;

                break;

            case "roma":
                BoardBackground.sprite = roma;

                break;
        }
    }
 
}
