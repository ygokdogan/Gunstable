using UnityEngine;
using Utilities;

namespace Managers
{
    using UnityEngine;
using System;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    [Header("Sound Library")]
    [SerializeField] private Sound[] sounds;

    [Header("Mixer Settings")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private void Awake()
    {
        InitializeSounds();
        // Load saved volumes
        LoadSavedVolumes();
    }

        public void Play(string name)
        {
            Sound s = Array.Find(sounds, sound => sound.name == name);
            if (s == null)
            {
                Debug.LogWarning($"Sound '{name}' not found!");
                return;
            }

            float baseVolume = s.isMusic ? musicVolume : sfxVolume;
            Debug.Log(s.source);
            s.source.volume = s.volume * baseVolume * masterVolume;
            s.source.pitch = s.pitch;

            // ✅ Don’t reassign loop/playOnAwake here
            if (!s.source.isPlaying)
                s.source.Play();
        }

        public void Stop(string name)
        {
            Sound s = Array.Find(sounds, sound => sound.name == name);
            if (s != null && s.source.isPlaying)
                s.source.Stop();
        }

        public Sound Get(string name)
        {
            Sound s = Array.Find(sounds, sound => sound.name == name);
            if (s == null)
            {
                Debug.LogWarning($"Sound '{name}' not found!");
                return null;
            }

            return s;
        }

        public void SetVolume(float master, float music, float sfx)
        {
            masterVolume = master;
            musicVolume = music;
            sfxVolume = sfx;

            foreach (Sound s in sounds)
            {
                float baseVolume = s.isMusic ? musicVolume : sfxVolume;
                s.source.volume = s.volume * baseVolume * masterVolume;
            }

            PlayerPrefs.SetFloat("MasterVol", master);
            PlayerPrefs.SetFloat("MusicVol", music);
            PlayerPrefs.SetFloat("SFXVol", sfx);
        }

        public void LoadSavedVolumes()
        {
            masterVolume = PlayerPrefs.GetFloat("MasterVol", 1f);
            musicVolume = PlayerPrefs.GetFloat("MusicVol", 1f);
            sfxVolume = PlayerPrefs.GetFloat("SFXVol", 1f);
        }
        
        private void InitializeSounds()
        {
            foreach (Sound s in sounds)
            {
                if (s.source == null)
                {
                    s.source = gameObject.AddComponent<AudioSource>();
                    s.source.clip = s.clip;
                    s.source.loop = s.loop;
                    s.source.playOnAwake = s.playOnAwake;
                    s.source.volume = s.volume;
                    s.source.pitch = s.pitch;

                    if (s.playOnAwake)
                        s.source.Play();
                }
            }
        }
    }

}
