using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Nakama;

public class SimpleLoading : MonoBehaviour {

    private RectTransform rectComponent;
    private Image imageComp;
    [SerializeField] GameObject HiddenUserImage;
    [SerializeField] GameObject PlayButton;
    public float rotateSpeed = 200f;
     ISocket isocket;

    // Use this for initialization
    void Start () {

        
        isocket =PassData.isocket;
        var mainThread = UnityMainThreadDispatcher.Instance();
        isocket.ReceivedMatchmakerMatched += match => mainThread.Enqueue(() => OnREceivedMatchmakerMatched(match));
        rectComponent = GetComponent<RectTransform>();
        imageComp = rectComponent.GetComponent<Image>();
    }
	
	// Update is called once per frame
	void Update () {
        rectComponent.Rotate(0f, 0f, -(rotateSpeed * Time.deltaTime));
    }

    private async void OnREceivedMatchmakerMatched(IMatchmakerMatched matchmakerMatched)
    {

        var users = matchmakerMatched.Users;

        Debug.Log(matchmakerMatched.Self.Presence.Username);
       
        
        foreach(var u in users)
        {

            if(matchmakerMatched.Self.Presence.UserId != u.Presence.UserId)
            {
        
                Debug.Log(u.Presence.Username);
             
            


            var ids = new[] { u.Presence.UserId};
            var result = await PassData.iClient.GetUsersAsync(PassData.isession, ids);

            foreach (var user in result.Users)
            {
                Debug.Log(user.AvatarUrl);
                


            }
        }

        }
        imageComp.gameObject.SetActive(false);
        HiddenUserImage.SetActive(false);
        PlayButton.SetActive(true);

    }
}
