using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spooky
{
    public class TapePlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip[] tapes;
        //private AudioSource audioSource;
        private bool hasPlayed = false;
        private int numberTape = 0;
        [SerializeField] private Dialogue dialogue;

        // Start is called before the first frame update
        void Start()
        {
            PlayTape(numberTape);
        }

        // Update is called once per frame
        void Update()
        {
            //if (!hasPlayed)
            //{
            //    if (Input.GetKeyDown(KeyCode.Space))
            //    {
            //        if (!dialogue.IsPlaying())
            //        {
            //            PlayTape(numberTape);
            //            numberTape++;
            //        }
            //    }
            //}
            if (!dialogue.IsPlaying())
            {
                Player.ToggleInput(true);
            }

        }

        void PlayTape(int numberTape)
        {
            Player.ToggleInput(false);
            dialogue.Say(tapes[numberTape]);
            if (numberTape == 0)
            {
                hasPlayed = true;
            }

        }


    }
}

