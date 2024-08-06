using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using ByteBrewSDK;

public class EditProfile : MonoBehaviour
{
    [SerializeField] InputField UsernameInputField;
    [SerializeField] TextMeshProUGUI ProfileUsernameText;
    [SerializeField] TextMeshProUGUI MenuUsernameText;
    [SerializeField] Text MatchUsernameText;
    [SerializeField] Text ChallangeUsernameText;
    [SerializeField] GameObject EditAvatarButton;
    [SerializeField] GameObject AlreadyExistText;

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

        if(UsernameInputField.text != null)
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

}

