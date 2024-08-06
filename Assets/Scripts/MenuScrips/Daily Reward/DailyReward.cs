using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class DailyReward : MonoBehaviour
{
    public Text rewardMessage;
    [SerializeField] Button ClaimButton;
    [SerializeField] GameObject DailyRewardPanel;
    [SerializeField] Text ClaimButtonText;
    private const string lastClaimKey = "LastClaimDate";
    private const string streakKey = "LoginStreak";
    DateTime lastClaimDate;
    DateTime currentDate;


    public int strk;


    public DailyItem[] DailyItems;


    void Start()
    {
        //PlayerPrefs.SetString(lastClaimKey, "01/01/0001 12:00:00 AM");
        //PlayerPrefs.SetInt(streakKey, strk);
        ClaimButton.onClick.AddListener(ClaimReward);
        CheckRewardAvailability();

    }


    private void CheckRewardAvailability()
    {
        // Make sure the time is fetched before checking
        if (TimeManager.CurrentTime == default)
        {
            StartCoroutine(WaitForTime());
            return;
        }

         lastClaimDate = GetLastClaimDate(); 
         currentDate = TimeManager.CurrentTime.Date;


        CheckStreak();

        Debug.Log("streak " + PlayerPrefs.GetInt(streakKey));
        Debug.Log(lastClaimDate);
        Debug.Log(currentDate);


        CheckAvaiavbleDate();
    }

    private void CheckStreak()
    {
        if (currentDate == lastClaimDate.AddDays(1) || currentDate == lastClaimDate)
        {
            for (int i = 0; i < PlayerPrefs.GetInt(streakKey); i++)
            {
                DailyItems[i].item_state = Item_State.claimed;
            }
        }
       
        if(currentDate > lastClaimDate.AddDays(1))
        {
            PlayerPrefs.SetInt(streakKey, 0);
        }
    }

    private void CheckAvaiavbleDate()
    {
        if (currentDate > lastClaimDate)
        {
            DailyRewardPanel.SetActive(true);
            rewardMessage.text = "You can claim your daily reward!";
            ResetStreak();
            DailyItems[PlayerPrefs.GetInt(streakKey)].item_state = Item_State.available;
        }
        else
        {
            DailyRewardPanel.SetActive(false);
            rewardMessage.text = "You have already claimed your reward today.";
            ClaimButtonText.text = "Close";
            ClaimButton.onClick.RemoveAllListeners();
            ClaimButton.onClick.AddListener(CloseDailyReward);
        }
    }

    private void ResetStreak()
    {
        //if user finished all week
        if (PlayerPrefs.GetInt(streakKey) == 7)
        {
            //reset daily reward
            PlayerPrefs.DeleteKey("LastClaimDate");
            PlayerPrefs.DeleteKey("LoginStreak");
            CheckStreak();

        }
    }

    public void ClaimReward()
    {
        var streak = PlayerPrefs.GetInt(streakKey);
        Debug.Log("you gained "+ DailyItems[streak].coins);
        PlayerPrefs.SetString("LastClaimDate", currentDate.ToString("yyyy-MM-dd"));
        streak++;
        PlayerPrefs.SetInt("LoginStreak", streak);
        DailyItems[streak - 1].item_state = Item_State.claimed;
        Debug.Log(PlayerPrefs.GetInt("LoginStreak"));
        rewardMessage.text = "Reward Claimed.";

        UserProfile.instance.BonusWallet(DailyItems[streak - 1].coins);
        ChessUserDatas.Instance.UpdateXPAsync(DailyItems[streak -1].xp);

        ClaimButtonText.text = "Close";
        ClaimButton.onClick.RemoveAllListeners();
        ClaimButton.onClick.AddListener(CloseDailyReward);
    }

    private IEnumerator WaitForTime()
    {
        yield return new WaitUntil(() => TimeManager.CurrentTime != default);
        CheckRewardAvailability();

    }

    public void CloseDailyReward()
    {
        DailyRewardPanel.SetActive(false);
    }

    public void OpenDailyReward()
    {
        DailyRewardPanel.SetActive(true);
    }

    private DateTime GetLastClaimDate()
    {
        string lastClaimDateString = PlayerPrefs.GetString(lastClaimKey, DateTime.MinValue.ToString("yyyy-MM-dd"));
        return DateTime.Parse(lastClaimDateString);
    }

}
