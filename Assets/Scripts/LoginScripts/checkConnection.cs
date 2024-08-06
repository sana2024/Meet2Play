using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkConnection : MonoBehaviour
{
    [SerializeField] GameObject ConnectionPanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            ConnectionPanel.SetActive(true);
        }
        else
        {
            StartCoroutine(BackOnline());
        }
    }

    IEnumerator BackOnline()
    {
        yield return new WaitForSeconds(5);
        ConnectionPanel.SetActive(false);
    }
}
