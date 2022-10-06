using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{

    public GameObject [] Panel;


    public GameObject setting;
    [SerializeField] GameObject settingPanel;


   


  
public void panel(){

    Panel[0].SetActive(true);
    setting.SetActive(false);
}
    

    public void Back(){
Panel[0].SetActive(false);
Panel[1].SetActive(false);
Panel[2].SetActive(false);
Panel[3].SetActive(false);
Panel[4].SetActive(false);
Panel[5].SetActive(false);

    setting.SetActive(true);
    settingPanel.SetActive(true);
       
    }
    
  
}


