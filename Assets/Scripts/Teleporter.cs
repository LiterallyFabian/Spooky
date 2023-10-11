using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Spooky
{
    [RequireComponent(typeof(Collider))]
    public class Teleporter : MonoBehaviour
    {
        [Tooltip("The scene to load from this teleporter")]
        [SerializeField] private string _scene;
        [SerializeField] private AudioClip _doorClose;

        private void OnTriggerEnter(Collider other)
        {
            StartCoroutine(Teleport(1));
        }

        public IEnumerator Teleport(float fadeTime)
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