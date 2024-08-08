using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using ByteBrewSDK;
using System.Text.RegularExpressions;

public class EditProfile : MonoBehaviour
{
    [SerializeField] InputField UsernameInputField;
    [SerializeField] TextMeshProUGUI ProfileUsernameText;
    [SerializeField] TextMeshProUGUI MenuUsernameText;
    [SerializeField] Text MatchUsernameText;
    [SerializeField] Text ChallangeUsernameText;
    [SerializeField] GameObject EditAvatarButton;
    [SerializeField] GameObject AlreadyExistText;
    [SerializeField] GameObject InappropriateText;
    [SerializeField] GameObject StartWithnumberText;
    [SerializeField] GameObject AllnumberText;

    [SerializeField] Button EditButton;
    [SerializeField] Text EditButtonText;

    // Start is called before the first frame update
    void Start()
    {
 
        EditButton.onClick.AddListener(Editprofile);
   
    }

    // Update is called once per frame
   async void Update()
    {

    }

    public void Editprofile()
    {
        ProfileUsernameText.gameObject.SetActive(false);
        UsernameInputField.gameObject.SetActive(true);
        EditAvatarButton.SetActive(true);
        EditButtonText.text = "SUBMIT";
        EditButton.onClick.AddListener(SubmitEdit);
    }

    public async void SubmitEdit()
    {
        ProfileUsernameText.gameObject.SetActive(true);
        UsernameInputField.gameObject.SetActive(false);
        EditAvatarButton.SetActive(false);
        EditButtonText.text = "EDIT PROFILE";
        EditButton.onClick.AddListener(Editprofile);

        if(UsernameInputField.text != null && IsWordSafe() == true)
        {
        try
        {
        await  UserProfile.instance.client.UpdateAccountAsync(UserProfile.instance.session, UsernameInputField.text, UsernameInputField.text, PassData.EditAvatar, null, null);
            PassData.Username = UsernameInputField.text;
            ProfileUsernameText.text = PassData.Username;
            MenuUsernameText.text = PassData.Username;
            MatchUsernameText.text = PassData.Username;
            ChallangeUsernameText.text = PassData.Username;

                ByteBrew.NewCustomEvent("ChangedName", "Username=" + PassData.isession.Username + ";");
            }

        catch (Nakama.ApiResponseException ex)
        {
                Debug.Log(ex.Message);
                if (ex.Message == "Username is already in use.")
            {
                   
                    StartCoroutine(UsernameExists());


            }
        }
        }



    }


    IEnumerator UsernameExists()
    {
        AlreadyExistText.SetActive(true);
        yield return new WaitForSeconds(5);
        AlreadyExistText.SetActive(false);


    }

    IEnumerator Inappropriate()
    {
        InappropriateText.SetActive(true);
        yield return new WaitForSeconds(5);
        InappropriateText.SetActive(false);

    }

    IEnumerator StartsWithNumber()
    {
        StartWithnumberText.SetActive(true);
        yield return new WaitForSeconds(5);
        StartWithnumberText.SetActive(false);
    }

    IEnumerator AllNumber()
    {
        AllnumberText.SetActive(true);
        yield return new WaitForSeconds(5);
        AllnumberText.SetActive(false);
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

    public bool IsWordSafe()
    {
        var word = UsernameInputField.text.ToLower();
        if (ReadWordsFromFile("en").Contains(word) || ReadWordsFromFile("es").Contains(word))
        {
            StartCoroutine(Inappropriate());
            return false;
        }
        else if (StartsWithNumberOrSymbol(word))
        {
            StartCoroutine(StartsWithNumber());
            return false;
        }
        else if (IsOnlyNumbersOrSymbols(word))
        {
            StartCoroutine(AllNumber());
            return false;
        }
        else
        {
            Debug.Log("Username Allowed");
            return true;
        }


    }
}

