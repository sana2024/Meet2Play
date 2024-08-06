using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IAPManager : MonoBehaviour, IStoreListener
{
    public static IAPManager instance;

    private static IStoreController m_StoreController;
    private static IExtensionProvider m_StoreExtensionProvider;
    [SerializeField] GameObject LoadingPanel;

    //Step 1 create your products

    private Text text;
    private string coin16500 = "100_coins";
    private string coin30000 = "300_coins";
    private string coin625000 = "500_coins";
    private string coin1750000 = "1000_coins";

    [SerializeField] UserProfile userProfile;
    [SerializeField] InGameData inGameData;
 

 



    public void InitializePurchasing()
    {
        if (IsInitialized()) { return; }
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct(coin16500, ProductType.Consumable);
        builder.AddProduct(coin30000, ProductType.Consumable);
        builder.AddProduct(coin625000, ProductType.Consumable);
        builder.AddProduct(coin1750000, ProductType.Consumable);



        UnityPurchasing.Initialize(this, builder);
    }


    private bool IsInitialized()
    {
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    public void Buycoin16500()
    {
        BuyProductID(coin16500);

        //UserProfile.instance.AddXP(50);
        //UserProfile.instance.BonusWallet(16500);
        //ChessUserDatas.Instance.UpdateXP(50);  
            
    }
    public void Buycoin30000()
    {
        BuyProductID(coin30000);
        //UserProfile.instance.AddXP(100);
        //UserProfile.instance.BonusWallet(30000);
       // ChessUserDatas.Instance.UpdateXP(100);
    }
    public void Buycoin625000()
    {
        BuyProductID(coin625000);
        //UserProfile.instance.AddXP(500);
        //UserProfile.instance.BonusWallet(625000);
        //ChessUserDatas.Instance.UpdateXP(500);

    }
    public void Buycoin1750000()
    {
        BuyProductID(coin1750000);
        //UserProfile.instance.AddXP(2000);
        //UserProfile.instance.BonusWallet(1750000);
       // ChessUserDatas.Instance.UpdateXP(2000);
    }




    //Step 4 modify purchasing
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        if (String.Equals(args.purchasedProduct.definition.id, coin16500, StringComparison.Ordinal))

        {
 
            Debug.Log("Give Player coin16500 ");
            LoadingPanel.SetActive(false);
            UserProfile.instance.AddXP(50);
            UserProfile.instance.BonusWallet(16500);

        }
        else if (String.Equals(args.purchasedProduct.definition.id, coin30000, StringComparison.Ordinal))
        {
 
            Debug.Log("Give Player coin30000 ");
            LoadingPanel.SetActive(false);
            UserProfile.instance.AddXP(100);
            UserProfile.instance.BonusWallet(30000);
        }
        else if (String.Equals(args.purchasedProduct.definition.id, coin625000, StringComparison.Ordinal))
        {
 
            Debug.Log("Give Player coin625000 ");
            LoadingPanel.SetActive(false);
            UserProfile.instance.AddXP(500);
            UserProfile.instance.BonusWallet(625000);
        }
        else if (String.Equals(args.purchasedProduct.definition.id, coin1750000, StringComparison.Ordinal))
        {
 
            Debug.Log("Give Player coin1750000 ");
            LoadingPanel.SetActive(false);
            UserProfile.instance.AddXP(2000);
            UserProfile.instance.BonusWallet(1750000);
        }
        else
        {
            LoadingPanel.SetActive(false);
            Debug.Log("Purchase Failed");
        }
        return PurchaseProcessingResult.Complete;
    }











    private void Awake()
    {
        TestSingleton();
    }

    void Start()
    {
        if (m_StoreController == null) { InitializePurchasing(); }

 

         
       
    }

    public void UpdateWallet(int amount)
    {

        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;

        if(sceneName == "Menu")
        {
            userProfile.BonusWallet(amount);
        }
        if(sceneName == "GameScene")
        {
            inGameData.BonusWallet(amount);
        }
    }

 

    private void TestSingleton()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void BuyProductID(string productId)
    {
        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(productId);
            if (product != null && product.availableToPurchase)
            {

                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
             
                m_StoreController.InitiatePurchase(product);
 
            }
            else
            {
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        else
        {
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }

    public void RestorePurchases()
    {
        if (!IsInitialized())
        {
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            Debug.Log("RestorePurchases started ...");

            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            apple.RestoreTransactions((result) => {
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        else
        {
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("OnInitialized: PASS");
        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        LoadingPanel.SetActive(false);
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        throw new NotImplementedException();
    }
}