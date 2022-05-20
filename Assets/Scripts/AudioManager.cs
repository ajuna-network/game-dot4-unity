using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager 
{
    [SerializeField] private AudioSource test;


    void Play()
    {
        test.Play();
    }
}
