using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioClip mainBGM;
    [SerializeField] AudioClip inGameBGM;
    AudioSource audio;

    void Start()
    {
        DontDestroyOnLoad(transform.gameObject);
        audio = GetComponent<AudioSource>();
    }

    public void switchAudio(string pos)
    {
        switch (pos)
        {
            case "main":
                audio.clip = mainBGM;
                break;
            case "inGame":
                audio.clip = inGameBGM;
                break;
        }
        audio.Play();
    }
}
