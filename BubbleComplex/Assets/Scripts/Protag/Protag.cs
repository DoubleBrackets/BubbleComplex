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

        [SerializeField]
        private float _minimumHardenTime;

        [Header("Depends")]

        [SerializeField]
        private SimpleMovement _simpleMovement;

        [SerializeField]
        public Animator _animator;

        [SerializeField]
        private Bubble.Bubble _bubble;

        [SerializeField]
        private GlobalState _globalState;

        private MovementState _movementState;

        private Vector2 _horizontalMovement;
        private bool _isInteracting;

        private float _hardenTime;

        private void Update()
        {
            GetInput();

            if (_movementState == MovementState.Normal)
            {
                _simpleMovement.Move(_movementConfig, _horizontalMovement, Time.deltaTime);
            }
            else if (_movementState == MovementState.Harden)
            {
                _simpleMovement.StandStill(_movementConfig, Time.deltaTime);
                _hardenTime += Time.deltaTime;
            }

            _globalState.PlayerPos = _simpleMovement.Rb.position;
        }

        private void GetInput()
        {
            _horizontalMovement = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical"));
            _isInteracting = Input.GetKey(KeyCode.Space);

            if (_movementState == MovementState.Normal)
            {
                if (_isInteracting && _bubble.BubbleState != Bubble.Bubble.BubbleStates.Child)
                {
                    EnterHarden();
                }
            }

            if (_movementState == MovementState.Harden && !Input.GetKey(KeyCode.Space))
            {
                if (!_isInteracting && _hardenTime >= _minimumHardenTime)
                {
                    ExitHarden();
                }
            }
        }

        private void EnterHarden()
        {
            _hardenTime = 0;
            _movementState = MovementState.Harden;
            _bubble.SetHardened(true);
        }

        private void ExitHarden()
        {
            _movementState = MovementState.Normal;
            _bubble.SetHardened(false);
        }
    }
}