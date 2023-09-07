using UnityEngine;

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

        // Start is called before the first frame update
        void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _transform = transform;

            _lastPosition = transform.position; 
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            float movementInput = Input.GetAxis("Movement");
            movementInput = Mathf.Clamp01(movementInput); // <-- Remove this line to be able to move backwards

            _transform.position += _transform.rotation * new Vector3(0, 0, movementInput) * (MovementSpeed * Time.fixedDeltaTime);
            _transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Look") * RotationSpeed * Time.fixedDeltaTime, 0);

            Vector3 movedDistance = transform.position - _lastPosition;
            _unitsSinceLastStep += Mathf.Abs(movedDistance.magnitude);

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

            // Cache the current position over to the next frame
            _lastPosition = transform.position;
        }
    }
}