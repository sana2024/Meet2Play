using UnityEngine;
using UnityEngine.UI;
using agora_gaming_rtc;
using agora_utilities;
using UnityEngine.Apple;
using UnityEngine.SceneManagement;

public class HelloVideoAgora : MonoBehaviour
{

    [SerializeField]
    private string APP_ID = "";

    public Text LogText;
    public bool UseCustomSink = false;

    private Logger _logger;
    private IRtcEngine _rtcEngine = null;
    private const float _offset = 100;
    private uint RemoteUID;

    //booleans for mute buttons
   public  bool localAudio = false;
   public  bool localvideo = false;
   public  bool remoteAudio = false;
   public  bool remoteVideo = false;
    public int NetworkQuality;

    [SerializeField] Button AudioButtonLocal;
    [SerializeField] Button VideoButtonLocal;

    [SerializeField] Button AudioButtonRemote;
    [SerializeField] Button VideoButtonRemote;

    [SerializeField] Sprite LoaclAudioMuteOn;
    [SerializeField] Sprite LoaclAudioMuteOff;

    [SerializeField] Sprite RemoteAudioMuteOn;
    [SerializeField] Sprite RemoteAudioMuteOff;

    [SerializeField] Sprite VideoMuteOn;
    [SerializeField] Sprite VideoMuteOff;

 



    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject LocalBackground;
    [SerializeField] GameObject RemoteBackground;
    public static HelloVideoAgora instance;


    GameObject Player1;
    GameObject Player2;

