using UnityEngine;

namespace Player
{
    public class ProtagMovement : MonoBehaviour
    {
        public enum MovementState
        {
            Normal,
            Action
        }

        [Header("Stats")]

        [SerializeField]
        private float _speed;

        [SerializeField]
        private float _acceleration;

        [Header("Depends")]

        [SerializeField]
        private Rigidbody2D _rigidbody2D;

        [SerializeField]
        private Animator _animator;

        private MovementState _movementState;

        private Vector2 _horizontalMovement;
        private bool _isInteracting;

        private void Update()
        {
            GetInput();

            if (_movementState == MovementState.Normal)
            {
                SimpleMovement();
            }
        }

        private void SimpleMovement()
        {
            Vector2 currentVelocity = _rigidbody2D.linearVelocity;
            Vector2 targetVelocity = _horizontalMovement.normalized * _speed;

            Vector2 newVelocity = Vector2.Lerp(currentVelocity, targetVelocity, _acceleration * Time.deltaTime);

            _rigidbody2D.linearVelocity = newVelocity;
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