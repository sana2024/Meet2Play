using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reselution : MonoBehaviour
{
    //[SerializeField] RectTransform MyBackground;
    //[SerializeField] RectTransform OpponentBackground;
    //[SerializeField] RectTransform OpponentBackgroundLocal;
    [SerializeField] RectTransform user1;
    [SerializeField] RectTransform user2;
    [SerializeField] RectTransform user1Timer;
    [SerializeField] RectTransform user2Timer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       // Debug.Log("main aspect " + Camera.main.aspect);
        if (Camera.main.aspect <= 1.5)
        {
            Camera.main.orthographicSize = 7.56144f;
            Camera.main.transform.position = new Vector2(4, 6f);
            user1.transform.localScale = new Vector3(1.6f, 1.6f, 1.6f);
            user2.transform.localScale = new Vector3(1.6f, 1.6f, 1.6f);
            user1.anchoredPosition = new Vector2(user1.anchoredPosition.x, -22);
            user2.anchoredPosition = new Vector2(user2.anchoredPosition.x, -22);
            user1Timer.anchoredPosition = new Vector2(user1Timer.anchoredPosition.x, -154);
            user2Timer.anchoredPosition = new Vector2(user2Timer.anchoredPosition.x, -154);
            user1Timer.transform.localScale = new Vector3(0.78f, 0.78f, 0.78f);
            user2Timer.transform.localScale = new Vector3(0.78f, 0.78f, 0.78f);

            //MyBackground.transform.localScale = new Vector2(1.611626f, 0.9194911f);
            //OpponentBackground.transform.localScale = new Vector2(1.611626f, 0.9194911f);
            //OpponentBackgroundLocal.transform.localScale = new Vector2(1.611626f, 0.9194911f);
        }

        if(Camera.main.aspect > 1.4 && Camera.main.aspect < 1.5)
        {
           // Debug.Log("aspect " + Camera.main.aspect);
            //MyBackground.anchoredPosition= new Vector2(-282f, 176);
            //OpponentBackground.anchoredPosition = new Vector2(280, 176);
            //OpponentBackgroundLocal.anchoredPosition = new Vector2(280, 176);


        }

        if (Camera.main.aspect > 1.3  && Camera.main.aspect < 1.4)
        {
           // Debug.Log("aspect " + Camera.main.aspect);
            //MyBackground.anchoredPosition = new Vector2(-284, 190.5f);
            //OpponentBackground.anchoredPosition = new Vector2(278.4f, 190.5f);
            //OpponentBackgroundLocal.anchoredPosition = new Vector2(278.4f, 190.5f);
        }

        if (Camera.main.aspect > 1.7 && Camera.main.aspect < 1.9)
        {
          //  Debug.Log("aspect " + Camera.main.aspect);
            //MyBackground.anchoredPosition = new Vector2(MyBackground.anchoredPosition.x, 121);
            //OpponentBackground.anchoredPosition = new Vector2(OpponentBackground.anchoredPosition.x, 121);
            //OpponentBackgroundLocal.anchoredPosition = new Vector2(OpponentBackground.anchoredPosition.x, 121);
        }

        if (Camera.main.aspect > 2)
        {
            Camera.main.orthographicSize = 4.86f;

        }


        //if(Camera.main.aspect > 2.1 && Camera.main.aspect < 2.5)
        //{
        //    MyBackground.anchoredPosition = new Vector2(MyBackground.anchoredPosition.x, 89.9f);
        //    OpponentBackground.anchoredPosition = new Vector2(OpponentBackground.anchoredPosition.x, 89.9f);
        //}

        //if(Camera.main.aspect < 2.1)
        //{
           
        //    MyBackground.anchoredPosition = new Vector2(MyBackground.anchoredPosition.x, 97.09998f);
        //    OpponentBackground.anchoredPosition = new Vector2(OpponentBackground.anchoredPosition.x, 97.09998f);
        //}




    }
}
