using UnityEngine;
using Util;

namespace Bubble
{
    public class BubbleVisualEffects : NoteMonoBehavior
    {
        [Header("Depends")]

        [SerializeField]
        private Bubble _bubble;

        [SerializeField]
        private Transform _visualTransform;

        [Header("Radius Config")]

        [SerializeField]
        private float _radiusSpringConstant;

        [SerializeField]
        private float _radiusSpringDamping;

        [SerializeField]
        private float _radiusMargin;

        [Header("Position Config")]

        [SerializeField]
        private float _positionSpringConstant;

        [SerializeField]
        private float _positionSpringDamping;

        [SerializeField]
        private float _speedScale;

        private Vector2 _targetPosition;
        private Vector2 _visualPositionVelocity;
        private Vector2 _visualPosition;

        private float _targetRadius;
        private float _visualRadiusVelocity;
        private float _visualRadius;

        private void Awake()
        {
            _bubble.OnRadiusChanged += OnRadiusChanged;
            _bubble.OnPositionChanged += OnPositionChanged;
        }

        private void Update()
        {
            // Radius spring
            float displayRadius = _targetRadius + (_targetRadius > 0 ? _radiusMargin : 0);
            float deltaRadius = displayRadius - _visualRadius;
            float accel = deltaRadius * _radiusSpringConstant;
            accel -= _visualRadiusVelocity * _radiusSpringDamping;

            _visualRadiusVelocity += accel * Time.deltaTime;
            _visualRadius += _visualRadiusVelocity * Time.deltaTime * _speedScale;

            // Position spring
            Vector2 deltaPosition = _targetPosition - _visualPosition;
            Vector2 posAccel = deltaPosition * _positionSpringConstant;
            posAccel -= _visualPositionVelocity * _positionSpringDamping;

            _visualPositionVelocity += posAccel * Time.deltaTime;
            _visualPosition += _visualPositionVelocity * Time.deltaTime * _speedScale;

            _visualTransform.position = _visualPosition;
            _visualTransform.localScale = Vector2.one * _visualRadius;
        }

        private void OnDestroy()
        {
            _bubble.OnRadiusChanged -= OnRadiusChanged;
            _bubble.OnPositionChanged -= OnPositionChanged;
        }

        private void OnRadiusChanged(float radius)
        {
            _targetRadius = radius;
        }

        private void OnPositionChanged(Vector2 position)
        {
            _targetPosition = position;
        }
    }
}