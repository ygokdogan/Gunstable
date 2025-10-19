using System.Collections;
using Managers;
using UnityEngine;

namespace Utilities
{
    public class MusicPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private float fadeDuration = 1f;

        private Coroutine currentFade;

        public void PlayMusic(AudioClip clip)
        {
            if (currentFade != null)
                StopCoroutine(currentFade);
            currentFade = StartCoroutine(FadeInMusic(clip));
        }

        private IEnumerator FadeInMusic(AudioClip newClip)
        {
            float t = 0f;
            float startVol = musicSource.volume;

            // Fade out
            while (t < fadeDuration)
            {
                t += Time.unscaledDeltaTime;
                musicSource.volume = Mathf.Lerp(startVol, 0, t / fadeDuration);
                yield return null;
            }

            musicSource.clip = newClip;
            musicSource.Play();

            // Fade in
            t = 0f;
            while (t < fadeDuration)
            {
                t += Time.unscaledDeltaTime;
                musicSource.volume = Mathf.Lerp(0, AudioManager.Instance.musicVolume, t / fadeDuration);
                yield return null;
            }
        }
    }
}

