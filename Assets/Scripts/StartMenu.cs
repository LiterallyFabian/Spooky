using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Spooky
{
    public class StartMenu : MonoBehaviour
    {
        private void Update()
        {
            if (Input.anyKeyDown)
                SceneManager.LoadScene("StartScene");
        }
    }
}