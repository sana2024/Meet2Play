using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Nakama;
using UnityEngine.UI;
using UnityEngine.Networking;
using Nakama.TinyJson;
using ByteBrewSDK;

public class SystemSettings : MonoBehaviour
{

    [SerializeField] GameObject SettingPanel;
    [SerializeField] public GameObject ConnectionPanel;

    [SerializeField] GameObject MusicToggle;
    [SerializeField] GameObject SoundToggle;

    [SerializeField] Sprite ToggleOn;
    [SerializeField] Sprite ToggleOff;

    [SerializeField] AudioSource sound;
    [SerializeField] AudioSource Music;
    [SerializeField] AudioSource CashSound;

    [SerializeField] GameObject AdsPanel;

    [SerializeField] GameObject TutorialPanel;
    [SerializeField] GameObject DeleteComformation;

    public static SystemSettings instance;
    float timeToEndGame = 0;
 


    bool MusicMute = false;
    bool SoundMute = false;
    public void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void Start()
    {
        string muteSound = PlayerPrefs.GetString("muteSound");

        if (muteSound == "True")
        {
            SoundToggle.SetActive(false);
            sound.mute = true;
            CashSound.mute = true;
        }
        if (muteSound == "False")
        {
            SoundToggle.SetActive(true);
            sound.mute = false;
            CashSound.mute = false;
        }


 
        string muteMusic = PlayerPrefs.GetString("muteMusic");


        if (muteMusic == "True")
        {
            MusicToggle.SetActive(false);
            Music.mute = true;
        }
        if (muteMusic == "False")
        {
            MusicToggle.SetActive(true);
            Music.mute = false;
        }

    }

    public async void OnLeaveClicked()
    {
        PlayerPrefs.SetString("login", "");
        await PassData.iClient.SessionLogoutAsync(PassData.isession);
        await PassData.isocket.RpcAsync("leave", "payload");
        SceneManager.LoadScene("Login");
        ByteBrew.NewCustomEvent("Logout", "Username=" + PassData.isession.Username + ";");
    }

    public void OpenSettings()
    {
        SettingPanel.SetActive(true);

    }

    public void CloseSettings()
    {
        SettingPanel.SetActive(false);

    }

    public void Update()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                ConnectionPanel.SetActive(true);

                timeToEndGame += Time.deltaTime;

                if (timeToEndGame > 16)
                {

                }

            }
            else
            {

            }
        }
 
    }


    public void onSoundClicked()
    {
        SoundMute = !SoundMute;

        PlayerPrefs.SetString("muteSound", SoundMute.ToString());
        string muteSound = PlayerPrefs.GetString("muteSound");

 
        sound.mute = SoundMute;
        CashSound.mute = SoundMute;

        if (muteSound == "True")
        {
            SoundToggle.SetActive(false);

        }
        if(muteSound == "False")
        {
            SoundToggle.SetActive(true);

        }
    }


    public void onMusicClicked()
    {

        MusicMute = !MusicMute;


        PlayerPrefs.SetString("muteMusic", MusicMute.ToString());
        string muteMusic = PlayerPrefs.GetString("muteMusic");

       
        Music.mute = MusicMute;

        if (muteMusic == "True")
        {
            MusicToggle.SetActive(false);

        }
        if (muteMusic == "False")
        {
            MusicToggle.SetActive(true);

        }
    }

    public void openAdsPanel()
    {
        AdsPanel.SetActive(true);

    }

    public void CloseAdsPanel()
    {
        AdsPanel.SetActive(false);
    }

    public void OpenTutorialPanel()
    {
        TutorialPanel.SetActive(true);
    }

    public void CloseTutorialPanel()
    {
        TutorialPanel.SetActive(false);
    }

    public void cancleDeletation()
    {
        DeleteComformation.SetActive(false);
    }

    public void OpenDeletePanel()
    {
        SettingPanel.SetActive(false);
        DeleteComformation.SetActive(true);

    }

    public void CloseTheApp()
    {
        Application.Quit();
    }


}
