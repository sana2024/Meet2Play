using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loading : MonoBehaviour
{
    [SerializeField] GameObject DiceRotate;
    [SerializeField] GameObject LoadingPanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    } 

    private void FixedUpdate()
    {
        if (LoadingPanel.activeSelf)
        {
            var speed = 5;
            DiceRotate.transform.Rotate(Vector3.forward * speed);

        }
    }
}
