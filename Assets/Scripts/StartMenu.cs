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
            _gameTitle.text = GameConfig.Config.GameName;
        }

        private void Update()
        {
            if (Input.anyKeyDown)
                SceneManager.LoadScene("StartScene");
        }
    }
}