using System;
using UnityEngine;
using UnityEngine.Events;

namespace SinforosoPackage
{
    [Serializable]
    public class HotKeyCheat
    {
        [SerializeField] private string _description;
        [SerializeField] private KeyCode _keyCode;
        [SerializeField] private UnityEvent _unityEvent;

        public string Description => _description;
        public KeyCode KeyCode => _keyCode;
        public UnityEvent UnityEvent => _unityEvent;
    }
}
