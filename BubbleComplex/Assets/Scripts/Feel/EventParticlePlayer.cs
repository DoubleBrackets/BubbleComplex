using Events;
using UnityEngine;

namespace Feel
{
    /// <summary>
    ///  Starts and steps a particle system in response to events.
    /// </summary>
    public class EventParticlePlayer : MonoBehaviour
    {
        [Header("Event")]

        [SerializeField]
        private SOEvent _event;

        [SerializeField]
        private ParticleSystem _particleSystem;

        private void Awake()
        {
            _event.AddListener(PlayParticle);
        }

        private void OnDestroy()
        {
            _event.RemoveListener(PlayParticle);
        }

        private void PlayParticle()
        {
            _particleSystem.Play();
        }
    }
}