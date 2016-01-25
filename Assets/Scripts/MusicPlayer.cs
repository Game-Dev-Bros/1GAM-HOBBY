using UnityEngine;
using System.Collections;
using System;

public class MusicPlayer : MonoBehaviour
{

    public AudioSource MpPlayer;
    public AudioSource Footsteps;
    public AudioSource Interaction;

    IEnumerator WaitForTrackToEndAndPlay(AudioClip toPlay)
    {
        while (MpPlayer.isPlaying)
            yield return new WaitForSeconds(0.01f);
        MpPlayer.clip = toPlay;
        MpPlayer.Play();
    }

    public void StopMusic()
    {
        MpPlayer.Stop();
    }

    public void PlayFootsteps()
    {
        if (!Footsteps.isPlaying)
        {
            Footsteps.Play();
        }
    }
    
    public void StopLoopedFootsteps()
    {
        Footsteps.loop = false;
        Footsteps.Stop();
    }

    public IEnumerator PlayLoopedFootsteps()
    {
        while(Footsteps.isPlaying)
        {
            yield return new WaitForEndOfFrame();
        }

        Footsteps.loop = true;
        Footsteps.Play();
    }

    public void PlayInteraction()
    {
        if (!Interaction.isPlaying)
        {
            Interaction.loop = false;
            Interaction.Play();
        }
    }

    public void SetMusicVolume(float vol)
    {
        MpPlayer.volume = vol;
    }

    public float GetMusicVolume()
    {
        return MpPlayer.volume;
    }

    public IEnumerator FadeToVolume(float volume, float fadeTime, int steps = 60)
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
