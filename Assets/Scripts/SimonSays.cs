using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spooky
{
    public class SimonSays : Interactable
    {
        [SerializeField] private AudioClip[] _sounds = new AudioClip[4];

        void Start()
        {
            if (_sounds.Length != 4)
                Debug.LogWarning($"An incorrect number of sounds ({_sounds.Length}) was provided.");
        }

        void Update()
        {
        }
    }
}