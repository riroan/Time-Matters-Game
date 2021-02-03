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

    public AudioSource audio_;

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
        {
            audio_.clip = mainBGM;
            audio_.volume = 1f;
        }
        else if (pos == "1F")
            audio_.clip = firstBGM;
        else if (pos == "2F")
            audio_.clip = secondBGM;
        else if (pos == "3F")
            audio_.clip = thirdBGM;
        else if (pos == "win")
        {
            audio_.clip = winBGM;
            audio_.volume = 1f;
        }
        else if (pos == "death")
        {
            audio_.clip = deathBGM;
            audio_.volume = 1f;
        }
        audio_.Play();
    }
}
