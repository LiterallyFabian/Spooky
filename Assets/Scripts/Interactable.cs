using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spooky
{
    /// <summary>
    /// An interactable is an object the player can interact with using the interact button (A?).
    /// The base class is mostly empty, but used for the <see cref="InteractableController"/>.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public abstract class Interactable : MonoBehaviour
    {
        /// <summary>
        /// Whether or not this interactable is locked. Locked interactables can not be interacted with.
        /// </summary>
        public virtual bool Locked => false;

        public virtual void Highlight(bool highlight)
        {
            if (!highlight)
                return;
            
            Debug.Log($"Highlighted object {gameObject.name}");

            AudioSource audio = gameObject.AddComponent<AudioSource>();
            AudioClip clip = Resources.Load<AudioClip>("InteractionSound");

            audio.clip = clip;
            audio.Play();

            Destroy(audio, clip.length);
        }

        public virtual void Interact()
        {
            Debug.Log($"Interacted with {gameObject.name}, type {GetType().Name}");
        }
    }
}