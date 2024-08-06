using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelHandlers : MonoBehaviour
{
    [Header("PANELS -----")]
    [SerializeField] GameObject UserProfilePanel;
    [SerializeField] GameObject LeaderBoardPanel;
    [SerializeField] GameObject FriendsPanel;
    [SerializeField] GameObject NotificationsPanel;
    [SerializeField] GameObject SettingPanel;
    [SerializeField] GameObject CollectBonusPanel;
    [SerializeField] GameObject IAPPanel;
    [SerializeField] GameObject NoEnoughCoinPanel;
    [SerializeField] GameObject SharePanel;
    [SerializeField] GameObject BackgammonPanel;
    [SerializeField] GameObject SelectionPanel;
    [SerializeField] GameObject ChessPanel;
    [SerializeField] GameObject AppleSettingsPanel;
    [SerializeField] GameObject OnlineUserPanel;
    [SerializeField] GameObject WaitingsPanel;
    [SerializeField] GameObject OpponentFindingPanel;
    [SerializeField] GameObject ChallangeRejectedPanel;
    [SerializeField] Button LogoutButton;
    //[SerializeField] GameObject BackBackgammon;
    //[SerializeField] GameObject left;
    //[SerializeField] GameObject right;

    [SerializeField] GameObject SelectTimePanel;
    public float TimePanelSmoothTime = 0.05F;
    private Vector3 TimePanelVelocity = Vector3.one;
    Vector3 TimePaneltarget;
    public static PanelHandlers instance;



    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        TimePaneltarget = SelectTimePanel.transform.localPosition;

    }

    private void Update()
    {
        if (OpponentFindingPanel.activeSelf)
        {
            WaitingsPanel.SetActive(false);
            OnlineUserPanel.SetActive(false);
        }
    }

    public void selectTime()
    {
         TimePaneltarget = new Vector3(SelectTimePanel.transform.localPosition.x, -120 , SelectTimePanel.transform.localPosition.z);
    }

    public void OpenSharePanel()
    {
        LeaderBoardPanel.SetActive(false);
        FriendsPanel.SetActive(false);
        NotificationsPanel.SetActive(false);
        CollectBonusPanel.SetActive(false);
        IAPPanel.SetActive(false);
        SettingPanel.SetActive(false);
        UserProfilePanel.SetActive(false);
        SharePanel.SetActive(true);
    }



    public void CloseSharePanel()
    {
        SharePanel.SetActive(false);
    }



    public void OpenProfilePanel()
    {
        LeaderBoardPanel.SetActive(false);
        FriendsPanel.SetActive(false);
        NotificationsPanel.SetActive(false);
        CollectBonusPanel.SetActive(false);
        IAPPanel.SetActive(false);
        SettingPanel.SetActive(false);
        UserProfilePanel.SetActive(true);
       
    }

    public void OpenLeaderBoardPanel()
    {
        LeaderBoardPanel.SetActive(true);
        FriendsPanel.SetActive(false);
        NotificationsPanel.SetActive(false);
        CollectBonusPanel.SetActive(false);
        IAPPanel.SetActive(false);
        SettingPanel.SetActive(false);
        UserProfilePanel.SetActive(false);

    }

    public void OpenFriendsPanel()
    {
        LeaderBoardPanel.SetActive(false);
        FriendsPanel.SetActive(true);
        NotificationsPanel.SetActive(false);
        CollectBonusPanel.SetActive(false);
        IAPPanel.SetActive(false);
        SettingPanel.SetActive(false);
        UserProfilePanel.SetActive(false);
    }

    public void OpenAdsPanel()
    {
        LeaderBoardPanel.SetActive(false);
        FriendsPanel.SetActive(false);
        NotificationsPanel.SetActive(false);
        CollectBonusPanel.SetActive(false);
        IAPPanel.SetActive(false);
        SettingPanel.SetActive(false);
        UserProfilePanel.SetActive(false);
    }

    public void OpenNotificationPanel()
    {
        LeaderBoardPanel.SetActive(false);
        FriendsPanel.SetActive(false);
        NotificationsPanel.SetActive(true);
        CollectBonusPanel.SetActive(false);
        IAPPanel.SetActive(false);
        SettingPanel.SetActive(false);
        UserProfilePanel.SetActive(false);
        OnlineUserPanel.SetActive(false);
    }

    public void OpenSettingsPanel()
    {
        LeaderBoardPanel.SetActive(false);
        FriendsPanel.SetActive(false);
        NotificationsPanel.SetActive(false);
        CollectBonusPanel.SetActive(false);
        IAPPanel.SetActive(false);
        SettingPanel.SetActive(true);
        UserProfilePanel.SetActive(false);
    }

    public void OpenCollectBonus()
    {
        LeaderBoardPanel.SetActive(false);
        FriendsPanel.SetActive(false);
        NotificationsPanel.SetActive(false);
        CollectBonusPanel.SetActive(true);
        IAPPanel.SetActive(false);
        SettingPanel.SetActive(false);
        UserProfilePanel.SetActive(false);
    }

    public void OpenIAPPanel()
    {
        LeaderBoardPanel.SetActive(false);
        FriendsPanel.SetActive(false);
        NotificationsPanel.SetActive(false);
        CollectBonusPanel.SetActive(false);
        IAPPanel.SetActive(true);
        SettingPanel.SetActive(false);
        UserProfilePanel.SetActive(false);
    }

    public void OpenWaitingsPanel()
    {
        //LeaderBoardPanel.SetActive(false);
        //FriendsPanel.SetActive(false);
        //NotificationsPanel.SetActive(false);
        //CollectBonusPanel.SetActive(false);
        //IAPPanel.SetActive(false);
        //SettingPanel.SetActive(false);
        //UserProfilePanel.SetActive(false);
        WaitingsPanel.SetActive(true);
    }

    public void CloseWaitingsPanel()
    {
        LeaderBoardPanel.SetActive(false);
        FriendsPanel.SetActive(false);
        NotificationsPanel.SetActive(false);
        CollectBonusPanel.SetActive(false);
        IAPPanel.SetActive(false);
        SettingPanel.SetActive(false);
        UserProfilePanel.SetActive(false);
        WaitingsPanel.SetActive(false);
    }

    public void NoEnoughMoneyClose()
    {
        NoEnoughCoinPanel.SetActive(false);
    }

    public void OpenBackgammonPanel()
    {
        LogoutButton.interactable = false;
        SelectionPanel.SetActive(false);
        BackgammonPanel.SetActive(true);
    }

    public void OpenChessPanel()
    {
        //BackBackgammon.SetActive(true);
        //left.SetActive(true);
        //right.SetActive(true);
        LogoutButton.interactable = false;
        SelectionPanel.SetActive(false);
        ChessPanel.SetActive(true);
    }

    public void BackToSelectionPanel()
    {
        //BackBackgammon.SetActive(false);
        //left.SetActive(false);
        //right.SetActive(false);
        LogoutButton.interactable = true;
        SelectionPanel.SetActive(true);
        ChessPanel.SetActive(false);
        BackgammonPanel.SetActive(false);
        TimePaneltarget = new Vector3(SelectTimePanel.transform.localPosition.x, -200, SelectTimePanel.transform.localPosition.z);
    }

    public void CloseAppleSettingsPanel()
    {
        AppleSettingsPanel.SetActive(false);
    }
 
 public void CloseChallageRejectionPanel()
    {
        ChallangeRejectedPanel.SetActive(false);
        LeaderBoardPanel.SetActive(false);
        FriendsPanel.SetActive(false);
        NotificationsPanel.SetActive(false);
        OnlineUserPanel.SetActive(false);


    }

}
