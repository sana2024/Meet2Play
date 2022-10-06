using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class navigate : MonoBehaviour
{

    public GameObject plane;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


    }

    public void back()
    {
        plane.SetActive(false);
    }

    public void PopPlane()
    {

        plane.SetActive(true);
    }
}
