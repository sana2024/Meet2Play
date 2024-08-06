using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class AIfakeCam : MonoBehaviour
{

    int currentCamIndex = 0;

    WebCamTexture tex;

    public RawImage display;
    [SerializeField] GameObject FakeCamera;

    //Local
    [SerializeField] Sprite LocalVideoMute;
    [SerializeField] Sprite LocalAudioMute;

    [SerializeField] Sprite LocalVideoUnmute;
    [SerializeField] Sprite LocalAudioUnmute;


    [SerializeField] Button LocalAudio;
    [SerializeField] Button LocalVideo;

    //Remote
    [SerializeField] Sprite RemoteVideoMute;
    [SerializeField] Sprite RemoteAudioMute;

    [SerializeField] Sprite RemoteVideoUnmute;
    [SerializeField] Sprite RemoteAudioUnmute;


    [SerializeField] Button RemoteAudio;
    [SerializeField] Button RemoteVideo;

    [SerializeField] GameObject WhiteBackground;

    public static AIfakeCam Instance;

    WebCamDevice[] devices;

    bool IsLocalAudioMute = false;
    bool IsLocalVideoMute = false;
    bool IsRemoteAudioMute = false;
    bool IsRemoteVideoMute = false;

    private void Awake()
    {
       devices = WebCamTexture.devices;
    }

    public void SwapCam_Clicked()
    {
        if (WebCamTexture.devices.Length > 0)
        {
            currentCamIndex += 1;
            currentCamIndex %= WebCamTexture.devices.Length;


            // if tex is not null:
            // stop the web cam
            // start the web cam

            if (tex != null)
            {
                StopWebCam();
                // StartStopCam_Clicked();
            }
        }
    }

    public void TurnOffWhiteImage()
    {
        WhiteBackground.SetActive(false);
    }

    public void StartAndMuteCamera()
    {
        if (tex != null) // Stop the camera
        {
            StopWebCam();


        }
        else // St
        {
            LocalVideo.image.sprite = LocalVideoUnmute;
 

            foreach(var device in devices)
            {
                if (device.isFrontFacing)
                {
                    tex = new WebCamTexture(device.name);
                    display.texture = tex;

                    tex.Play();
                    FakeCamera.SetActive(true);
                }

      
            }


        }
    }

    public void StopWebCam()
    {
        LocalVideo.image.sprite = LocalVideoMute;
        display.texture = null;
        tex.Stop();
        tex = null;
        FakeCamera.SetActive(false);

    }

    public void MuteLocalAudio()
    {
        IsLocalAudioMute = !IsLocalAudioMute;

        if (IsLocalAudioMute == false)
        {
            LocalAudio.image.sprite = LocalAudioUnmute;

        }
        if (IsLocalAudioMute == true)
        {
            LocalAudio.image.sprite = LocalAudioMute;
        }
    }

    public void MuteLocalVideo()
    {
        IsLocalVideoMute = !IsLocalVideoMute;

        if (IsLocalVideoMute == false)
        {
            LocalVideo.image.sprite = LocalVideoUnmute;

        }
        if (IsLocalVideoMute == true)
        {
            LocalVideo.image.sprite = LocalVideoMute;
        }
    }

    public void MuteRemoteAudio()
    {
        IsRemoteAudioMute = !IsRemoteAudioMute;

        if (IsRemoteAudioMute == false)
        {
            RemoteAudio.image.sprite = RemoteAudioUnmute;

        }
        if (IsRemoteAudioMute == true)
        {
            RemoteAudio.image.sprite = RemoteAudioMute;
        }
    }

    public void MuteRemoteVideo()
    {
        IsRemoteVideoMute = !IsRemoteVideoMute;

        if (IsRemoteVideoMute == false)
        {
            RemoteVideo.image.sprite = RemoteVideoUnmute;

        }
        if (IsRemoteVideoMute == true)
        {
            RemoteVideo.image.sprite = RemoteVideoMute;
        }
    }


    void Start()
    {
        Invoke("TurnOffWhiteImage", 1.2f);

        if(Instance == null)
        {
            Instance = this;

        }

        StartAndMuteCamera();
    }

    // Update is called once per frame
    void Update()
    {


    }
}
