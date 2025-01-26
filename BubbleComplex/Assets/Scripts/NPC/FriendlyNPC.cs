using System;
using Bubble;
using Global;
using UnityEditor;
using UnityEngine;
using Util;
using Random = UnityEngine.Random;

namespace NPC
{
    public class FriendlyNPC : MonoBehaviour
    {
        private enum State
        {
            Wandering,
            FollowingPlayer
        }

        [Header("Depends")]

        [SerializeField]
        private SimpleMovement _simpleMovement;

        [SerializeField]
        private Bubble.Bubble _bubble;

        [SerializeField]
        private GlobalState _globalState;

        [SerializeField]
        private Transform _body;

        [Header("Movement Config")]

        [SerializeField]
        private MovementConfig _movementConfig;

        [SerializeField]
        private MovementConfig _followMovementConfig;

        [SerializeField]
        private float _followDistance;

        [Header("Wander Config")]

        [SerializeField]
        private float _wanderRadius;

        [SerializeField]
        private Vector2 _wanderChangeTimeRange;

        private State _state = State.Wandering;

        private Vector2 _wanderCenter;
        private Vector2 _targetPosition;

        private float _timeToNextWanderChange;

        private void Start()
        {
            _wanderCenter = _body.position;

            _bubble.OnAbsorbedByOther.AddListener(OnAbsorbedByOther);
        }

        private void Update()
        {
            switch (_state)
            {
                case State.Wandering:
                    Wander();
                    break;
                case State.FollowingPlayer:
                    FollowPlayer();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnDestroy()
        {
            _bubble.OnAbsorbedByOther.RemoveListener(OnAbsorbedByOther);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_body.position, _wanderRadius);

#if UNITY_EDITOR
            Handles.Label(_body.position + Vector3.right, _state.ToString());
#endif

            if (Application.isPlaying)
            {
                Gizmos.DrawLine(_body.position, _targetPosition);
            }
        }

        private void OnAbsorbedByOther(Bubble.Bubble other)
        {
            if (other.BubbleType == BubbleType.Player)
            {
                _state = State.FollowingPlayer;
            }
        }

        private void Wander()
        {
            if (_timeToNextWanderChange <= 0)
            {
                _timeToNextWanderChange = Random.Range(_wanderChangeTimeRange.x, _wanderChangeTimeRange.y);
                _targetPosition = _wanderCenter + Random.insideUnitCircle * _wanderRadius;
            }
            else
            {
                _timeToNextWanderChange -= Time.deltaTime;
            }

            // Move towards wander target
            Vector2 direction = _targetPosition - (Vector2)_body.position;
            if (direction.magnitude > 0.5f)
            {
                _simpleMovement.Move(_movementConfig, direction, Time.deltaTime);
            }
            else
            {
                _simpleMovement.StandStill(_movementConfig, Time.deltaTime);
            }
        }

        private void FollowPlayer()
        {
            _targetPosition = _globalState.PlayerPos;

            Vector2 direction = _targetPosition - (Vector2)_body.position;
            if (direction.magnitude > _followDistance)
            {
                _simpleMovement.Move(_followMovementConfig, direction, Time.deltaTime);
            }
            else
            {
                _simpleMovement.StandStill(_followMovementConfig, Time.deltaTime);
            }
        }
    }
}