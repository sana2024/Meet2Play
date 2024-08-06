using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    [SerializeField] AudioSource AudioSound;
    // Start is called before the first frame update
    public void PlayAudio()
    {

        AudioSound.Play();
    }
}
