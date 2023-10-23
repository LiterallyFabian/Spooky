using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spooky
{
    public class TapePlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip tape, comment;
        //private AudioSource audioSource;
        private bool hasPlayed = false;
        [SerializeField] private Dialogue dialogue;

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(playAudioSequentially());
        }

        void PlayTape(AudioClip clip)
        {
            Player.ToggleInput(false);
            dialogue.Say(clip);
        }

        void EndTape()
        {
            Player.ToggleInput(true);
        }

        IEnumerator playAudioSequentially()
        {
            //yield return null;

            //3.Play Audio
            PlayTape(tape);

            //4.Wait for it to finish playing
            while (dialogue.IsPlaying())
            {
                yield return null;
            }

            if(comment != null){
                yield return new WaitForSeconds(1);
                PlayTape(comment);

                while (dialogue.IsPlaying())
                {
                    yield return null;
                }
            }

            EndTape();
        }


    }
}

