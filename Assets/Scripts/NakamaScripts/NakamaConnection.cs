using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using UnityEngine.Networking;

[CreateAssetMenu]
public class NakamaConnection : ScriptableObject
{
    public string scheme;
    public string host;
    public int port;
    public string serverKey;

    public IClient iclient;
 
 
    public IClient client()
    {

        iclient = new Client(scheme, host, port, serverKey, UnityWebRequestAdapter.Instance);

        var logger = new Nakama.UnityLogger(); // Implements Nakama.ILogger
        
 
        return iclient;
    }

    

}
