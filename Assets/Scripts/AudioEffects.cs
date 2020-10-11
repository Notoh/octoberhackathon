using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEffects : MonoBehaviour
{
    public AudioSource Music;
    void Start()
    {
        
    }

    void Update()
    {
        if (Music.pitch <= 1)
        {
            Music.pitch += 0.01f;
        }
    }
}
