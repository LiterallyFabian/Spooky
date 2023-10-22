using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spooky
{
    [RequireComponent(typeof(AudioSource))]
    public class SequenceInteractionComponent : Interactable
    {
        [HideInInspector]
        public SequenceInteraction controller;
        [HideInInspector]
        public bool isEnabled = false;

        public override void Interact()
        {
            base.Interact();
            if (isEnabled)
            {
                if (controller)
                {
                    controller.increaseIndex();
                }
            }
        }
    }
}
