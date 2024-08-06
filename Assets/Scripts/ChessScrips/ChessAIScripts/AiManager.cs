using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AiManager : MonoBehaviour
{
 
    public bool isBlackStockfish;
    public bool isWhiteStockfish;
    public huggingFaceStock stocky;
    // Start is called before the first frame update
    void Start()
    {
        isWhiteStockfish = false;
        isBlackStockfish = true;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
 
}
