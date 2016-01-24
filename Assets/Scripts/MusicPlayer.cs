﻿using UnityEngine;
using System.Collections;
using System;

public class MusicPlayer : MonoBehaviour
{

    public AudioSource MpPlayer;
    public AudioClip BGM;
    // Use this for initialization
    void Start()
    {
        MpPlayer.clip = BGM;
        MpPlayer.loop = false;
        MpPlayer.Play();
    }

    IEnumerator WaitForTrackTOendAndPlay(AudioClip toPlay)
    {
        while (MpPlayer.isPlaying)
            yield return new WaitForSeconds(0.01f);
        MpPlayer.clip = toPlay;
        MpPlayer.loop = false;
        MpPlayer.Play();
    }


    IEnumerator FadeToVolume(float volume, float fadeTime, int steps = 60)
    {
        float dif = Math.Abs(MpPlayer.volume - volume);
        float delta = dif / (float)steps;
        while (MpPlayer.volume != volume)
        {
            for (float s = 0; s <= steps; s++)
            {
                if (MpPlayer.volume <= volume)
                    MpPlayer.volume += delta;
                else
                    MpPlayer.volume -= delta;
                yield return new WaitForSeconds(fadeTime / steps);
            }
        }

    }

}