// using UnityEngine.Audio;
// using System;
// using UnityEngine;

// public class AudioManager : MonoBehaviour
// {

//     public Sound[] AudioSound;

//     // Start is called before the first frame update
//     void Awake()
//     {
//         foreach (Sound s in AudioSound)
//         {
//             s.source = gameObject.AddComponent<AudioSource>();
//             s.source.clip = s.clip;
//             s.source.volume = s.volume;
//             s.source.pitch = s.pitch;
//         }
//     }

//     public void Play (string name)
//     {
//         Sound s = Array.Find(AudioSound, AudioSound => AudioSound.name == name);
//         if (!s.source.isPlaying())
//         {
//             s.source.Play();
//         }
        
//     }
// }
