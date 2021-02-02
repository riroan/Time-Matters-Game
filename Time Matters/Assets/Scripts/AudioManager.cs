using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioClip mainBGM;
    [SerializeField] AudioClip firstBGM;
    [SerializeField] AudioClip secondBGM;
    [SerializeField] AudioClip thirdBGM;
    [SerializeField] AudioClip winBGM;
    [SerializeField] AudioClip deathBGM;

    AudioSource audio_;

    public static AudioManager instance;

    void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(transform.gameObject);
        audio_ = GetComponent<AudioSource>();
    }

    public void switchAudio(string pos)
    {
        if (pos == "main")
            audio_.clip = mainBGM;
        else if (pos == "1F")
            audio_.clip = firstBGM;
        else if (pos == "2F")
            audio_.clip = secondBGM;
        else if (pos == "3F")
            audio_.clip = thirdBGM;
        else if (pos == "win")
            audio_.clip = winBGM;
        else if (pos == "death")
            audio_.clip = deathBGM;
        audio_.Play();
    }
}
