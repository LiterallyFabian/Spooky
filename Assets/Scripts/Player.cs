using UnityEngine;

namespace Spooky
{
    [RequireComponent(typeof(Rigidbody))]
    public class Player : MonoBehaviour
    {
        private Rigidbody _rigidbody;
        public float MovementSpeed = 2f;
        public float RotationSpeed = 100f;
        private Transform _transform;


        // Start is called before the first frame update
        void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _transform = transform;
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            float movement = Input.GetAxis("Movement");
            movement = Mathf.Clamp01(movement); // <-- remove 2 move backwards

            Quaternion rotation = _transform.rotation;
            _transform.position += rotation * new Vector3(0, 0, movement) * (MovementSpeed * Time.fixedDeltaTime);
            rotation *= Quaternion.Euler(0, Input.GetAxis("Look") * RotationSpeed * Time.fixedDeltaTime, 0);
            _transform.rotation = rotation;
        }
    }
}