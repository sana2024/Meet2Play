using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Theme : MonoBehaviour
{
    [SerializeField] Image ThemeImage;
    [SerializeField] Sprite GreenBoard;
    [SerializeField] Sprite GreyBoard;
    [SerializeField] Sprite BlueBoard;
    [SerializeField] Sprite BrownBoard;

 
    public void GreenSelected()
    {
        PlayerPrefs.SetString("theme","green");
       // ThemeImage.sprite = GreenBoard;
    }

    public void GreySelected()
    {
        PlayerPrefs.SetString("theme", "grey");
      //  ThemeImage.sprite = GreyBoard;
    }

    public void BlueSelected()
    {
        PlayerPrefs.SetString("theme", "blue");
      //  ThemeImage.sprite = BlueBoard;
    }

    public void BrownSelected()
    {
        PlayerPrefs.SetString("theme", "brown");
       // ThemeImage.sprite = BrownBoard;
    }
}
