using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectBonus : MonoBehaviour
{
    [SerializeField] GameObject BonusPanel;
 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenBonusPanel()
    {
        BonusPanel.SetActive(true);
    }

    public void CloseBonusPanel()
    {
        BonusPanel.SetActive(false);
    }
}
