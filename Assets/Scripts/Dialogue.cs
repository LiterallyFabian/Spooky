using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private SubtitleManager sm;

    void Awake()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    public void Say(AudioClip dialogue)
    {
        int line = int.Parse(dialogue.name.Replace("line ", ""));
        sm.StartSubtitle(line);
        audioSource.clip = dialogue;
        audioSource.Play();
    }

    public bool IsPlaying()
    {
        return audioSource.isPlaying;
    }
}
