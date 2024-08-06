using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SplashScreen : MonoBehaviour
{
    [SerializeField] Image slider;
    [SerializeField] GameObject splashScreen;
    public bool splashEnabled = true;
    public static SplashScreen Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

    }

    void Update()
    {
        timer();
    }

    void timer()
    {
#if UNITY_IOS

        if (slider.fillAmount != 1)
        {
            slider.fillAmount += 0.02f;
        }

        if (slider.fillAmount == 1)
        {
            splashScreen.SetActive(false);
            splashEnabled = false;
        }

#endif

#if UNITY_ANDROID

        if (slider.fillAmount != 1)
        {
            slider.fillAmount += 0.01f;
        }

        if (slider.fillAmount == 1)
        {
            splashScreen.SetActive(false);
            splashEnabled = false;
        }

#endif
    }
}
