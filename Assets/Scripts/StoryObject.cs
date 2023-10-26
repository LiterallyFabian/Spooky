using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spooky
{

    public class StoryObject : Interactable
    {
        public override bool Locked => hasPlayed;
        [SerializeField] private AudioClip sound, comment;
        private AudioSource audioSource;
        [SerializeField] private Dialogue dialogue;
        private bool hasPlayed = false;

        void Awake()
        {
            audioSource = gameObject.GetComponent<AudioSource>();
            audioSource.clip = sound;
            audioSource.loop = true;
            audioSource.Play();
            audioSource.maxDistance = GameConfig.Config.SOSoundDistance;
        }


        public override void Interact()
        {
            if (!hasPlayed)
            {
                audioSource.Stop();
                dialogue.Say(comment);
                hasPlayed = true;
            }
            
        }


    }
}

