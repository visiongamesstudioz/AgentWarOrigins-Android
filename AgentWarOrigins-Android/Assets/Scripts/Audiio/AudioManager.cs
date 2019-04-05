using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  UnityEngine.Audio;
namespace EndlessRunner
{
    public class AudioManager : MonoBehaviour
    {

        public static AudioManager Instance;
        public AudioMixerSnapshot MuteSoundFxSnapshot;
        public AudioMixerSnapshot UnMuteSoundFxSnapshot;
        public AudioMixerSnapshot MuteGameMusicSnapshot;
        public AudioMixerSnapshot UnMuteGameMusicSnapshot;

        public static bool IsMusicOn;
        public static bool IsSounfFxOn;
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
            if (PlayerPrefs.HasKey("IsMusicOn"))
            {
                IsMusicOn = PlayerPrefs.GetInt("IsMusicOn") == 1;
            }
            else
            {
                PlayerPrefs.SetInt("IsMusicOn", 1);
                IsMusicOn = PlayerPrefs.GetInt("IsMusicOn") == 1;
            }
            if (PlayerPrefs.HasKey("IsSoundFxOn"))
            {
                IsSounfFxOn = PlayerPrefs.GetInt("IsSoundFxOn") == 1;
            }
            else
            {
                PlayerPrefs.SetInt("IsSoundFxOn", 1);
                IsSounfFxOn = PlayerPrefs.GetInt("IsSoundFxOn") == 1;
            }

            //mute or unmute 
            if (IsMusicOn)
            {
                UnMuteGameMusic();
            }
            else
            {
                MuteGameMusic();
            }
            if (IsSounfFxOn)
            {
                UnMuteSoundFx();
            }
            else
            {
                MuteSoundFx();
            }

        }

        public void PlaySound(AudioSource audioSource, AudioClip clip, float delay)
        {
            //if (audioSource.isPlaying)
            //{
            //    audioSource.Stop();
            //}
            audioSource.clip = clip;
            audioSource.PlayDelayed(delay);
        }

        public void PlaySound(AudioSource audioSource, AudioClip clip)
        {
            if (audioSource.isPlaying)
            {
                if (audioSource.clip == clip)
                {
                    return;
                }
                audioSource.Stop();               
            }
            audioSource.clip = clip;
            audioSource.Play();
        }

        public void PlaySound(AudioSource audioSource)
        {
            if (audioSource.isPlaying)
            {                
                audioSource.Stop();
            }
            audioSource.Play();
        }

        public void StopSound(AudioSource audioSource)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
        public void PlayOneShotSound(AudioSource audioSource, AudioClip clip)
        {
            audioSource.PlayOneShot(clip);
        }

        public void MuteGameMusic()
        {

            MuteGameMusicSnapshot.TransitionTo(0.01f);
        }

        public void UnMuteGameMusic()
        {
            UnMuteGameMusicSnapshot.TransitionTo(0.01f);
        }

        public void MuteSoundFx()
        {

            MuteSoundFxSnapshot.TransitionTo(0.01f);
        }

        public void UnMuteSoundFx()
        {
            UnMuteSoundFxSnapshot.TransitionTo(0.01f);
        }

    }


}
