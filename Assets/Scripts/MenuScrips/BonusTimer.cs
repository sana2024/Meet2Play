using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ByteBrewSDK;

public class BonusTimer : MonoBehaviour
{
    float msTowait;
    public Button btnReward;
    private ulong lastGivenReward;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] ParticleSystem RewardParticle;
    [SerializeField] Image RewardImage;
    [SerializeField] Sprite collectOn;
    [SerializeField] Sprite collectOff;
    [SerializeField] GameObject SpinnerItems;



    private void Start()
    {
        msTowait = PassData.BonusTimer;
        btnReward = GetComponent<Button>();
        lastGivenReward = ulong.Parse(PlayerPrefs.GetString("lastGivenReward"));
        if (!rewardReady())

            btnReward.interactable = false;


    }

    private void Update()
    {

        if (!btnReward.IsInteractable())
        {
            if (rewardReady())
            {

                btnReward.interactable = true;
                return;
            }
            else
            {
                RewardImage.sprite = collectOff;
                SpinnerItems.SetActive(false);
                RewardParticle.Stop();
            }

            ulong differance = ((ulong)DateTime.Now.Ticks - lastGivenReward);


            ulong m = differance / TimeSpan.TicksPerMillisecond;
            float secondleft = (msTowait - m) / 1000.0f;


            string r = "";

            r += ((int)secondleft / 3600).ToString() + ":";
            secondleft -= ((int)secondleft / 3600) * 3600;
            r += ((int)secondleft / 60).ToString("00") + ":";
            r += (secondleft % 60).ToString("00") + "";
            text.text = r;



        }
    }

    public void OnBonusClicked()
    {
        ByteBrew.NewCustomEvent("CollectedBonus", "Username=" + name + ";");
        lastGivenReward = (ulong)DateTime.Now.Ticks;
        PlayerPrefs.SetString("lastGivenReward", lastGivenReward.ToString());
        btnReward.interactable = false;

    }


    private bool rewardReady()
    {

        ulong differance = ((ulong)DateTime.Now.Ticks - lastGivenReward);
        ulong m = differance / TimeSpan.TicksPerMillisecond;
        float secondleft = (msTowait - m) / 1000.0f;


        if (secondleft < 0)
        {
            // text.text = "COLLECT BONUS";
            RewardParticle.Play();
            RewardImage.sprite = collectOn;
            SpinnerItems.SetActive(true);
            return true;


        }
        return false;
    }
}
