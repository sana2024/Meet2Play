using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackAIResize : MonoBehaviour
{
    [SerializeField] RectTransform GameButtons;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera.main.aspect <= 1.6)
        {
            Camera.main.orthographicSize = 14.8f;
            Camera.main.transform.position = new Vector3(0.06f, -0.7f, 5.7f);
            GameButtons.transform.localPosition = new Vector3(0, -32f, 0);

        }
        }
}