    public void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }
    }


    // Use this for initialization
    void Start()
    {
        if (CheckAppId())
        {
            InitEngine();
            JoinChannel();

        }
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_IOS

        PermissionHelper.RequestMicrophontPermission();
        PermissionHelper.RequestCameraPermission();


#endif


#if UNITY_STANDALONE_OSX

        PermissionHelper.RequestMicrophontPermission();
        PermissionHelper.RequestCameraPermission();

        Application.RequestUserAuthorization(UserAuthorization.WebCam);
#endif

    }

    bool CheckAppId()
    {
        _logger = new Logger(LogText);
        return _logger.DebugAssert(APP_ID.Length > 10, "Please fill in your appId in VideoCanvas!!!!!");
    }

   public void InitEngine()
    {
  
       _rtcEngine = IRtcEngine.GetEngine(APP_ID);
        _rtcEngine.SetLogFile("log.txt");
        _rtcEngine.SetChannelProfile(CHANNEL_PROFILE.CHANNEL_PROFILE_LIVE_BROADCASTING);
        _rtcEngine.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);
        if (UseCustomSink)
        {
            _rtcEngine.SetExternalAudioSink(true, 44100, 1);
        }

        _rtcEngine.OnJoinChannelSuccess += OnJoinChannelSuccessHandler;
        _rtcEngine.OnLeaveChannel += OnLeaveChannelHandler;
        _rtcEngine.OnWarning += OnSDKWarningHandler;
        _rtcEngine.OnError += OnSDKErrorHandler;
        _rtcEngine.OnConnectionLost += OnConnectionLostHandler;
        _rtcEngine.OnUserJoined += OnUserJoinedHandler;
        _rtcEngine.OnUserOffline += OnUserOfflineHandler;
        _rtcEngine.OnNetworkQuality += OnTestNetworkQuality;
        _rtcEngine.OnNetworkTypeChanged += OnNetworkTypeChanged;
       




    }
 

    private void OnNetworkTypeChanged( NETWORK_TYPE type)
    {
        Debug.Log("network type " + type);
        if (SceneManager.GetActiveScene().name == "Game")
        {
            gameManager.InVokeConnect();
        }

    }

    private void OnTestNetworkQuality(uint uid, int txQuality, int rxQuality)
    {
        NetworkQuality = txQuality;
    }

    public void JoinChannel()
    {
        _rtcEngine.EnableAudio();
        _rtcEngine.EnableVideo();
        _rtcEngine.EnableVideoObserver();
        _rtcEngine.JoinChannel(PassData.Match.Id, null, 0);

    }

    void OnJoinChannelSuccessHandler(string channelName, uint uid, int elapsed)
    {
        _logger.UpdateLog(string.Format("sdk version: ${0}", IRtcEngine.GetSdkVersion()));
        _logger.UpdateLog(string.Format("onJoinChannelSuccess channelName: {0}, uid: {1}, elapsed: {2}", channelName, uid, elapsed));
        makeVideoView1(0);
    }

    void OnLeaveChannelHandler(RtcStats stats)
    {
        _logger.UpdateLog("OnLeaveChannelSuccess");
        DestroyVideoView(0);
    }

    void OnUserJoinedHandler(uint uid, int elapsed)
    {
        _logger.UpdateLog(string.Format("OnUserJoined uid: ${0} elapsed: ${1}", uid, elapsed));
        makeVideoView2(uid);
        RemoteUID = uid;
    }

    void OnUserOfflineHandler(uint uid, USER_OFFLINE_REASON reason)
    {
        //Debug.Log("user offline "+reason);
        _logger.UpdateLog(string.Format("OnUserOffLine uid: ${0}, reason: ${1}", uid, (int)reason));
        DestroyVideoView(uid);
    }

    void OnSDKWarningHandler(int warn, string msg)
    {
        Debug.Log(warn + " " + msg);
    }

    void OnSDKErrorHandler(int error, string msg)
    {
        _logger.UpdateLog(string.Format("OnSDKError error: {0}, msg: {1}", error, msg));
    }

    void OnConnectionLostHandler()
    {
        _logger.UpdateLog(string.Format("OnConnectionLost "));
    }

    public void OnApplicationQuit()
    {
        _rtcEngine.MuteLocalAudioStream(localAudio);
        Debug.Log("OnApplicationQuit");
        if (_rtcEngine != null)
        {
            _rtcEngine.LeaveChannel();
            _rtcEngine.DisableVideoObserver();
            IRtcEngine.Destroy();
        }
    }
 

    public void DestroyVideoView(uint uid)
    {
        GameObject go = GameObject.Find(uid.ToString());
        if (!ReferenceEquals(go, null))
        {
            Object.Destroy(go);
        }
    }

    private void makeVideoView1(uint uid)
    {
        GameObject go = GameObject.Find("Player1Call");

        if (!ReferenceEquals(go, null))
        {
            return; // reuse
        }

        // create a GameObject and assign to this new user
        VideoSurface videoSurface = makeImageSurface(uid.ToString());
        if (!ReferenceEquals(videoSurface, null))
        {
            // configure videoSurface
            videoSurface.SetForUser(uid);
            videoSurface.SetEnable(true);
            videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
        }
    }
    private void makeVideoView2(uint uid)
    {
        GameObject go = GameObject.Find("Player2Call");
        if (!ReferenceEquals(go, null))
        {
            return; // reuse
        }

        // create a GameObject and assign to this new user
        VideoSurface videoSurface = makeImageSurface2(uid.ToString());
        if (!ReferenceEquals(videoSurface, null))
        {
            // configure videoSurface
            videoSurface.SetForUser(uid);
            videoSurface.SetEnable(true);
            videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
        }
    }


    // Video TYPE 2: RawImage
    public VideoSurface makeImageSurface(string goName)
    {
        GameObject go = new GameObject();

        if (go == null)
        {
            return null;
        }

        go.name = "Player1";
        // to be renderered onto
        go.AddComponent<RawImage>();
        // make the object draggable
 
        GameObject canvas = GameObject.Find("CameraPlace1");
        if (canvas != null)
        {
            go.transform.SetParent(canvas.transform);
 
        }
        else
        {
            Debug.Log("Canvas is null video view");
        }
        go.transform.Rotate(0, 0, -180f);

        go.transform.localScale = new Vector3(1.42f, 0.83f, 1.394179f);

        RectTransform uitransform = go.GetComponent<RectTransform>();
        uitransform.anchorMin = new Vector2(0.5f, 0.5f);
        uitransform.anchorMax = new Vector2(0.5f, 0.5f);
        uitransform.pivot = new Vector2(0.5f, 0.5f);
        go.transform.localPosition = new Vector3(0, 0, 0);
 

        // configure videoSurface
        VideoSurface videoSurface = go.AddComponent<VideoSurface>();
        return videoSurface;
    }

    public VideoSurface makeImageSurface2(string goName)
    {
        GameObject go = new GameObject();

        if (go == null)
        {
            return null;
        }

        go.name = "Player2";
        // to be renderered onto
        go.AddComponent<RawImage>();
        // make the object draggable
 
        GameObject canvas = GameObject.Find("CameraPlace2");
        if (canvas != null)
        {
            go.transform.SetParent(canvas.transform);
 
        }
        else
        {
            Debug.Log("Canvas is null video view");
        }


        go.transform.Rotate(0, 0, -180f);
        go.transform.localScale = new Vector3(1.42f, 0.83f, 1.394179f);

        RectTransform uitransform = go.GetComponent<RectTransform>();
        uitransform.anchorMin = new Vector2(0.5f, 0.5f);
        uitransform.anchorMax = new Vector2(0.5f, 0.5f);
        uitransform.pivot = new Vector2(0.5f, 0.5f);
        go.transform.localPosition = new Vector3(0, 0, 0);

        // configure videoSurface
        VideoSurface videoSurface = go.AddComponent<VideoSurface>();
        return videoSurface;
    }

    // we will create a send match state function in the video class so we don't have any dependency on gamemanager
    public async void SendMatchState(long opCode, string state)
    {

        try
        {
            Debug.Log(PassData.OtherPresence.Username);
            await PassData.isocket.SendMatchStateAsync(PassData.Match.Id, opCode, state, new[] { PassData.OtherPresence });
        }
        catch (Nakama.ApiResponseException ex)
        {
            Debug.Log("send game manager " + ex);
        }
    }

    #region Mute Actions


    public void muteUserAudio()
    {
        localAudio = !localAudio;
        _rtcEngine.MuteLocalAudioStream(localAudio);

        if (localAudio)
        {
        AudioButtonLocal.image.sprite = LoaclAudioMuteOn;
        }
        else
        {
            AudioButtonLocal.image.sprite = LoaclAudioMuteOff;
        }

    }


    public void muteUserVideo()
    {
        localvideo = !localvideo;
 

        _rtcEngine.MuteLocalVideoStream(localvideo);
        LocalBackground.SetActive(localvideo);

         SendMatchState(OpCodes.Camera_Background, MatchDataJson.SetCameraBackground(localvideo));

        if (localvideo) {
            VideoButtonLocal.image.sprite = VideoMuteOn;
         }
        else
        {
            VideoButtonLocal.image.sprite = VideoMuteOff;
        }
    }

    public void muteRemoteAudio()
    {
        remoteAudio = !remoteAudio;
       _rtcEngine.MuteRemoteAudioStream(RemoteUID, remoteAudio);

        if (remoteAudio)
        {
            AudioButtonRemote.image.sprite = RemoteAudioMuteOn;
        }
        else
        {
            AudioButtonRemote.image.sprite = RemoteAudioMuteOff;
        }


    }

    public void muteRemoteVedio()
    {
        remoteVideo = !remoteVideo;
        _rtcEngine.MuteAllRemoteVideoStreams(remoteVideo);
        RemoteBackground.SetActive(remoteVideo);


        if (remoteVideo)
        {
            VideoButtonRemote.image.sprite = VideoMuteOn;
        }
        else
        {
            VideoButtonRemote.image.sprite = VideoMuteOff;
        }


    }
      

    #endregion
}