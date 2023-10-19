using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

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

        /// <summary>The game is finished and can no longer be used.</summary>
        Completed,
    }

    [RequireComponent(typeof(AudioSource))]
    public class SimonSays : Interactable
    {
        public override bool Locked => !CanBePlayed && _invalidDropoffClip == null;

        public bool CanBePlayed => !(_dropoffPoint != null && !_dropoffPoint.Enabled);

        /// <summary>
        /// An optional <see cref="DropoffPoint"/>.
        /// If provided, it will have to be <see cref="DropoffPoint.Enabled"/> for this puzzle to be unlocked.
        /// If null, this puzzle will always be unlocked.
        /// </summary>
        [Tooltip("If provided, the dropoff point will have to be Enabled for this puzzle to be unlocked." +
                 "If null, this puzzle will always be unlocked.")]
        [SerializeField]
        private DropoffPoint _dropoffPoint;

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

        [Tooltip("A sound that will be played once in 3D space when this puzzle unlocks.")] [SerializeField]
        private AudioClip _idleStart;

        [Tooltip("A sound that will loop in 3D space after the startup sound when this puzzle unlocks.")] [SerializeField]
        private AudioClip _idleLoop;

        private AudioSource _instructionsSource;
        private AudioSource _idleSource;
        private AudioSource _invalidDropoffSource;

        public event Action OnGameCompleted;

        [SerializeField] private int[] _riggedSequence;

        [Tooltip("If not provided we will use a randomized sequence.")] [SerializeField]
        private bool _useRiggedSequence;

        [Tooltip("Should this play the idle sound from start? If false, the idle loop will start once all pre-conditions are met.")] [SerializeField]
        private bool _alwaysPlayIdle = false;

        [Tooltip("An (optional) audioclip that will play if the player tries to interact here without having the required pre-conditions. If not provided, the interactable-sound will not play.")] [SerializeField]
        private AudioClip _invalidDropoffClip;

        //Här har Ella och Cassandra förstört grejer
        [SerializeField] private bool tryAgainIsVoice = false;
        [SerializeField] private Dialogue dialogue;

        private void Awake()
        {
            _idleSource = GetComponent<AudioSource>();
            Input = new SpookyInput();

            Input.SimonSays.KeyWest.performed += ctx => SubmitKey(0);
            Input.SimonSays.KeyNorth.performed += ctx => SubmitKey(1);
            Input.SimonSays.KeyEast.performed += ctx => SubmitKey(2);
            Input.SimonSays.KeySouth.performed += ctx => SubmitKey(3);

            Input.Player.Interact.performed += ctx => ActionInteract();

            _instructionsSource = gameObject.AddComponent<AudioSource>();
            _invalidDropoffSource = gameObject.AddComponent<AudioSource>();

            if (!_dropoffPoint || _alwaysPlayIdle)
            {
                StartCoroutine(PlayIdleSounds());
            }
            else
            {
                _dropoffPoint.OnItemPlaced += () => StartCoroutine(PlayIdleSounds());
            }
        }

        /// <summary>
        /// Event handler for when the player interacts outside of this puzzle, ie. starting it.
        /// For the interaction event inside of the puzzle, see <see cref="ActionInteract"/>
        /// </summary>
        public override void Interact()
        {
            base.Interact();

            if (!CanBePlayed && _invalidDropoffClip != null && !_invalidDropoffSource.isPlaying)
            {
                //_invalidDropoffSource.clip = _invalidDropoffClip;
                //_invalidDropoffSource.Play();
                dialogue.Say(_invalidDropoffClip);
            }

            // early return if we have a dropoff point and it is not enabled
            if (!CanBePlayed)
            {
                Debug.LogWarning($"This puzzle requires {_dropoffPoint.name} to be played.");
                return;
            }

            StartCoroutine(TryStartGame());
        }

        /// <summary>
        /// Event handler for when the player interacts inside this puzzle.
        /// Interaction is only used to start the sequence from practice.
        /// </summary>
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

        private IEnumerator TryStartGame()
        {
            if (_state != SimonSaysState.Idle)
                yield return null;

            Debug.Log("Starting game");

            //_instructionsSource.clip = _instructions;
            //_instructionsSource.Play();
            dialogue.Say(_instructions);

            Player.ToggleInput(false);

            _state = SimonSaysState.Practice;
            _sequence = _useRiggedSequence ? _riggedSequence : GenerateSequence();
            _currentPosition = 0;
            _currentLevel = 1;

            _idleSource.volume = 0.04f;

            Input.Enable();
        }


        private static int[] GenerateSequence(int length = 5)
        {
            int[] seq = new int[length];
            string text = "Sequence ";

            for (int i = 0; i < length; i++)
            {
                int num = Random.Range(0, 4);

                while (seq.Count(e => e == num) >= 2) // generate a new number if it appears more than twice
                    num = Random.Range(0, 4);

                seq[i] = num;
                text += seq[i];

                if (i < length - 1)
                    text += "-"; // add a dash if we have numbers left
            }

            Debug.Log(text);
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
            if (_state is not (SimonSaysState.Practice
                or SimonSaysState.Waiting)) // Only allow practice or waiting-state
                return;

            PlaySound(index);

            if (_state == SimonSaysState.Practice) // Early return if we're just practicing.
                return;

            if (index != _sequence[_currentPosition])
            {
                Debug.Log("Wrong key!");

                //Här också :)
                if(tryAgainIsVoice){
                    dialogue.Say(_tryAgain);
                }else{
                    _instructionsSource.clip = _tryAgain;
                    _instructionsSource.Play();
                }
                
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

                OnGameCompleted?.Invoke();

                _idleSource.Stop();

                Player.ToggleInput(true);
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

        private IEnumerator PlayIdleSounds()
        {
            if (_idleSource.isPlaying)
                yield return null;

            _idleSource.clip = _idleStart;
            _idleSource.Play();

            yield return new WaitForSeconds(_idleStart.length);

            // early return if game is not idle
            if (_state != SimonSaysState.Idle)
                yield return null;

            _idleSource.Stop();
            _idleSource.clip = _idleLoop;
            _idleSource.loop = true;
            _idleSource.Play();
        }
    }
}