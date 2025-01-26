using Global;
using UnityEngine;
using Util;

namespace Protag
{
    public class Protag : MonoBehaviour
    {
        public enum MovementState
        {
            Normal,
            Harden
        }

        [Header("Stats")]

        [SerializeField]
        private MovementConfig _movementConfig;

        [Header("Depends")]

        [SerializeField]
        private SimpleMovement _simpleMovement;

        [SerializeField]
        private Animator _animator;

        [SerializeField]
        private GlobalState _globalState;

        private MovementState _movementState;

        private Vector2 _horizontalMovement;
        private bool _isInteracting;

        private void Update()
        {
            GetInput();

            if (_movementState == MovementState.Normal)
            {
                _simpleMovement.Move(_movementConfig, _horizontalMovement, Time.deltaTime);
            }

            _globalState.PlayerPos = _simpleMovement.Rb.position;
        }

        private void GetInput()
        {
            _horizontalMovement = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical"));
            _isInteracting = Input.GetKeyDown(KeyCode.Space);
        }
    }
}