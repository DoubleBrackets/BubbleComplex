using System;
using UnityEngine;

namespace Util
{
    [Serializable]
    public struct MovementConfig
    {
        public float Speed;
        public float Acceleration;
    }

    public class SimpleMovement : MonoBehaviour
    {
        [Header("Depends")]

        [SerializeField]
        private Rigidbody2D _rigidbody2D;

        public Rigidbody2D Rb => _rigidbody2D;

        public void Move(MovementConfig moveConfig, Vector2 direction, float deltaTime)
        {
            Vector2 currentVelocity = _rigidbody2D.linearVelocity;
            Vector2 targetVelocity = direction.normalized * moveConfig.Speed;

            Vector2 newVelocity = Vector2.Lerp(currentVelocity, targetVelocity, moveConfig.Acceleration * deltaTime);

            _rigidbody2D.linearVelocity = newVelocity;
        }

        public void StandStill(MovementConfig moveConfig, float deltaTime)
        {
            Move(moveConfig, Vector2.zero, deltaTime);
        }
    }
}