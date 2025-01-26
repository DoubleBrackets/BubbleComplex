using UnityEngine;

namespace Events
{
    [CreateAssetMenu(menuName = "Events/Basic/VoidEvent")]
    public class VoidEvent : SOEvent
    {
        public void Raise()
        {
            _internalVoidEvent?.Invoke();
        }
    }
}