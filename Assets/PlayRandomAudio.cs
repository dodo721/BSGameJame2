using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayRandomAudio : MonoBehaviour
{
    public AudioSource Src;
    public bool isPlaying;

    public float uiVolume;
    public float Volume;

    public int currentlyPlaying;
    public int lastPlayed = 6;
    public AudioClip[] Song;

    private void Start()
    {
        if (isPlaying)
        {
            int randomInt = Random.Range(0, 4);
            Src.clip = Song[randomInt];
            currentlyPlaying = randomInt;
            lastPlayed = 6;
            Src.Play();
        }
    }


    void Update()
    {
        if (isPlaying && !Src.isPlaying)
        {
            int randomInt = Random.Range(0, 4);
            if (randomInt != currentlyPlaying && randomInt != lastPlayed)
            {
                lastPlayed = currentlyPlaying;
                Src.clip = Song[randomInt];
                currentlyPlaying = randomInt;
                Src.Play();
            }
        }

        if (Volume != uiVolume)
        {
            Volume = uiVolume;
            Src.volume = Volume;
        }
    }

    public void StopAudio()
    {
        isPlaying = false;
        Src.Stop();
    }
}
