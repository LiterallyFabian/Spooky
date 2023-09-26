using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spooky
{

    public class StoryObject : Interactable
    {
        [SerializeField] private AudioClip sound, dialogue;
        [SerializeField] private AudioSource playerAudioSource;
        private AudioSource audioSource;

        void Awake()
        {
            audioSource = gameObject.GetComponent<AudioSource>();
            audioSource.clip = sound;
            audioSource.loop = true;
            audioSource.Play();
        }

        void Update()
        {
            
        }

        public override void Interact()
        {
            audioSource.Pause();
            playerAudioSource.clip = dialogue;
            playerAudioSource.Play();
        }


    }
}

