using System;
using Bubble;
using Global;
using UnityEditor;
using UnityEngine;
using Util;
using Random = UnityEngine.Random;

namespace NPC
{
    public class NegativeNPC : MonoBehaviour
    {
        private enum State
        {
            Wandering,
            TrappingPlayer
        }

        [SerializeField]
        private Bubble.Bubble _bubble;

        [SerializeField]
        private Transform _body;

        [Header("Wander Config")]

        [SerializeField]
        private float _wanderRadius;

        [SerializeField]
        private Vector2 _wanderChangeTimeRange;

        [Header("Depends")]

        [SerializeField]
        private SimpleMovement _simpleMovement;

        [SerializeField]
        private GlobalState _globalState;

        [Header("Movement Config")]

        [SerializeField]
        private MovementConfig _movementConfig;

        private State _state = State.Wandering;

        private Vector2 _wanderCenter;
        private Vector2 _targetPosition;

        private float _timeToNextWanderChange;

        private void Start()
        {
            _wanderCenter = _body.position;

            _bubble.OnAbsorbedOther.AddListener(OnAbsorbOther);
            _bubble.OnBecomeIndividual.AddListener(OnBecomeIndividual);
        }

        private void Update()
        {
            switch (_state)
            {
                case State.Wandering:
                    Wander();
                    break;
                case State.TrappingPlayer:
                    TrappingPlayer();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnDestroy()
        {
            _bubble.OnAbsorbedOther.RemoveListener(OnAbsorbOther);
            _bubble.OnBecomeIndividual.RemoveListener(OnBecomeIndividual);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_body.position, _wanderRadius);

#if UNITY_EDITOR
            Handles.Label(_body.position + Vector3.right, _state.ToString());
#endif

            if (Application.isPlaying)
            {
                Gizmos.DrawLine(_body.position, _targetPosition);
            }
        }

        private void OnBecomeIndividual()
        {
            if (_state == State.TrappingPlayer)
            {
                _state = State.Wandering;
            }
        }

        private void OnAbsorbOther(Bubble.Bubble other)
        {
            if (other.BubbleType == BubbleType.Player)
            {
                _state = State.TrappingPlayer;
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

        private void TrappingPlayer()
        {
            _simpleMovement.StandStill(_movementConfig, Time.deltaTime);
        }
    }
}