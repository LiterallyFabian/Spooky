using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;


public class StartInstructions : MonoBehaviour
{
    [SerializeField] private AudioClip instructions, intro;
    [SerializeField] private Dialogue dialogue;
    private bool allDone = false;

    // Start is called before the first frame update
    void Start()
    {
        dialogue.Say(instructions);
    }

    // Update is called once per frame
    void Update()
    {
        if(!dialogue.IsPlaying() && allDone)
        {
            SceneManager.LoadScene(1);
        }

        if (!dialogue.IsPlaying() && (Input.GetKeyDown("joystick button 0") || Input.GetKeyDown("joystick button 1") || Input.GetKeyDown("joystick button 2") || Input.GetKeyDown("joystick button 3")))
        {
            dialogue.Say(intro);
            allDone = true;
            
        }
    }
}
