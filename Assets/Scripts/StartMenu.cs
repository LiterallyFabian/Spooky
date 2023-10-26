using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Spooky
{
    public class StartMenu : MonoBehaviour
    {
        [SerializeField] private Text _gameTitle;
        private void Awake()
        {
            Debug.Log(DateTime.Now);
            _gameTitle.text = GameConfig.Config.GameName;
            Time.timeScale = 1;
		AudioListener.volume = 1;
        }

        private void Update()
        {
            if (Input.anyKeyDown)
                SceneManager.LoadScene("StartScene");
        }
    }
}