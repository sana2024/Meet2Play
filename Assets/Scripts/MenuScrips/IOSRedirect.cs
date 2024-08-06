using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IOSRedirect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OpenSettings()
    {
#if UNITY_IOS
        // Opens the iOS settings app
        Application.OpenURL("app-settings:");
#endif
    }
}
