using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InAppPurchase : MonoBehaviour
{
    [SerializeField] GameObject IAPpanel;
  

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenIAPpanel()
    {
        IAPpanel.SetActive(true);
    }

    public void CloseIAPpanel()
    {
        IAPpanel.SetActive(false);
    }
}
