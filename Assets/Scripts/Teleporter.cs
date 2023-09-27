using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Spooky
{
    [RequireComponent(typeof(Collider))]
    public class Teleporter : MonoBehaviour
    {
        [SerializeField] private string _scene;
       [SerializeField] private AudioClip _doorClose;
        
        private void OnTriggerEnter(Collider other)
        {
            StartCoroutine(FadeOut(1));
        }

        public IEnumerator FadeOut(float fadeTime)
        {
            Player.ToggleInput(false);
            
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = _doorClose;
            source.Play();

            yield return new WaitForSeconds(_doorClose.length);

            float startVolume = AudioListener.volume;

            while (AudioListener.volume > 0)
            {
                AudioListener.volume -= startVolume * Time.deltaTime / fadeTime;

                yield return null;
            }

            SceneManager.LoadScene(_scene);
        }
    }
}