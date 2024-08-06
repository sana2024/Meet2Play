using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Nakama;
using System.Linq;

public class SimpleLoading : MonoBehaviour {

    [SerializeField] public RectTransform rectComponent;
    public Image imageComp;
    [SerializeField] public GameObject HiddenUserImage;
    [SerializeField] GameObject PlayButton;
    public float rotateSpeed = 200f;
     ISocket isocket;
    public static SimpleLoading Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    // Use this for initialization
    void Start () {

        
        isocket =PassData.isocket;
        var mainThread = UnityMainThreadDispatcher.Instance();
        isocket.ReceivedMatchmakerMatched += match => mainThread.Enqueue(() => OnREceivedMatchmakerMatched(match));
       // rectComponent = GetComponent<RectTransform>();
        imageComp = rectComponent.GetComponent<Image>();
    }
	
	// Update is called once per frame
	void Update () {
        rectComponent.Rotate(0f, 0f, -(rotateSpeed * Time.deltaTime));
    }

    private async void OnREceivedMatchmakerMatched(IMatchmakerMatched matchmakerMatched)
    {

        var users = matchmakerMatched.Users;


        if (matchmakerMatched.Users.Count() == 2)
        {
            foreach(var user in matchmakerMatched.Users)
            {
              //  Debug.Log("users " + user.Presence.Username +"  "+user.Presence.UserId);
            }
            imageComp.gameObject.SetActive(false);
            HiddenUserImage.SetActive(false);
            PlayButton.SetActive(true);
        }
    }
}
