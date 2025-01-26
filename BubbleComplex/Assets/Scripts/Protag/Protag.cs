using Events;
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

        [Header("Event (In)")]

        [SerializeField]
        private SOEvent _onEnterNegative;

        [SerializeField]
        private SOEvent _onExitNegative;

        [Header("Stats")]

        [SerializeField]
        private MovementConfig _movementConfig;

        [SerializeField]
        private MovementConfig _negativeConfig;

        [SerializeField]
        private float _minimumHardenTime;

        [Header("Depends")]

        [SerializeField]
        private SimpleMovement _simpleMovement;

        [SerializeField]
        public Animator _animator;

        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        [SerializeField]
        private Bubble.Bubble _bubble;

        [SerializeField]
        private GlobalState _globalState;

        private MovementState _movementState;

        private Vector2 _horizontalMovement;
        private bool _isInteracting;

        private float _hardenTime;

        private bool _inNegative;

        private void Awake()
        {
            _onEnterNegative.AddListener(OnEnterNegative);
            _onExitNegative.AddListener(OnExitNegative);
        }

        private void Update()
        {
            GetInput();

            if (_movementState == MovementState.Normal)
            {
                _simpleMovement.Move(
                    _inNegative ? _negativeConfig : _movementConfig,
                    _horizontalMovement, Time.deltaTime);

                if (_inNegative)
                {
                    _animator.Play("Slowed");
                }
                else
                {
                    _animator.Play(WalkAnim(_horizontalMovement));
                }
            }
            else if (_movementState == MovementState.Harden)
            {
                _simpleMovement.StandStill(_movementConfig, Time.deltaTime);
                _hardenTime += Time.deltaTime;
            }

            _globalState.PlayerPos = _simpleMovement.Rb.position;
        }

        private void OnDestroy()
        {
            _onEnterNegative.RemoveListener(OnEnterNegative);
            _onExitNegative.RemoveListener(OnExitNegative);
        }

        private void OnEnterNegative()
        {
            _inNegative = true;
        }

        private void OnExitNegative()
        {
            _inNegative = false;
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

            if (_horizontalMovement.x > 0)
            {
                _spriteRenderer.flipX = false;
            }
            else if (_horizontalMovement.x < 0)
            {
                _spriteRenderer.flipX = true;
            }
        }

        private void EnterHarden()
        {
            _hardenTime = 0;
            _movementState = MovementState.Harden;
            _bubble.SetHardened(true);
            _animator.Play("HardenedDown");
        }

        private void ExitHarden()
        {
            _movementState = MovementState.Normal;
            _bubble.SetHardened(false);
            _animator.Play("HardenedUp");
        }

        private string WalkAnim(Vector2 input)
        {
            if (input == Vector2.zero)
            {
                return "Idle";
            }

            if (input.y > 0)
            {
                return "Walk Up R";
            }

            if (input.y < 0)
            {
                return "Walk Down R";
            }

            return "Walk Straight R";
        }
    }
}