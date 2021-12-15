using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource musicJukeBox;
    public AudioClip musicClip;

    public AudioSource soundJukeBox;
    public AudioClip[] audioClips;

    bool musicStopped = false;

    public static MusicManager instance { private set; get; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
    }

    public void PlayMusic()
    {
        musicStopped = false;

        if (musicJukeBox.clip == null)
            musicJukeBox.clip = musicClip;

        musicJukeBox.Play();
    }

    public void StopMusic()
    {
        musicJukeBox.Stop();

        musicStopped = true;
    }

    public void Pause(bool unpause = false)
    {
        if (musicJukeBox.clip == null)
            musicJukeBox.clip = musicClip;

        if (unpause)
            musicJukeBox.UnPause();
        else
            musicJukeBox.Pause();
    }

    public IEnumerator PlaySound(Sounds s)
    {
        bool musicPlaying = musicJukeBox.isPlaying;

        if (musicPlaying)
            musicJukeBox.Pause();

        switch (s)
        {
            case Sounds.menuOver:
                soundJukeBox.PlayOneShot(audioClips[0]);
                break;
            case Sounds.menuSelect:
                soundJukeBox.PlayOneShot(audioClips[1]);
                break;
            case Sounds.resetB:
                soundJukeBox.PlayOneShot(audioClips[2]);
                break;
            case Sounds.resetS:
                soundJukeBox.PlayOneShot(audioClips[3]);
                break;
            case Sounds.pierceB:
                soundJukeBox.PlayOneShot(audioClips[4]);
                break;
            case Sounds.pierceS:
                soundJukeBox.PlayOneShot(audioClips[5]);
                break;
            case Sounds.winB:
                soundJukeBox.PlayOneShot(audioClips[6]);
                break;
            case Sounds.winS:
                soundJukeBox.PlayOneShot(audioClips[7]);
                break;
            case Sounds.pointB:
                soundJukeBox.PlayOneShot(audioClips[8]);
                break;
            case Sounds.pointS:
                soundJukeBox.PlayOneShot(audioClips[9]);
                break;
            case Sounds.credits:
                soundJukeBox.PlayOneShot(audioClips[10]);
                break;
            case Sounds.logo:
                soundJukeBox.PlayOneShot(audioClips[11]);
                break;
            case Sounds.exit:
                soundJukeBox.PlayOneShot(audioClips[12]);
                break;
        }

        yield return new WaitForSeconds(0.35f);

        if (!musicStopped && musicPlaying)
            musicJukeBox.UnPause();
    }
}
