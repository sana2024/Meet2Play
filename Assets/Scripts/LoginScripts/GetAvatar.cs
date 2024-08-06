using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GetAvatar : MonoBehaviour
{

    public static GetAvatar Instance;
    private string imageUrl;
    // Start is called before the first frame update

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }


   public IEnumerator GetAvatarUrl(string imageName )
    {
       
        UnityWebRequest www = UnityWebRequest.Get("http://3.253.14.136:8000/api/get-image-url/"+imageName);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Something went wrong: " + www.error);
        }
        else
        {
            Avatar avatar = JsonUtility.FromJson<Avatar>(www.downloadHandler.text);
            PassData.MyURL = avatar.url;
            Debug.Log(avatar.url);

            
        }

       
    }



}


[Serializable]
public class Avatar
{
    public string url;

}