using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

// ReSharper disable Unity.InefficientPropertyAccess - too small performance diff to fix

namespace Spooky
{
    [RequireComponent(typeof(Rigidbody))]
    public class Player : MonoBehaviour
    {
        private Rigidbody _rigidbody;
        private Transform _transform;

        [SerializeField] private float MovementSpeed = 2f;
        [SerializeField] private float RotationSpeed = 100f;

        [SerializeField] private AudioClip[] _footsteps;
        private float _unitsSinceLastStep = 0.5f;
        [SerializeField] private float _unitsBetweenStep = 0.9f;
        private Vector3 _lastPosition;

        public Image Cover;

        private InteractableController _interactableController;
        public SpookyInput Input { get; private set; }

        [SerializeField] private float _maxMotorSpeed = 0.5f;

        private void Start()
        {
            StartCoroutine(FadeIn()); // reset audio
            
            _rigidbody = GetComponent<Rigidbody>();
            _transform = transform;

            Input = new SpookyInput();
            Input.Enable();

            _lastPosition = transform.position;

            _interactableController = GetComponentInChildren<InteractableController>();

            Input.Player.Interact.performed += ctx => Interact();
        }

        private void Update()
        {
            //if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
            //    Cover.enabled = !Cover.enabled;
        }

        private void FixedUpdate()
        {
            float movementInput = Input.Player.Movement.ReadValue<float>();
            movementInput = Mathf.Clamp01(movementInput); // <-- Remove this line to be able to move backwards

            Vector2 rotationInputVector = Input.Player.Look.ReadValue<Vector2>();
            float rotationInput = rotationInputVector.x;

            // Target movement is the movement we *want* to make this frame
            Vector3 targetMovement = _transform.rotation * new Vector3(0, 0, movementInput) *
                                     (MovementSpeed * Time.fixedDeltaTime);
            _transform.position += targetMovement;
            _transform.rotation *= Quaternion.Euler(0, rotationInput * RotationSpeed * Time.fixedDeltaTime, 0);

            // Actual movement is the movement we *actually* made this frame
            Vector3 actualMovement = transform.position - _lastPosition;
            _unitsSinceLastStep += Mathf.Abs(actualMovement.magnitude);

            // Check if we should play a footstep
            if (_unitsSinceLastStep > _unitsBetweenStep)
            {
                _unitsSinceLastStep = 0;

                AudioSource a = gameObject.AddComponent<AudioSource>();
                AudioClip clip = _footsteps[Random.Range(0, _footsteps.Length)];
                a.clip = clip;
                a.Play();
                Destroy(a, clip.length);
            }

            float movementPercentage = actualMovement.magnitude / targetMovement.magnitude;
            float rotationSign = Mathf.Sign(rotationInput);

            // Set gamepad motor speeds based on reduced movement percentage
            if (Gamepad.current != null && movementPercentage < 0.98f && movementInput > 0.01f)
            {
                // Split the motorSpeed based on rotation direction
                float leftMotorSpeed = 1 - movementPercentage;
                float rightMotorSpeed = 1 - movementPercentage;

                // Adjust motorSpeeds based on rotation direction
                if (rotationSign > 0)
                    rightMotorSpeed *= 1.5f;
                else if (rotationSign < 0)
                    leftMotorSpeed *= 1.5f;

                Gamepad.current.SetMotorSpeeds(leftMotorSpeed * _maxMotorSpeed, rightMotorSpeed * _maxMotorSpeed);
            }

            if (movementInput < 0.01f || movementPercentage > 0.95f)
                Gamepad.current?.SetMotorSpeeds(0, 0);

            // Cache the current position over to the next frame
            _lastPosition = transform.position;
        }

        private void OnApplicationQuit()
        {
            if (Gamepad.current != null)
                Gamepad.current.SetMotorSpeeds(0, 0); // reset motor speeds
        }

        private void Interact()
        {
            Interactable i = _interactableController.CurrentInteractable;
            if (i == null)
            {
                Debug.LogWarning("Nothing to interact with here");
                return;
            }

            i.Interact();
        }

        public static void ToggleInput(bool enabled)
        {
            Player p = FindObjectOfType<Player>();

            if (p == null)
                return;

            if (enabled)
                p.Input.Enable();
            else
                p.Input.Disable();
        }

        public IEnumerator FadeIn()
        {
            float startVolume = AudioListener.volume;

            while (AudioListener.volume < 1)
            {
                AudioListener.volume += Time.deltaTime;

                yield return null;
            }
        }
    }
}