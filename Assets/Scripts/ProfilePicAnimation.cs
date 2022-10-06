using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfilePicAnimation : MonoBehaviour
{

    [SerializeField] Image ImageSelector;

    float speed = 1000;

    // Start is called before the first frame update

    public void Update()
    {
        if(transform.position.y < -320)
        {

            transform.Translate(Vector2.up * speed * Time.deltaTime);

        }
        if (transform.position.y > 467)
        {
           transform.Translate(Vector2.down * speed * Time.deltaTime);
        }
    
        

    }


}
