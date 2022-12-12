
using UnityEngine;
using UnityEngine.UI;
using Nakama;
using System.Collections;
using UnityEngine.Networking;
using Nakama.TinyJson;

public class Test : MonoBehaviour
{
    [SerializeField] NakamaConnection Nconnect;
    ISession isession;
    IClient iclient;
    ISocket isocket;

    public string email;
    public string username;

    private void Start()
    {
        string str = "2.586094";
        Debug.Log(str.ToJson());

         
    }




}
