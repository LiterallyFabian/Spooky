using System;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

namespace Spooky
{
    [RequireComponent(typeof(AudioSource))]
    public class PickupPoint : Interactable
    {
        /// <summary>
        /// Whether or not the item at this point has been picked up.
        /// If true, the associated <see cref="DropoffPoint"/> will be able to be interacted with.
        /// </summary>
        public bool InteractedWith { get; private set; }

        [Tooltip("This clip will be played once when you pick up the item here. It is played in 2D space. " +
                 "This can for example be a sound effect combined with voice instructions on what to do next.")]
        [SerializeField]
        private AudioClip _pickupClip;

        /// <summary>
        /// Event called when the item here is picked up.
        /// </summary>
        public event Action OnItemPicked;

        private AudioSource _source;

        private void Awake()
        {
            _source = GetComponent<AudioSource>();
        }

        public override void Interact()
        {
            base.Interact();

            // early return if we're already done here
            if (InteractedWith)
                return;

            InteractedWith = true;
            OnItemPicked?.Invoke();

            if (_pickupClip)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();
                source.clip = _pickupClip;
                source.Play();
            }

            _source.Stop();

            Debug.Log($"Picked up item at {name}");
        }
    }
}