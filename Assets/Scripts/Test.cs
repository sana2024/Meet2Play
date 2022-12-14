
using UnityEngine;
using UnityEngine.UI;
using Nakama;
using System.Collections;
using UnityEngine.Networking;
using Nakama.TinyJson;

public class Test : MonoBehaviour
{
    [SerializeField] Text text;

    private void OnApplicationFocus(bool focus)
    {
        Debug.Log("on focus");
    }



}
