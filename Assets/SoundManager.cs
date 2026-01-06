using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    
    [SerializeField] private AudioSource soundObject;
    
    //Maps clips to currently playing audio sources
    private Dictionary<AudioClip, List<AudioSource>> soundsPlaying = new Dictionary<AudioClip, List<AudioSource>>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySoundEffect(AudioClip clip, Transform pos, float volume)
    {
        AudioSource audioSource = Instantiate(soundObject, pos.position, Quaternion.identity);
        
        audioSource.clip = clip;
        
        audioSource.volume = volume;
        
        audioSource.Play();
        
        // Track this audio source
        if (!soundsPlaying.ContainsKey(clip))
        {
            soundsPlaying[clip] = new List<AudioSource>();
        }
        soundsPlaying[clip].Add(audioSource);
        
        float clipLength = audioSource.clip.length;
        StartCoroutine(StopSourceAfterDelay(audioSource, clipLength));
    }

    public void PlayRandomSoundEffect(AudioClip[] clip, Transform pos, float volume)
    {
        AudioClip randomChosen = clip[Random.Range(0, clip.Length)];
        PlaySoundEffect(randomChosen, pos, volume);
    }

    public void StopSoundEffect(AudioClip clip)
    {
        if (soundsPlaying.ContainsKey(clip))
        {
            foreach (AudioSource source in soundsPlaying[clip])
            {
                if (source != null)
                {
                    source.Stop();
                    Destroy(source.gameObject);
                }
            }
            soundsPlaying[clip].Clear();
            soundsPlaying.Remove(clip);
        }
    }
    
    private IEnumerator StopSourceAfterDelay(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (source != null && source.clip != null)
        {
            if (soundsPlaying.ContainsKey(source.clip))
            {
                soundsPlaying[source.clip].Remove(source);
                
                if (soundsPlaying[source.clip].Count == 0)
                {
                    soundsPlaying.Remove(source.clip);
                }
            }
        }
        
        Destroy(source.gameObject);
    }

    public bool IsPlayingSound(AudioClip clip)
    {
        return soundsPlaying.ContainsKey(clip);
    }
}
