using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonusTimer : MonoBehaviour
{
    public float msTowait;
    public Button btnReward;
    private ulong lastGivenReward;
   [SerializeField] Text text;


    private void Start()
    {
        btnReward = GetComponent<Button>();
        lastGivenReward = ulong.Parse(PlayerPrefs.GetString("lastGivenReward"));
        text = GetComponentInChildren<Text>();
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

            ulong differance = ((ulong)DateTime.Now.Ticks - lastGivenReward);


            ulong m = differance / TimeSpan.TicksPerMillisecond;
            float secondleft = (msTowait - m) / 1000.0f;


            string r = "";

            r += ((int)secondleft / 3600).ToString() + "h";
            secondleft -= ((int)secondleft / 3600) * 3600;
            r += ((int)secondleft / 60).ToString("00") + "m";
            r += (secondleft % 60).ToString("00") + "s";
            text.text = r;



        }
    }

    public void OnBonusClicked()
    {
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
            text.text = "COLLECT BONUS";
            return true;


        }
        return false;
    }
}
