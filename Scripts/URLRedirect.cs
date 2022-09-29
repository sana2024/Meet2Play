using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class URLRedirect : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] string Url;


    public void URL()
    {

        Application.OpenURL(Url);

    }
}
