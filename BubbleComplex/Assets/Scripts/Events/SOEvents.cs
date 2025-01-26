using System;
using UnityEngine;

namespace Events
{
    public abstract class SOEvent : ScriptableObject
    {
        [SerializeField]
        [TextArea]
        private string _description;

        protected Action _internalVoidEvent;

        public void AddListener(Action listener)
        {
            _internalVoidEvent += listener;
        }

        public void RemoveListener(Action listener)
        {
            _internalVoidEvent -= listener;
        }
    }

    public abstract class SOEvent<T> : SOEvent
    {
        private event Action<T> _internalEvent;

        public void Raise(T value)
        {
#if UNITY_EDITOR
            Debug.Log($"Event {name} raised with value {value}");
#endif
            _internalEvent?.Invoke(value);
            _internalVoidEvent?.Invoke();
        }

        public void AddListener(Action<T> listener)
        {
            _internalEvent += listener;
        }

        public void RemoveListener(Action<T> listener)
        {
            _internalEvent -= listener;
        }
    }
}