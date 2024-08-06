using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateBoard : MonoBehaviour
{

    [SerializeField] GameObject camera;

   public static RotateBoard Instance;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
     //   rotateChessBoard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   public void rotateChessBoard()
    {
        camera.transform.Rotate(0,0,180);
    }
}
