using System.Collections;
using UnityEngine;

namespace Spooky
{
    internal enum SimonSaysState
    {
        /// <summary>The game is not running.</summary>
        Idle,

        /// <summary>The player can test the various keys.</summary>
        Practice,

        /// <summary>The game is playing a sequence.</summary>
        Running,

        /// <summary>The game is waiting for the player to input their key(s).</summary>
        Waiting,
        Completed,
    }

    public class SimonSays : Interactable
    {
        [SerializeField] private AudioClip[] _sounds = new AudioClip[4];

        private SimonSaysState _state = SimonSaysState.Idle;

        private int[] _sequence;

        /// <summary>
        /// The current position in the sequence the player is at.
        /// </summary>
        private int _currentPosition;

        /// <summary>
        /// The current key the player is at.
        /// </summary>
        private int _currentLevel = 1;

        public SpookyInput Input { get; private set; }

        [SerializeField] private AudioClip _instructions;
        [SerializeField] private AudioClip _tryAgain;
        [SerializeField] private AudioClip _victory;

        private AudioSource _instructionsSource;

        private void Awake()
        {
            Input = new SpookyInput();

            Input.SimonSays.KeyWest.performed += ctx => SubmitKey(0);
            Input.SimonSays.KeyNorth.performed += ctx => SubmitKey(1);
            Input.SimonSays.KeyEast.performed += ctx => SubmitKey(2);
            Input.SimonSays.KeySouth.performed += ctx => SubmitKey(3);

            Input.Player.Interact.performed += ctx => ActionInteract();

            _instructionsSource = gameObject.AddComponent<AudioSource>();
        }

        private void ActionInteract()
        {
            if (_state == SimonSaysState.Practice)
                StartCoroutine(RunSequence());
        }

        void Start()
        {
            if (_sounds.Length != 4)
                Debug.LogWarning($"An incorrect number of sounds ({_sounds.Length}) was provided.");
        }

        void Update()
        {
        }

        public override void Interact()
        {
            base.Interact();

            StartCoroutine(TryStartGame());
        }

        private IEnumerator TryStartGame()
        {
            if (_state != SimonSaysState.Idle)
                yield return null;

            Debug.Log("Starting game");

            _instructionsSource.clip = _instructions;
            _instructionsSource.Play();

            Player.ToggleInput(false);

            _state = SimonSaysState.Practice;
            _sequence = GenerateSequence();
            _currentPosition = 0;
            _currentLevel = 1;

            Input.Enable();
        }


        private static int[] GenerateSequence(int length = 5)
        {
            int[] seq = new int[length];

            for (int i = 0; i < length; i++)
            {
                seq[i] = Random.Range(0, 4);
                Debug.Log($"Generated key {seq[i]}");
            }

            return seq;
        }

        private IEnumerator RunSequence()
        {
            _instructionsSource.Stop();
            _state = SimonSaysState.Running;

            yield return new WaitForSeconds(2f);

            for (int i = 0; i < _currentLevel; i++)
            {
                PlaySound(_sequence[i]);
                yield return new WaitForSeconds(1);
            }

            _state = SimonSaysState.Waiting;
        }

        private void PlaySound(int index)
        {
            AudioSource a = gameObject.AddComponent<AudioSource>();
            AudioClip clip = _sounds[index];
            a.clip = clip;
            a.Play();
            Destroy(a, clip.length);
        }

        private void SubmitKey(int index)
        {
            if (_state is not (SimonSaysState.Practice or SimonSaysState.Waiting)) // Only allow practice or waiting-state
                return;

            PlaySound(index);

            if (_state == SimonSaysState.Practice) // Early return if we're just practicing.
                return;

            if (index != _sequence[_currentPosition])
            {
                Debug.Log("Wrong key!");

                _instructionsSource.clip = _tryAgain;
                _instructionsSource.Play();

                _state = SimonSaysState.Idle;
                Input.Disable();
                Player.ToggleInput(true);
                return;
            }

            _currentPosition++;

            if (_currentPosition == _sequence.Length)
            {
                Debug.Log("Sequence complete!");
                _state = SimonSaysState.Completed;
                _instructionsSource.clip = _victory;
                _instructionsSource.Play();
                
                Player.ToggleInput(false);
                Input.Disable();
                return;
            }
            else if (_currentPosition == _currentLevel)
            {
                _currentLevel++;
                _currentPosition = 0;

                StartCoroutine(RunSequence());
            }
        }
    }
}