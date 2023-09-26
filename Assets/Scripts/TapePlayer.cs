using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapePlayer : MonoBehaviour
{
    [SerializeField] private AudioClip[] tapes;
    private AudioSource audioSource;
    private bool hasPlayed = false;
    private int numberTape = 0;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!hasPlayed){
            if(Input.GetKeyDown(KeyCode.Space))
            {
                if(!audioSource.isPlaying){
                    PlayTape(numberTape);
                    numberTape++;
                }                
            }
        }
        
    }

    void PlayTape(int numberTape)
    {
        //Gör så att spelaren inte kan gå
        audioSource.clip = tapes[numberTape];
        audioSource.Play();
        if(numberTape == 1){
            hasPlayed = true;
        }
        
    }


}
