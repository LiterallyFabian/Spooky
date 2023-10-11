using System;
using System.Collections;
using UnityEngine;

namespace Spooky
{
    [RequireComponent(typeof(Collider), typeof(AudioSource))]
    public class Door : MonoBehaviour
    {
        [SerializeField] private SimonSays _simonSays;
        [SerializeField] private DropoffPoint _dropoffPoint;

        private Collider _collider;
        private AudioSource _source;

        [SerializeField] private AudioClip _doorOpen;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _source = GetComponent<AudioSource>();
            
            _source.Stop();

            if (_simonSays != null)
                _simonSays.OnGameCompleted += OpenDoor;
            else if (_dropoffPoint != null)
                _dropoffPoint.OnItemPlaced += OpenDoor;
            else 
                OpenDoor();
        }

        private void OpenDoor()
        {
            Debug.Log("Door opened");

            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = _doorOpen;
            source.Play();
            
            _collider.isTrigger = true;
            _source.loop = true;
            _source.Play();
        }
    }
}