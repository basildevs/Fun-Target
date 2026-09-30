using UnityEngine;
using UnityEngine.Audio;
public class AudioController : Singleton<AudioController>
{

    public void PlayAudio(bool play, string audioName)
    {
        if (play)
        {
            foreach (Transform audio in transform)
            {
                if (audio.name == audioName)
                {
                    audio.GetComponent<AudioSource>().Play();
                }
            }
        }
        else
        {
            foreach (Transform audio in transform)
            {
                if (audio.name == audioName)
                {
                    if (audio.GetComponent<AudioSource>().isPlaying)
                        audio.GetComponent<AudioSource>().Stop();

                }
            }

        }
    }


}

