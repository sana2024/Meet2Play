using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchseButton : MonoBehaviour
{
  [SerializeField] UserProfile userProfile;
 
  public enum Purchasetype{coin16500,coin30000,coin625000,coin1750000};
    public Purchasetype purchasetype;
    [SerializeField] GameObject LoadingPanel;


public void clickPurchaseType(){

   LoadingPanel.SetActive(true);
  switch(purchasetype){

    case Purchasetype.coin16500:

    IAPManager.instance.Buycoin16500();
 
 
    break;
    case Purchasetype.coin30000:

    IAPManager.instance.Buycoin30000();
   

    break;
    case Purchasetype.coin625000:

    IAPManager.instance.Buycoin625000();
   

    break;
    case Purchasetype.coin1750000:

    IAPManager.instance.Buycoin1750000();
 

    break;



  }
}

 


}
