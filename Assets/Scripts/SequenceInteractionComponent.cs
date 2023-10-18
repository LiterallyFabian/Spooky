using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spooky
{
    [RequireComponent(typeof(AudioSource))]
    public class SequenceInteractionComponent : Interactable
    {

        public Component componentToEnable;
        // Start is called before the first frame update
        public SequenceInteraction controller;
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
