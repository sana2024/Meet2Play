
using UnityEngine;
using UnityEngine.UI;
 

public class Test : MonoBehaviour
{
    [SerializeField] GameObject cam;
    private void Start()
    {
        RectTransform rect = cam.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector3(5, 5, 5);
       
    }


    [SerializeField] GameObject board;
    void Update()
    {
        Debug.Log(Camera.main.aspect);
       if(Camera.main.aspect <= 1.5f)
        {
            Debug.Log("ipad");
            board.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        }

        if (Camera.main.aspect >= 1.5f)
        {
            Debug.Log("iphone");
            board.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        }






    }
    

 

 

}
