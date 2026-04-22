using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

namespace SABI.SOA
{
    public abstract class BaseValueSO<T> : ScriptableObject
    {
        [SerializeField] private T value;
        private event Action<T> OnValueChange;
        [SerializeField] private bool eventOnReset = false;
        [SerializeField] private T resetValue = default;
        [SerializeField] private int totalSubscribers;
        [SerializeField] private List<MonoBehaviour> MonoBehaviourSubscribers = new();
        [SerializeField] private List<ScriptableObject> ScriptableObjectSubscribers = new();
        [SerializeField] private T debugValue = default;
        [SerializeField] private bool debugLog;
        [SerializeField] private string debugLogMessageStarting;
        [SerializeField] private string debugLogMessageEnding;
    
        public void SetValue(T newValue)
        {
            value = newValue;
            OnValueChange?.Invoke(value);
            if (debugLog) Debug.Log($" {debugLogMessageStarting} {value} {debugLogMessageEnding} ");
        }
    
        public T GetValue() => value;
    
        public void SetResetValue(T newResetValue) => resetValue = newResetValue;
    
        public void ResetValue()
        {
            if (eventOnReset) SetValue(resetValue);
            else value = resetValue;
        }
    
        public void DebugSetValue() => SetValue(debugValue);
    
        public void Subscribe(Object subscriber, Action<T> callBack)
        {
            OnValueChange += callBack;
            totalSubscribers++;
            if (subscriber is MonoBehaviour) MonoBehaviourSubscribers.Add((MonoBehaviour)subscriber);
            else if (subscriber is ScriptableObject) ScriptableObjectSubscribers.Add((ScriptableObject)subscriber);
        }
    
        public void UnSubscribe(Object subscriber, Action<T> callBack)
        {
            OnValueChange -= callBack;
            totalSubscribers--;
            if (subscriber is MonoBehaviour) MonoBehaviourSubscribers.Remove((MonoBehaviour)subscriber);
            else if (subscriber is ScriptableObject) ScriptableObjectSubscribers.Remove((ScriptableObject)subscriber);
        }
    }
}

