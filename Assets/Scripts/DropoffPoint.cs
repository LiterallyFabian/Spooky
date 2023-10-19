using System;
using UnityEngine;

namespace Spooky
{
    [RequireComponent(typeof(AudioSource))]
    public class DropoffPoint : Interactable
    {
        public override bool Locked => !(_pickupPoint.InteractedWith || _invalidDropoffClip != null);

        [Tooltip("The pickup point that the player has to interact with to enable this point.")] [SerializeField]
        private PickupPoint _pickupPoint;

        /// <summary>
        /// Whether or not an item has been dropped here.
        /// </summary>
        public bool Enabled { get; private set; } = false;

        [Tooltip(
            "This clip will be played once when you place an item here. It is played in 2D space. " +
            "This can for example be a sound effect combined with voice instructions on what to do next.")]
        [SerializeField]
        private AudioClip _dropoffClip;

        [Tooltip("An (optional) audioclip that will play if the player tries to interact here without having the required item. If not provided, the interactable-sound will not play.")] [SerializeField]
        private AudioClip _invalidDropoffClip;

        /// <summary>
        /// Event called when the correct item is placed here.
        /// </summary>
        public event Action OnItemPlaced;

        private AudioSource _source;
        private AudioSource _invalidDropoffSource;


        [Tooltip("The volume when this point can be interacted with")]
        [SerializeField] private float _activeVolume = 0.5f;
        [Tooltip("The volume before this point can be interacted with")]
        [SerializeField] private float _idleVolume = 0.2f;
        [Tooltip("The volume when this point has been interacted with and is no longer relevant")]
        [SerializeField] private float _finishedVolume = 0.1f;

        [SerializeField] private bool dropOffIsVoice;
        [SerializeField] private bool selfDestruct = false;
        [SerializeField] private Dialogue dialogue;

        private void Awake()
        {
            _source = GetComponent<AudioSource>();
            _invalidDropoffSource = gameObject.AddComponent<AudioSource>();

            if (!_pickupPoint)
            {
                Debug.LogError($"Drop off point {name} does not have a {nameof(PickupPoint)}");
                return;
            }

            _source.volume = _idleVolume;
            _pickupPoint.OnItemPicked += () => _source.volume = _activeVolume;
        }

        public override void Interact()
        {
            base.Interact();

            // early return if we're already done here
            if (Enabled)
                return;

            if (_invalidDropoffClip != null && !_invalidDropoffSource.isPlaying && !_pickupPoint.InteractedWith)
            {
                dialogue.Say(_invalidDropoffClip);
            }

            // early return if we have an invalid or non-interacted pickup point
            if (_pickupPoint == null || !_pickupPoint.InteractedWith)
                return;

            Enabled = true;
            OnItemPlaced?.Invoke();

            if (_dropoffClip)
            {
                if(dropOffIsVoice)
                {
                    dialogue.Say(_dropoffClip);
                }else{
                    AudioSource source = gameObject.AddComponent<AudioSource>();
                    source.clip = _dropoffClip;
                    source.Play();
                }
            }

            Debug.Log($"Dropped off item at {name}");

            _source.volume = _finishedVolume;

            if(selfDestruct){
                gameObject.transform.position = new Vector3(gameObject.transform.position.x - 10, gameObject.transform.position.y, gameObject.transform.position.z);
            } 
        }
    }
}