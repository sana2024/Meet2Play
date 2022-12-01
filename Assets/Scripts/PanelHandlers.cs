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
    [SerializeField] GameObject AdsPanel;
    [SerializeField] GameObject NotificationsPanel;
    [SerializeField] GameObject SettingPanel;
    [SerializeField] GameObject CollectBonusPanel;
    [SerializeField] GameObject IAPPanel;



     public void OpenProfilePanel()
    {
        LeaderBoardPanel.SetActive(false);
        FriendsPanel.SetActive(false);
        AdsPanel.SetActive(false);
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
        AdsPanel.SetActive(false);
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
        AdsPanel.SetActive(false);
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
        AdsPanel.SetActive(true);
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
        AdsPanel.SetActive(false);
        NotificationsPanel.SetActive(true);
        CollectBonusPanel.SetActive(false);
        IAPPanel.SetActive(false);
        SettingPanel.SetActive(false);
        UserProfilePanel.SetActive(false);
    }

    public void OpenSettingsPanel()
    {
        LeaderBoardPanel.SetActive(false);
        FriendsPanel.SetActive(false);
        AdsPanel.SetActive(false);
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
        AdsPanel.SetActive(false);
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
        AdsPanel.SetActive(false);
        NotificationsPanel.SetActive(false);
        CollectBonusPanel.SetActive(false);
        IAPPanel.SetActive(true);
        SettingPanel.SetActive(false);
        UserProfilePanel.SetActive(false);
    }

}
