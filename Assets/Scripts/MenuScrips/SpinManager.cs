using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpinManager : MonoBehaviour
{
    int randVal;
    private float timeInterval;
    private bool isCoroutine;
    private int finalAngle;

    public Text winText;
    public int section;
    float totalAngle;
    public string[] PrizeName;
    public Button SpinButton;
    public Button CollectButton;
    [SerializeField] GameObject CollectBonusPanel;
    [SerializeField] ParticleSystem CoinParticle;
    [SerializeField] UserProfile userProfile;
    [SerializeField] GameObject spinButton;

    int amount;







    // Start is called before the first frame update
    void Start()
    {
        isCoroutine = true;
        totalAngle = 360 / section;
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private IEnumerator Spin()
    {
        spinButton.SetActive(false);
        isCoroutine = false;
        randVal = Random.Range(20, 70);
        timeInterval = 0.000001f * Time.deltaTime * 5;
        for (int i = 0; i < randVal; i++)
        {
            transform.Rotate(0, 0, (totalAngle / 2));

            // if(i>Mathf.RoundToInt(randVal*0.2f))
            // timeInterval=0.5f*Time.deltaTime;

            // if(i>Mathf.RoundToInt(randVal*0.5f))
            // timeInterval=1f*Time.deltaTime;

            if (i > Mathf.RoundToInt(randVal * 0.7f))
                timeInterval = 0.5f * Time.deltaTime;

            if (i > Mathf.RoundToInt(randVal * 0.8f))
                timeInterval = 1f * Time.deltaTime;

            if (i > Mathf.RoundToInt(randVal * 0.9f))
                timeInterval = 1.5f * Time.deltaTime;

            yield return new WaitForSeconds(timeInterval);

        }
        if (Mathf.RoundToInt(transform.eulerAngles.z) % totalAngle != 0)
            transform.Rotate(0, 0, totalAngle / 2);

        finalAngle = Mathf.RoundToInt(transform.eulerAngles.z);
 

        for (int i = 0; i < section; i++)
        {

            if (finalAngle == i * totalAngle)
                winText.text = PrizeName[i];
 
 
                
        }
        isCoroutine = true;
        SpinButton.gameObject.SetActive(false);
        CollectButton.gameObject.SetActive(true);


    }




    public void RollSpin()
    {

        StartCoroutine(Spin());

        SpinButton.enabled = true;
        winText.text = "";

        winText.gameObject.SetActive(true);





        if (SpinButton.enabled == false)
        {

            CollectButton.enabled = true;

        }


    }

    public void CollectReward()
    {
        amount = int.Parse(winText.text);
        Debug.Log(amount);
        SpinButton.gameObject.SetActive(true);
        CollectButton.gameObject.SetActive(false);
        winText.gameObject.SetActive(false);
        CollectBonusPanel.SetActive(false);
        CoinParticle.Play();
        UserProfile.instance.AddXP(5);
        userProfile.BonusWallet(amount);
       // ChessUserDatas.Instance.UpdateXP(5);
    }

 
}
