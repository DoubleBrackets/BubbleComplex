using Events;
using UnityEngine;
using UnityEngine.Events;

namespace Feel
{
    public class EventMonoBehavior : MonoBehaviour
    {
        [SerializeField]
        private SOEvent _event;

        [SerializeField]
        private bool _oneOff;
    
        public UnityEvent OnEventRaised;
        
        private bool raised = false;
        
        private void OnEnable()
        {
            _event.AddListener(OnTrigger);
        }
        
        private void OnDisable()
        {
            _event.RemoveListener(OnTrigger);
        }

        private void OnTrigger()
        {
            if (raised && _oneOff)
            {
                return;
            }
            OnEventRaised.Invoke();
            raised = true;
        }
    }
}
