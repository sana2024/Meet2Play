using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeSlots : MonoBehaviour
{
    [SerializeField] GameObject Section1;
    [SerializeField] GameObject Section2;
    [SerializeField] GameObject Section3;
    [SerializeField] GameObject Section4;
    [SerializeField] GameObject Boards;

    [SerializeField] ThrowLocation DiceLocation;
    [SerializeField] GameObject ThrowLocation;

    [SerializeField] GameObject whiteSlot;
    [SerializeField] GameObject BlackSlot;

    

    private void Start()
    {

 
    }

    public void rotate()
    {
         Boards.transform.Rotate(180, 0, 0);
         ThrowLocation.transform.position = new Vector2(2.85f, -3);

      //  whiteSlot.transform.position = new Vector2(4.11f,-3.41f);
      //  BlackSlot.transform.position = new Vector2(5.06f, 4.46f);




    }

    public void Resize()
    {
        Section1.transform.position = new Vector3(Section1.transform.position.x, 0.1f, Section1.transform.position.z);
        Section2.transform.position = new Vector3(Section2.transform.position.x, 0.1f, Section2.transform.position.z);
        Section3.transform.position = new Vector3(Section3.transform.position.x, -0.1f, Section3.transform.position.z);
        Section4.transform.position = new Vector3(Section4.transform.position.x, -0.1f, Section3.transform.position.z);
    }

   

}
