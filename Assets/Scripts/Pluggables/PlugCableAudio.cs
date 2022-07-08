using System;
using UnityEngine;

namespace Pluggables
{
    public class PlugCableAudio : MonoBehaviour
    {
        [SerializeField] private PlugCable plugCable;
        [SerializeField] private AudioSource audioSource;
        [Tooltip("Clips to be played when a connection creates a full connection.")]
        [SerializeField] private AudioClip[] audioConnectOn;
        [Tooltip("Clips to be played when a disconnection destroys a full connection.")]
        [SerializeField] private AudioClip[] audioDisconnectOn;
        [Tooltip("Clips to be played when a connection does not create a full connection.")]
        [SerializeField] private AudioClip[] audioConnectOff;
        [Tooltip("Clips to be played when a disconnection does not destroy a full connection.")]
        [SerializeField] private AudioClip[] audioDisconnectOff;
        [SerializeField] private AudioClip[] audioElecFailure;

        private void OnEnable()
        {
            
        }

        public void PlayRandomSound(AudioClip[] array) {
            // get a random AudioClip from the given array
            int num = UnityEngine.Random.Range(0, array.Length-1);
            AudioClip ac = array[num];
            //print("Object: " + name + " playing random clip:" + ac.name);
            // play the sound
            audioSource.time = 0f;
            audioSource.clip = ac;
            audioSource.Play();
        }

        public void PlayRandomDisconnectSound() { PlayRandomSound(audioDisconnectOn); }
        
        
        // On Unplug
        
            // if (useErrorSound)
                // PlayRandomSound(audioElecFailure);
            // else
                // PlayRandomDisconnectSound();
                
                // On Disconnect
                
                
                // Additional conditions are required to determine whether the target is on or off
                // connectionOut.PluggableEnd.PlayRandomSound(audioDisconnectOff);

                // On Connection Started
                // PlayRandomSound(audioConnectOn);
                
                // On Connection Filure
                // PlayRandomSound(audioElecFailure);
                
                // On Cable Connection
                // PlayRandomSound(audioConnectOff);
                
                // On Connection Abandoned
                
            
            // TODO: Reimplement this.
            // connection.PluggableStart.PlayRandomDisconnectSound();
    }
}