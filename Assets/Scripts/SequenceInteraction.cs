using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spooky
{
    [System.Serializable]
    public class SequenceIndex
    {
        public SequenceInteractionComponent component;

        public GameObject[] objectsToEnable;
        [Header("Volumes")]
        public float loudVolume = 1;
        public float lowVolume = 0.05f;

        [Header("Sound")]
        public AudioClip clip;


        [Header("Oneshot")]
        public AudioClip oneShot;
        public float oneShotVolume;
    }
    public class SequenceInteraction : MonoBehaviour
    {
        [SerializeField]
        SequenceIndex[] sequence;
        int currentIndex = 0;


        private void Start()
        {
            for (int i = 0; i < sequence.Length; i++)
            {
                sequence[i].component.controller = this;
                sequence[i].component.GetComponent<AudioSource>().volume = sequence[i].lowVolume;
                if (sequence[i].clip)
                {
                    sequence[i].component.GetComponent<AudioSource>().clip = sequence[i].clip;
                    sequence[i].component.GetComponent<AudioSource>().Play();
                }
            }
            if (sequence.Length > 0)
            {
                sequence[0].component.GetComponent<AudioSource>().volume = sequence[0].loudVolume;
                sequence[0].component.isEnabled = true;
                print(sequence[0].component.name);
            }
        }

        public void increaseIndex()
        {
            sequence[currentIndex].component.isEnabled = false;
            sequence[currentIndex].component.GetComponent<AudioSource>().volume = sequence[currentIndex].lowVolume;
            if(sequence[currentIndex].oneShot)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();
                source.clip = sequence[currentIndex].oneShot;
                source.volume = sequence[currentIndex].oneShotVolume;
                source.Play();
                print("playOneshot");
            }
            currentIndex++;
            if (currentIndex < sequence.Length) 
            {
                SequenceInteractionComponent component = sequence[currentIndex].component;
                component.isEnabled = true;
                component.GetComponent<AudioSource>().volume = sequence[currentIndex].loudVolume;
                for (int i = 0; i < sequence[currentIndex].objectsToEnable.Length; i++)
                {
                    sequence[currentIndex].objectsToEnable[i].SetActive(true);
                }
                print(sequence[currentIndex].component.name);
            }
        }

    }
}
