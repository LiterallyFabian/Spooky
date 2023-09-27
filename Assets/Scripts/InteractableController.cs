using System.Collections.Generic;
using UnityEngine;
// ReSharper disable Unity.NoNullPropagation - null propagation is used for readability

namespace Spooky
{
    [RequireComponent(typeof(Collider))]
    public class InteractableController : MonoBehaviour
    {
        [SerializeField] private Transform _playerPivot;
        
        /// <summary>
        /// A set of all interactables currently in range.
        /// </summary>
        private readonly HashSet<Interactable> _interactables = new HashSet<Interactable>();

        /// <summary>
        /// The closest interactable in range. Null if there is none in range.
        /// </summary>
        public Interactable CurrentInteractable { get; private set; }
        
        private void Awake()
        {
            if (_playerPivot == null)
                _playerPivot = transform;
        }

        private void FixedUpdate()
        {
            Interactable closest = TryGetClosestInteractable();

            if (closest == CurrentInteractable) // nothing has changed
                return;

            CurrentInteractable?.Highlight(false);
            CurrentInteractable = closest;
            closest?.Highlight(true);
        }

        private void OnTriggerEnter(Collider other)
        {
            Interactable interactable = other.GetComponent<Interactable>();

            if (interactable)
                _interactables.Add(interactable);
        }

        private void OnTriggerExit(Collider other)
        {
            Interactable interactable = other.GetComponent<Interactable>();

            if (interactable)
                _interactables.Remove(interactable);
        }

        /// <summary>
        /// Gets the closest interactable in range. 
        /// </summary>
        /// <returns>The closest interactable in range. Null if none.</returns>
        private Interactable TryGetClosestInteractable()
        {
            Interactable closest = null;
            float closestDistance = float.MaxValue;

            foreach (Interactable interactable in _interactables)
            {
                if (interactable.Locked)
                    continue;
                
                float distance = Vector3.Distance(_playerPivot.position, interactable.transform.position);

                if (!(distance < closestDistance)) continue;

                closest = interactable;
                closestDistance = distance;
            }

            return closest;
        }
    }
}