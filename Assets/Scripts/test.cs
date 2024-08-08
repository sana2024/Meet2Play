using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class test : MonoBehaviour
{
    [SerializeField] InputField Username;
    void Start()
    {


    }

    void Update()
    {

    }

    public static List<string> ReadWordsFromFile(string fileName)
    {
        // Load the text file from the Resources folder
        TextAsset textAsset = Resources.Load<TextAsset>(fileName);

        if (textAsset == null)
        {
            Debug.LogError("File not found: " + fileName);
            return new List<string>();
        }

        // Read the text content
        string fileContent = textAsset.text;

        // Split the content into words
        List<string> words = new List<string>(fileContent.Split(new char[] { ' ', '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries));

        return words;
    }


  public void WordChecker()
    {
        if (ReadWordsFromFile("en").Contains(Username.text) || ReadWordsFromFile("es").Contains(Username.text))
        {
            Debug.Log("this word is inappropriate, please choose another username");
        }
        else if (StartsWithNumberOrSymbol(Username.text))
        {
            Debug.Log("Username should not start with number");
        }
        else if (IsOnlyNumbersOrSymbols(Username.text))
        {
            Debug.Log("Username should at least contain a letter");
        }
        else
        {
            Debug.Log("Username Allowed");
        }

        
    }

    public bool StartsWithNumberOrSymbol(string input)
    {
        // This regex checks if the string starts with a digit or any symbol
        return Regex.IsMatch(input, @"^[\d\W]");
    }


    public bool IsOnlyNumbersOrSymbols(string input)
    {
        // This regex checks if the string is composed entirely of digits or symbols
        return Regex.IsMatch(input, @"^[\d\W]+$");
    }

}
