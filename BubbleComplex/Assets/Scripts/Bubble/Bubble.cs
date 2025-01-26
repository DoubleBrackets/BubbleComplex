using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Util;

namespace Bubble
{
    public enum BubbleType
    {
        Player,
        Friendly,
        Bad
    }

    public class Bubble : NoteMonoBehavior
    {
        public enum BubbleStates
        {
            Individual,
            Child,
            Parent
        }

        [Header("Config")]

        [SerializeField]
        private LayerMask _bubbleLayerMask;

        [SerializeField]
        private float _individualRadius;

        [SerializeField]
        private BubbleType _bubbleType;

        [Header("Depend")]

        [SerializeField]
        private CircleCollider2D _circleCollider2D;

        [SerializeField]
        private Transform _positionTracker;

        [SerializeField]
        private Transform _realTransform;

        [Header("Debug, Don't Change")]

        [SerializeField]
        private BubbleStates _bubbleState;

        [SerializeField]
        private Bubble _parentBubble;

        public UnityEvent<Bubble> OnAbsorbedByOther;
        public UnityEvent<Bubble> OnAbsorbedOther;

        public BubbleType BubbleType => _bubbleType;
        public float RealRadius => _realRadius;
        public BubbleStates BubbleState => _bubbleState;

        public float IndividualRadius => _individualRadius;
        public Vector2 RealPosition => _realPosition;
        public Vector2 IndividualPosition => _positionTracker.position;
        public event Action<float> OnRadiusChanged;
        public event Action<Vector2> OnPositionChanged;

        private readonly HashSet<Bubble> _childBubbles = new();

        // The target position and radius
        private float _realRadius;
        private Vector2 _realPosition;

        private void Start()
        {
            _realRadius = _individualRadius;
            _bubbleState = BubbleStates.Individual;
            _parentBubble = null;
        }

        private void Update()
        {
            _realTransform.position = _realPosition;
            TickBubble();
        }

        private void OnValidate()
        {
            SetRadius(_individualRadius);
        }

        private void SetRadius(float radius)
        {
            if (_realRadius == radius)
            {
                return;
            }

            _realRadius = radius;
            _circleCollider2D.radius = _realRadius;

            OnRadiusChanged?.Invoke(_realRadius);
        }

        private void SetPosition(Vector2 position)
        {
            if (_realPosition == position)
            {
                return;
            }

            _realPosition = position;
            OnPositionChanged?.Invoke(_realPosition);
        }

        private void TickBubble()
        {
            switch (_bubbleState)
            {
                case BubbleStates.Individual:
                    // Every third frame for performance
                    if (Time.frameCount % 3 == 0)
                    {
                        TickSeparated();
                    }

                    break;
                case BubbleStates.Child:
                    TickChild();
                    break;
                case BubbleStates.Parent:
                    TickParent();
                    break;
            }
        }

        private void TickParent()
        {
            CalculateStatsAsParent();
        }

        /// <summary>
        ///     Individual bubble state, check for overlapping with other bubbles for now
        /// </summary>
        private void TickSeparated()
        {
            CalculateStatsAsIndividual();

            Collider2D[] colls = Physics2D.OverlapCircleAll(_realPosition, _realRadius, _bubbleLayerMask);
            List<Bubble> overlappingBubbles = new();

            foreach (Collider2D coll in colls)
            {
                var entity = coll.GetComponentInParent<Bubble>();

                if (entity != null && entity != this)
                {
                    overlappingBubbles.Add(entity);
                }
            }

            foreach (Bubble bubble in overlappingBubbles)
            {
                // For simplicity, only the parent bubble has a valid collider
                if (bubble.BubbleState == BubbleStates.Child)
                {
                    continue;
                }

                float theirRadius = bubble.IndividualRadius;
                float ourRadius = _individualRadius;

                if (theirRadius > ourRadius)
                {
                    // We become a child
                    bubble.AbsorbBubble(this);
                }
                else
                {
                    // We are the one to absorb
                    AbsorbBubble(bubble);
                }
            }
        }

        private void AbsorbBubble(Bubble bubble)
        {
            if (_bubbleState == BubbleStates.Child)
            {
                return;
            }

            Debug.Log($"{name} ABSORBS {bubble.name}");

            // Steal their children
            foreach (Bubble child in bubble._childBubbles)
            {
                child.BecomeChild(this);
                _childBubbles.Add(this);
            }

            // Take them
            bubble.BecomeChild(this);
            _childBubbles.Add(bubble);

            // We are now a parent
            _bubbleState = BubbleStates.Parent;
            CalculateStatsAsParent();
        }

        private void BecomeChild(Bubble parentBubble)
        {
            _parentBubble = parentBubble;
            _bubbleState = BubbleStates.Child;
            CalculateStatsAsChild();
        }

        private void SeparateFromParent()
        {
            Debug.Log($"{name} SEPARATES FROM PARENT {_parentBubble.name}");

            _parentBubble.RemoveChild(this);
            _parentBubble = null;
            _bubbleState = BubbleStates.Individual;
            CalculateStatsAsIndividual();
        }

        private void RemoveChild(Bubble childBubble)
        {
            _childBubbles.Remove(childBubble);

            if (_childBubbles.Count == 0)
            {
                _bubbleState = BubbleStates.Individual;
                CalculateStatsAsIndividual();
            }
            else
            {
                CalculateStatsAsParent();
            }
        }

        private void TickChild()
        {
            if (_parentBubble == null)
            {
                _bubbleState = BubbleStates.Individual;
                return;
            }

            CalculateStatsAsChild();

            // If we leave the parent, we become an individual bubble
            float distToParent = Vector2.Distance(RealPosition, _parentBubble.RealPosition);
            if (distToParent > _parentBubble.RealRadius)
            {
                SeparateFromParent();
            }
        }

        /// <summary>
        ///     Calculate our "real" radius and position as a parent
        /// </summary>
        private void CalculateStatsAsParent()
        {
            float totalRadius = _individualRadius;

            foreach (Bubble child in _childBubbles)
            {
                totalRadius += child.IndividualRadius;
            }

            Vector2 weightedPosition = IndividualPosition * _individualRadius / totalRadius;
            foreach (Bubble child in _childBubbles)
            {
                weightedPosition += child.IndividualPosition * child.IndividualRadius / totalRadius;
            }

            SetRadius(totalRadius);
            SetPosition(weightedPosition);
        }

        /// <summary>
        ///     Calculate our "real" radius and position as a child
        /// </summary>
        private void CalculateStatsAsChild()
        {
            SetRadius(0);
            SetPosition(IndividualPosition);
        }

        /// <summary>
        ///     Calculate our "real" radius and position as an individual
        /// </summary>
        private void CalculateStatsAsIndividual()
        {
            SetRadius(_individualRadius);
            SetPosition(IndividualPosition);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_realPosition, _realRadius);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(IndividualPosition, _individualRadius);
        }
    }
}