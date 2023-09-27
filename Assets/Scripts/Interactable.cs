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
        public virtual bool Locked => false;
        
        public virtual void Highlight(bool highlight)
        {
            Debug.Log($"Highlighted object {gameObject.name}, type {GetType().Name}, highlight: {highlight}");

            if (highlight)
            {
                AudioSource audio = gameObject.AddComponent<AudioSource>();
                AudioClip clip = Resources.Load<AudioClip>("InteractableSound");

                audio.clip = clip;
                audio.Play();

                Destroy(audio, clip.length);
            }
        }

        public virtual void Interact()
        {
            Debug.Log($"Interacted with {gameObject.name}, type {GetType().Name}");
        }
    }
}